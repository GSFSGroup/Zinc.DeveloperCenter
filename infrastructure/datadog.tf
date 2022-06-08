module "datadog" {
  source     = "./modules/datadog"
  app_name   = local.app_name
  short_name = local.short_name
  scientist  = local.scientist
  domain     = local.domain
}
