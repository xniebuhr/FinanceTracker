# Configure Terraform and Azure Provider
terraform {
  required_version = ">= 1.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }

  # Backend configuration for remote state
  backend "azurerm" {
    resource_group_name  = "tf-state-rg"
    storage_account_name = "xnfinancetfstate"
    container_name       = "tfstate"
    key                  = "finance-tracker.tfstate"
  }
}

provider "azurerm" {
  features {}
}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.resource_group_location
}

# Virtual Network
resource "azurerm_virtual_network" "main" {
  name                = var.vnet_name
  location            = var.resources_location
  resource_group_name = azurerm_resource_group.main.name
  address_space       = ["10.0.0.0/16"]
}

# App Subnet
resource "azurerm_subnet" "app" {
  name                 = "app-subnet"
  resource_group_name  = azurerm_resource_group.main.name
  virtual_network_name = azurerm_virtual_network.main.name
  address_prefixes     = ["10.0.1.0/24"]

  delegation {
    name = "app-service-delegation"

    service_delegation {
      name    = "Microsoft.Web/serverFarms"
      actions = ["Microsoft.Network/virtualNetworks/subnets/action"]
    }
  }
}

# Database Subnet (for private endpoint)
resource "azurerm_subnet" "db" {
  name                 = "db-subnet"
  resource_group_name  = azurerm_resource_group.main.name
  virtual_network_name = azurerm_virtual_network.main.name
  address_prefixes     = ["10.0.2.0/24"]
}

# SQL Server
resource "azurerm_mssql_server" "main" {
  name                         = var.sql_server_name
  resource_group_name          = azurerm_resource_group.main.name
  location                     = var.resources_location
  version                      = "12.0"
  administrator_login          = var.sql_admin_username
  administrator_login_password = var.sql_admin_password

  # Public access disabled (you turned this off manually)
  public_network_access_enabled = false

  # Minimum TLS version for security
  minimum_tls_version = "1.2"

  # Just used for me to login with AD
  azuread_administrator {
    login_username = "xniebuhr_gmail.com#EXT#@xniebuhrgmail.onmicrosoft.com"
    object_id      = "5012371b-d523-4081-bd71-4e0be3c207d5"
  }
}

# SQL Database
resource "azurerm_mssql_database" "main" {
  name      = var.sql_database_name
  server_id = azurerm_mssql_server.main.id

  # Specify local storage otherwise terraform apply fails
  storage_account_type = "Local"

  # Payment tier
  sku_name = "GP_S_Gen5_2"

  # Backup retention
  max_size_gb = 32
}

# Private DNS Zone for SQL
resource "azurerm_private_dns_zone" "sql" {
  name                = "privatelink.database.windows.net"
  resource_group_name = azurerm_resource_group.main.name
}

# Link DNS Zone to VNet
resource "azurerm_private_dns_zone_virtual_network_link" "sql" {
  name                  = "k3qgu7mm44r4k"
  resource_group_name   = azurerm_resource_group.main.name
  private_dns_zone_name = azurerm_private_dns_zone.sql.name
  virtual_network_id    = azurerm_virtual_network.main.id
  registration_enabled  = false
}

# Private Endpoint for SQL Server
resource "azurerm_private_endpoint" "sql" {
  name                          = "sql-private-endpoint"
  location                      = var.resources_location
  resource_group_name           = azurerm_resource_group.main.name
  subnet_id                     = azurerm_subnet.db.id
  custom_network_interface_name = "sql-private-endpoint-nic"

  private_service_connection {
    name                           = "sql-private-endpoint"
    private_connection_resource_id = azurerm_mssql_server.main.id
    subresource_names              = ["sqlServer"]
    is_manual_connection           = false
  }

  private_dns_zone_group {
    name                 = "default"
    private_dns_zone_ids = [azurerm_private_dns_zone.sql.id]
  }
}

# Network Security Group for Database Subnet
resource "azurerm_network_security_group" "db" {
  name                = "db-nsg"
  location            = var.resources_location
  resource_group_name = azurerm_resource_group.main.name

  # Allow SQL traffic from app subnet
  security_rule {
    name                       = "allow-app-to-sql"
    priority                   = 100
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "1433"
    source_address_prefix      = "10.0.1.0/24"
    destination_address_prefix = "*"
  }
}

# Associate NSG with Database Subnet
resource "azurerm_subnet_network_security_group_association" "db" {
  subnet_id                 = azurerm_subnet.db.id
  network_security_group_id = azurerm_network_security_group.db.id
}
