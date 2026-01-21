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

output "app_service_default_site_hostname" {
  value = azurerm_linux_web_app.main.default_hostname
}

output "app_private_ip" {
  value = azurerm_private_endpoint.app.private_service_connection[0].private_ip_address
}

output "acr_login_server" {
  value = azurerm_container_registry.main.login_server
}