locals {
  app_name               = var.app_name
  short_name             = var.short_name
  short_name_underscored = replace(var.short_name, "-", "_")
  cluster                = "${var.scientist}.${var.domain}"
  dashboard_title        = "${var.short_name}-${var.scientist}-dashboard"
  scientist              = var.scientist
}
