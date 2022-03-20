terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=2.99.0"
    }
  }
}

provider "azurerm" {
  features {}
}

####################
#### RG ############
####################
resource "azurerm_resource_group" "rg" {
  name                = "${var.resourcePrefix}-web-rg"
  location            = var.location
}

####################
## AppServicePlan ##
####################
resource "azurerm_app_service_plan" "asp" {
  name                = "${var.resourcePrefix}-asp"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name

  sku {
    tier = "Basic"
    size = "B1"
  }
}

resource "azurerm_app_service" "web" {
  name                = "${var.resourcePrefix}-web"
  location            = var.location
  resource_group_name = azurerm_app_service_plan.asp.resource_group_name

  app_service_plan_id = azurerm_app_service_plan.asp.id

  site_config {
    dotnet_framework_version  = "v4.0"
  }

  source_control {
    repo_url           = var.repoUrl
    branch             = var.repoBranch
    manual_integration = true
    use_mercurial      = false
  }

  app_settings = {}
}