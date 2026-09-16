terraform {
  required_providers {
    hcloud = {
      source  = "hetznercloud/hcloud"
      version = "~> 1.68"
    }
  }
}

provider "hcloud" {
  token = var.hcloud_token
}

module "app_server" {
  source = "./modules/app-server"

  ssh_key_name = var.ssh_key_name
  server_name  = var.server_name
  server_type  = var.server_type
  location     = var.location
  image        = var.image
}