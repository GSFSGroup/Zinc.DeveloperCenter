#!/usr/bin/env bash
. "$RL_DOCKER_VOLS_ROOT/deps/set-redline-env.sh"

echo "Running tests (matching build server configuration)..."

ASPNETCORE_ENVIRONMENT=docker-circleci
echo "ASPNETCORE_ENVIRONMENT -> $ASPNETCORE_ENVIRONMENT"

echo "build all images..."
rl-docker-compose-test build --pull

echo "bring up the app..."
rl-docker-compose-test up -d developercenter

echo "run csharp tests..."
rl-docker-compose-test run --no-deps developercenter-tests-csharp

echo "run spa unit tests..."
rl-docker-compose-test run --no-deps developercenter-tests-spa-unit

# echo "run spa integration tests..."
# rl-docker-compose-test run --no-deps developercenter-tests-spa-integration
