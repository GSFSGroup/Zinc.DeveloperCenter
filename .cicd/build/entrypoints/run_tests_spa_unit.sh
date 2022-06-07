#!/bin/bash
set -euo pipefail

# This is the entrypoint script for angular unit tests; currently referred by the Dockerfile test image target

echo "Running unit tests..."
npm run test:cicd

echo "Copying SPA Unit coverage from ${SPA_UNIT_COVERAGE_PATH}..."
cp -vR ${SPA_UNIT_COVERAGE_PATH}/* /coverage/spa
