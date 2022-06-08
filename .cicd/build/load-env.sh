#! /bin/bash
# prime our tags
export CIRCLE_BRANCH=${CIRCLE_BRANCH:-$(git branch | grep \* | cut -d ' ' -f2)}
export SAFE_BRANCH=${CIRCLE_BRANCH//\//_}
RL_SHA_SHORT="$(git log -1 --pretty=%h)"
export RL_SHA_SHORT
export IS_PR_BUILD=false
export IS_MASTER_BUILD=false
export IS_BRANCH_BUILD=false

if [[ -n "$CIRCLE_PULL_REQUEST" ]]; then
  export RL_APP_TAG="${RL_SHA_SHORT}-${SAFE_BRANCH}-pr"
  export RL_APP_TAG_LATEST="latest-${SAFE_BRANCH}-pr"
  export IS_PR_BUILD=true
elif [[ "$CIRCLE_BRANCH" == "master" ]] || [[ "$CIRCLE_BRANCH" == "main" ]]; then
  export RL_APP_TAG="${RL_SHA_SHORT}"
  export RL_APP_TAG_LATEST="latest"
  export IS_MASTER_BUILD=true
else
  export RL_APP_TAG="${RL_SHA_SHORT}-${SAFE_BRANCH}"
  export RL_APP_TAG_LATEST="latest-${SAFE_BRANCH}"
  export IS_BRANCH_BUILD=true
fi

export RL_APP_TEST_CS_TAG="${RL_APP_TAG}-test-cs"
export RL_APP_TEST_SPA_UNIT_TAG="${RL_APP_TAG}-test-spa-unit"
export RL_APP_TEST_SPA_INTEGRATION_TAG="${RL_APP_TAG}-test-spa-integration"

export RL_IMAGE_BASE=${RL_DOCKER_REPO_BASE}/${RL_APP_NAME}
export RL_IMAGE="${RL_IMAGE_BASE}:$RL_APP_TAG"
export RL_IMAGE_LATEST="${RL_IMAGE_BASE}:${RL_APP_TAG_LATEST}"
export RL_IMAGE_TEST_CS="${RL_IMAGE_BASE}:${RL_APP_TEST_CS_TAG}"
export RL_IMAGE_TEST_SPA_UNIT="${RL_IMAGE_BASE}:${RL_APP_TEST_CS_TAG}"
export RL_IMAGE_TEST_SPA_INTEGRATION="${RL_IMAGE_BASE}:${RL_APP_TEST_SPA_INTEGRATION_TAG}"
export RL_DOCKER_HOST=host.docker.internal
export RL_APP_CONTEXT=docker-local
export ASPNETCORE_ENVIRONMENT=docker-circleci
export DOTNET_ENVIRONMENT=docker-circleci
