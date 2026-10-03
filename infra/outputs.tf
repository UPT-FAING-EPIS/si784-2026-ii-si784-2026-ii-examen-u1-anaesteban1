output "api_url" {
  description = "Published API URL."
  value       = "https://${azurerm_linux_web_app.api.default_hostname}"
}
