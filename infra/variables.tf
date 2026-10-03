variable "location" {
  description = "Azure region for the resource group."
  type        = string
  default     = "eastus"
}

variable "app_location" {
  description = "Azure region for App Service and PostgreSQL resources."
  type        = string
  default     = "brazilsouth"
}

variable "resource_group_name" {
  description = "Resource group name."
  type        = string
  default     = "rg-lost-found-university"
}

variable "app_name" {
  description = "Linux Web App name."
  type        = string
  default     = "lost-found-university-api"
}

variable "container_image" {
  description = "Backend container image to run."
  type        = string
  default     = "ghcr.io/upt-faing-epis/lost-found-university-api:latest"
}
