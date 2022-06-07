"Running tests (matching build server configuration)...`n"

$env:ASPNETCORE_ENVIRONMENT="docker-circleci"
"ASPNETCORE_ENVIRONMENT -> $env:ASPNETCORE_ENVIRONMENT"

write-output "`nbuild all images..."
rl-docker-compose-test build --pull

write-output "`nbring up the app..."
rl-docker-compose-test up -d developercenter

write-output "`nrun csharp tests..."
rl-docker-compose-test run --no-deps developercenter-tests-csharp

write-output "`nrun spa unit tests..."
rl-docker-compose-test run --no-deps developercenter-tests-spa-unit

# write-output "`nrun spa integration tests..."
# rl-docker-compose-test run --no-deps developercenter-tests-spa-integration
