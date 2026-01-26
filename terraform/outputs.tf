output "resource_group_name" {
  description = "Name of the resource group"
  value       = azurerm_resource_group.main.name
}

output "sql_server_fqdn" {
  description = "Fully qualified domain name of the SQL Server"
  value       = azurerm_mssql_server.main.fully_qualified_domain_name
}

output "vnet_id" {
  description = "ID of the virtual network"
  value       = azurerm_virtual_network.main.id
}

output "private_endpoint_ip" {
  description = "Private IP address of the SQL Server"
  value       = azurerm_private_endpoint.sql.private_service_connection[0].private_ip_address
}

output "acr_login_server" {
  value = azurerm_container_registry.main.login_server
}

output "acr_admin_username" {
  description = "ACR admin username for Docker login"
  value       = azurerm_container_registry.main.admin_username
  sensitive   = true
}

output "acr_admin_password" {
  description = "ACR admin password for Docker login"
  value       = azurerm_container_registry.main.admin_password
  sensitive   = true
}

output "key_vault_uri" {
  description = "URI of the Key Vault"
  value       = azurerm_key_vault.main.vault_uri
}

output "app_service_url" {
  description = "Full HTTPS URL of the app"
  value       = "https://${azurerm_linux_web_app.main.default_hostname}"
}