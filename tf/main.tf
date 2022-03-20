provider "azurerm" {
  version = "=2.99.0"
  features {}
}

####################
#### RG ############
####################
resource "azurerm_resource_group" "rg" {
  name                          = "${var.resourcePrefix}-web-rg"
  location                      = var.location
}

####################
## AppServicePlan ##
####################
resource "azurerm_app_service_plan" "asp" {
  name                = "${var.resourcePrefix}-asp"
  location            = var.location
  resource_group_name = azurerm_resource_group.rg.name

  kind                = "Linux"
  reserved            = true

  sku {
    tier = "Free"
    size = "F1"
  }
}

resource "azurerm_linux_web_app" "app" {
  name                = "${var.resourcePrefix}-app"
  location            = var.location
  resource_group_name = azurerm_app_service_plan.asp.resource_group_name

  service_plan_id     = azurerm_service_plan.rg.id

  site_config {}

  app_settings = {
  }
}