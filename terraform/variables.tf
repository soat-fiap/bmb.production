variable "profile" {
  description = "AWS profile name"
  type        = string
  default     = "default"
}

variable "region" {
  description = "AWS region"
  type        = string
  default     = "us-east-1"
}

variable "eks_cluster_name" {
  type    = string
  default = "eks_dev_quixada"
}

variable "jwt_signing_key" {
  type      = string
  sensitive = true
  default = "PkOhRwy6UtniEMo7lLWp3bADctYgnDHCTvH+2YkDeGg="
}

variable "jwt_issuer" {
  type      = string
  sensitive = false
  default   = "https://localhost:7004"
}

variable "jwt_aud" {
  type      = string
  sensitive = false
  default   = "https://localhost:7004"
}

variable "api_docker_image" {
  type    = string
  default = "ghcr.io/soat-fiap/bmb.production/api:1.1.0"
}

variable "internal_elb_name" {
  type    = string
  default = "kitchen-api-internal-elb"
}

variable "api_access_key_id" {
  type      = string
  nullable  = false
  sensitive = true
  default = ""
}

variable "api_secret_access_key" {
  type      = string
  nullable  = false
  sensitive = true
  default = ""
}