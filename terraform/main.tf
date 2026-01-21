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

# SQL Server Auditing
resource "azurerm_mssql_server_extended_auditing_policy" "main" {
  server_id                               = azurerm_mssql_server.main.id
  storage_endpoint                        = azurerm_storage_account.audit_logs.primary_blob_endpoint
  storage_account_access_key              = azurerm_storage_account.audit_logs.primary_access_key
  storage_account_access_key_is_secondary = false
  retention_in_days                       = 30
}

# Storage Account for Audit Logs
resource "azurerm_storage_account" "audit_logs" {
  name                     = "sqlaudit${random_string.audit_suffix.result}"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = var.resources_location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  # Enable blob soft delete for audit log protection
  blob_properties {
    delete_retention_policy {
      days = 7
    }
  }
}

# Random suffix for storage account
resource "random_string" "audit_suffix" {
  length  = 8
  special = false
  upper   = false
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

# Holds container images
resource "azurerm_container_registry" "main" {
  name                = "financetracker${random_string.audit_suffix.result}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "Basic"
  admin_enabled       = false
}

# Service plan to manage app
resource "azurerm_service_plan" "main" {
  name                = "finance-tracker-plan"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  os_type             = "Linux"
  sku_name            = "F1"
}

# Get my public IP so only I can access the app (for now)
data "http" "my_public_ip" {
  url = "https://ifconfig.me/ip"
}

# The actual App Service
resource "azurerm_linux_web_app" "main" {
  name                = "app-finance-tracker-${random_string.audit_suffix.result}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  service_plan_id     = azurerm_service_plan.main.id

  enabled                       = true
  public_network_access_enabled = true

  # Turn on Managed Identity
  identity {
    type = "SystemAssigned"
  }

  site_config {
    acr_use_managed_identity_credentials = true
    always_on                            = false

    # Placeholder image
    application_stack {
      docker_image_name   = "mcr.icrosoft.com/dotnet/aspnet:8.0"
      docker_registry_url = "https://${azurerm_container_registry.main.login_server}"
    }

    app_settings = {
      "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
      "DOCKER_REGISTRY_SERVER_URL"          = "https://${azurerm_container_registry.main.login_server}"
    }

    ip_restriction {
      name       = "AllowMyHomeIP"
      ip_address = "${chomp(data.http.my_public_ip.response_body)}/32"
      action     = "Allow"
      priority   = 100
    }
  }
}

# Allows app service to pull from contanier registry
resource "azurerm_role_assignment" "acr_pull" {
  scope                = azurerm_container_registry.main.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_linux_web_app.main.identity[0].principal_id
}

# Link the DNS Zone to your VNet
resource "azurerm_private_dns_zone_virtual_network_link" "app" {
  name                  = "app-dns-link"
  resource_group_name   = azurerm_resource_group.main.name
  private_dns_zone_name = azurerm_private_dns_zone.app.name
  virtual_network_id    = azurerm_virtual_network.main.id
}