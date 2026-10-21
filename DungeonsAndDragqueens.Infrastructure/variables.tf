variable "hcloud_token" {
  type        = string
  sensitive   = true
  description = "Hetzner Cloud API token"
}

variable "ssh_key_name" {
  type        = string
  description = "Navnet på SSH-nøkkelen i Hetzner"
}

variable "server_name" {
  type    = string
  default = "todo-app-server"
}

variable "server_type" {
  type    = string
  default = "cx22"
}

variable "location" {
  type    = string
  default = "fsn1"
}

variable "image" {
  type    = string
  default = "ubuntu-24.04"
}