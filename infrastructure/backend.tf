terraform {
  backend "remote" {
    hostname     = "app.terraform.io"
    organization = "GSFSGroup"

    workspaces {
      prefix = "zn-developercenter-"
    }
  }
}
