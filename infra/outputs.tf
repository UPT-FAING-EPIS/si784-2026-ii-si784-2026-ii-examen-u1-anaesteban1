output "api_url" {
  description = "Published API URL."
  value       = "https://${azurerm_linux_web_app.api.default_hostname}"
}

output "resource_group_name" {
  description = "Resource group name."
  value       = azurerm_resource_group.main.name
}

output "web_app_name" {
  description = "Web App name."
  value       = azurerm_linux_web_app.api.name
}

output "web_app_hostname" {
  description = "Web App hostname."
  value       = azurerm_linux_web_app.api.default_hostname
}

output "postgres_server_fqdn" {
  description = "PostgreSQL server FQDN."
  value       = azurerm_postgresql_flexible_server.main.fqdn
}

output "postgres_database_name" {
  description = "PostgreSQL database name."
  value       = azurerm_postgresql_flexible_server_database.main.name
}

output "postgres_connection_string" {
  description = "PostgreSQL connection string used by the app."
  value       = "Host=${azurerm_postgresql_flexible_server.main.fqdn};Port=5432;Database=${azurerm_postgresql_flexible_server_database.main.name};Username=${azurerm_postgresql_flexible_server.main.administrator_login};Password=${random_password.postgres_admin.result};SSL Mode=Require;Trust Server Certificate=true"
  sensitive   = true
}
