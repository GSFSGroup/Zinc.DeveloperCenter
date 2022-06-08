#!/bin/bash
set -euo pipefail

# This is the entrypoint script for angular integration tests; currently referred by the Dockerfile test image target

echo "Waiting for Web dependencies: $baseUrl"
until $(curl -X GET --output /dev/null --silent --head --fail $baseUrl); do
    echo "Waiting for dependency: $baseUrl"
    sleep 5;
done

echo "Dependencies ready."

echo "Running integration tests..."
#CYPRESS_baseUrl=$baseUrl npx cypress run --browser chrome --record --reporter junit --reporter-options "mochaFile=$CYPRESS_testResults"
CYPRESS_baseUrl=$baseUrl npx cypress run --browser electron --record --reporter junit --reporter-options "mochaFile=$CYPRESS_testResults"
