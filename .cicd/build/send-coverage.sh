#!/bin/bash
set -o pipefail

# https://stackoverflow.com/questions/5947742/how-to-change-the-output-color-of-echo-in-linux
# https://github.com/ryanoasis/public-bash-scripts/blob/master/unix-color-codes-not-escaped.sh
REDBI='\033[1;91m'
NC='\033[0m'

if [[ -z $RL_DOCKER_VOLS_ROOT ]]; then
    echo -e "${REDBI}Must define RL_DOCKER_VOLS_ROOT${NC}"
    exit 1
fi

# Needs to be set for get.sh
if [[ -z $CODACY_PROJECT_TOKEN ]]; then
    echo -e "${REDBI}Must define CODACY_PROJECT_TOKEN${NC}"
    exit 1
fi

if [[ -z $CODACY_API_TOKEN ]]; then
    echo -e "${REDBI}Must define CODACY_API_TOKEN${NC}"
    exit 1
fi

curl -Ls https://coverage.codacy.com/get.sh > get.sh
chmod +x ./get.sh

# Languages
#   https://github.com/codacy/codacy-plugins-api/blob/70df68d483a1241aa6257292c4f6c90a1ece5122/src/main/scala/com/codacy/plugins/api/languages/Language.scala#L43
#   CSharp
#   Python
#   Go
#   Shell
#   SASS
#   CSS
#   Dockerfile
#   Javascript
#   TypeScript
#   SQL
./get.sh report -l CSharp -r "$RL_DOCKER_VOLS_ROOT/out/coverage/csharp/Cobertura.xml"
find "$RL_DOCKER_VOLS_ROOT/out/coverage/spa/" -name "lcov.info" -not -empty -exec ./get.sh report -l TypeScript -r {} --partial \;

# send final notification to codacy.. looks like CodeCoverageReporter (get.sh) doesn't support the API Token, but its API does...
# https://github.com/codacy/codacy-coverage-reporter/issues/254
# ./get.sh final
curl -X POST "https://api.codacy.com/2.0/GSFSGroup/Zinc.DeveloperCenter/commit/$CIRCLE_SHA1/coverageFinal" \
  -H "Accept: application/json" \
  -H "api-token: $CODACY_API_TOKEN"
