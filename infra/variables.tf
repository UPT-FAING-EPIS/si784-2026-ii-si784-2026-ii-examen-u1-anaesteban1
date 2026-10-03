variable "location" {
  description = "Azure region for all resources."
  type        = string
  default     = "eastus"
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
  default     = "ghcr.io/owner/lost-found-university-api:latest"
}

variable "postgres_connection_string" {
  description = "PostgreSQL connection string injected from GitHub Secrets."
  type        = string
  sensitive   = true
}
