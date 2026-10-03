resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_service_plan" "main" {
  name                = "${var.app_name}-plan"
  location            = var.app_location
  resource_group_name = azurerm_resource_group.main.name
  os_type             = "Linux"
  sku_name            = "B1"
}

resource "random_password" "postgres_admin" {
  length      = 24
  min_lower   = 1
  min_numeric = 1
  min_upper   = 1
  special     = false
}

resource "azurerm_postgresql_flexible_server" "main" {
  name                          = "${var.app_name}-db"
  resource_group_name           = azurerm_resource_group.main.name
  location                      = var.app_location
  version                       = "16"
  administrator_login           = "lostfoundadmin"
  administrator_password        = random_password.postgres_admin.result
  public_network_access_enabled = true
  sku_name                      = "B_Standard_B1ms"
  storage_mb                    = 32768
}

resource "azurerm_postgresql_flexible_server_database" "main" {
  name      = "lostfound"
  server_id = azurerm_postgresql_flexible_server.main.id
  charset   = "UTF8"
  collation = "en_US.utf8"
}

resource "azurerm_postgresql_flexible_server_firewall_rule" "azure_services" {
  name             = "allow-azure-services"
  server_id        = azurerm_postgresql_flexible_server.main.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

resource "azurerm_linux_web_app" "api" {
  name                = var.app_name
  location            = var.app_location
  resource_group_name = azurerm_resource_group.main.name
  service_plan_id     = azurerm_service_plan.main.id

  site_config {
    application_stack {
      docker_image_name   = var.container_image
      docker_registry_url = "https://ghcr.io"
    }
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT"     = "Production"
    "POSTGRES_CONNECTION_STRING" = "Host=${azurerm_postgresql_flexible_server.main.fqdn};Port=5432;Database=${azurerm_postgresql_flexible_server_database.main.name};Username=${azurerm_postgresql_flexible_server.main.administrator_login};Password=${random_password.postgres_admin.result};SSL Mode=Require;Trust Server Certificate=true"
  }
}
