# Conventions:
#   * Projects with names matching the pattern Zinc.DeveloperCenter.Host.* are host projects. They will be published into their own folders.
#   * Project with name Zinc.DeveloperCenter.Client.Web is the SPA project. It gets built using npm run build:cicd.
#     Result is then copied to Zinc.DeveloperCenter.Host/wwwroot folder.

ARG COMPILE_CONFIG=Release
ARG DEBUG_TYPE=pdbonly
ARG NPMRC_CONTENTS
ARG BUILD_INFO_JSON

FROM quay.io/gsfsgroup/hg-images:redline-build-node-16 as build-spa
    # USER root
    # RUN mkdir -p /build-spa && chown --recursive node:node /build-spa

    USER node
    # ***** Start client app build layer *****
    # this goes first, because it is the longest and we dont want to trigger this with changes in other parts of the code.
    COPY --chown=node:node ./src/App/Zinc.DeveloperCenter.Client.Web/package.json /build-spa/package.json
    COPY --chown=node:node ./src/App/Zinc.DeveloperCenter.Client.Web/package-lock.json /build-spa/package-lock.json
    WORKDIR /build-spa

    # install dependencies whilst setting the authkey for a private repo and prevent baking the authkey into a Docker layer/image
    # https://www.alexandraulsh.com/2018/06/25/docker-npmrc-security/
    ARG NPMRC_CONTENTS
    RUN echo ${NPMRC_CONTENTS} > .npmrc && \
        npm ci && \
        rm -f .npmrc

    # build the SPA.
    COPY --chown=node:node ./src/App/Zinc.DeveloperCenter.Client.Web ./
    RUN npm run build:cicd && \
        find ./ -name node_modules -prune -o -type 'f'
    # ***** End client app build layer *****

FROM quay.io/gsfsgroup/hg-images:redline-build-dotnet-5.0 as build

    # let's setup our working directories (otherwise "magic" WORKDIR will create them all as root)
    #   note: /build is listed twice: /build -> chown+ls; /build/$SPA_APP -> mkdir
    USER root

    SHELL ["/bin/bash", "-c"]
    RUN declare -a WORKDIRS=("/build/src" "/publish" "/publish-tests") && \
    echo "Creating working directories: ${WORKDIRS[*]/#/-}..." && \
    for d in "${WORKDIRS[@]}"; do  mkdir -p "$d"; done && \
    for d in "${WORKDIRS[@]}"; do chown --recursive node:node "$d"; done

    USER node

    # ***** Start csharp dependency layer *****
    WORKDIR /build
    # If you alter the project structure significantly from what the template generates, you can automatically
    # regenerate the `COPY` block of this section by running the `./print-csharp-dep-layer.sh` script.
    COPY --chown=node:node ./Directory.Build.props ./Directory.Build.props
    COPY --chown=node:node ./Zinc.DeveloperCenter.sln ./Zinc.DeveloperCenter.sln
    COPY --chown=node:node ./src/App/Zinc.DeveloperCenter.Application/Zinc.DeveloperCenter.Application.csproj ./src/App/Zinc.DeveloperCenter.Application/Zinc.DeveloperCenter.Application.csproj
    COPY --chown=node:node ./src/App/Zinc.DeveloperCenter.Data.Migrations/Zinc.DeveloperCenter.Data.Migrations.csproj ./src/App/Zinc.DeveloperCenter.Data.Migrations/Zinc.DeveloperCenter.Data.Migrations.csproj
    COPY --chown=node:node ./src/App/Zinc.DeveloperCenter.Data/Zinc.DeveloperCenter.Data.csproj ./src/App/Zinc.DeveloperCenter.Data/Zinc.DeveloperCenter.Data.csproj
    COPY --chown=node:node ./src/App/Zinc.DeveloperCenter.Domain/Zinc.DeveloperCenter.Domain.csproj ./src/App/Zinc.DeveloperCenter.Domain/Zinc.DeveloperCenter.Domain.csproj
    COPY --chown=node:node ./src/App/Zinc.DeveloperCenter.Host.Jobs/Zinc.DeveloperCenter.Host.Jobs.csproj ./src/App/Zinc.DeveloperCenter.Host.Jobs/Zinc.DeveloperCenter.Host.Jobs.csproj
    COPY --chown=node:node ./src/App/Zinc.DeveloperCenter.Host.Messaging/Zinc.DeveloperCenter.Host.Messaging.csproj ./src/App/Zinc.DeveloperCenter.Host.Messaging/Zinc.DeveloperCenter.Host.Messaging.csproj
    COPY --chown=node:node ./src/App/Zinc.DeveloperCenter.Host.Web/Zinc.DeveloperCenter.Host.Web.csproj ./src/App/Zinc.DeveloperCenter.Host.Web/Zinc.DeveloperCenter.Host.Web.csproj
    COPY --chown=node:node ./src/RedLine/RedLine.Application/RedLine.Application.csproj ./src/RedLine/RedLine.Application/RedLine.Application.csproj
    COPY --chown=node:node ./src/RedLine/RedLine.Data/RedLine.Data.csproj ./src/RedLine/RedLine.Data/RedLine.Data.csproj
    COPY --chown=node:node ./src/RedLine/RedLine.Domain/RedLine.Domain.csproj ./src/RedLine/RedLine.Domain/RedLine.Domain.csproj
    COPY --chown=node:node ./src/RedLine/RedLine.Extensions.Hosting.Jobs/RedLine.Extensions.Hosting.Jobs.csproj ./src/RedLine/RedLine.Extensions.Hosting.Jobs/RedLine.Extensions.Hosting.Jobs.csproj
    COPY --chown=node:node ./src/RedLine/RedLine.Extensions.Hosting.Messaging/RedLine.Extensions.Hosting.Messaging.csproj ./src/RedLine/RedLine.Extensions.Hosting.Messaging/RedLine.Extensions.Hosting.Messaging.csproj
    COPY --chown=node:node ./src/RedLine/RedLine.Extensions.Hosting.Web/RedLine.Extensions.Hosting.Web.csproj ./src/RedLine/RedLine.Extensions.Hosting.Web/RedLine.Extensions.Hosting.Web.csproj
    COPY --chown=node:node ./src/RedLine/RedLine.Extensions.Hosting/RedLine.Extensions.Hosting.csproj ./src/RedLine/RedLine.Extensions.Hosting/RedLine.Extensions.Hosting.csproj
    COPY --chown=node:node ./src/RedLine/RedLine.HealthChecks/RedLine.HealthChecks.csproj ./src/RedLine/RedLine.HealthChecks/RedLine.HealthChecks.csproj
    COPY --chown=node:node ./tests/App/Zinc.DeveloperCenter.FunctionalTests/Zinc.DeveloperCenter.FunctionalTests.csproj ./tests/App/Zinc.DeveloperCenter.FunctionalTests/Zinc.DeveloperCenter.FunctionalTests.csproj
    COPY --chown=node:node ./tests/App/Zinc.DeveloperCenter.IntegrationTests.Jobs/Zinc.DeveloperCenter.IntegrationTests.Jobs.csproj ./tests/App/Zinc.DeveloperCenter.IntegrationTests.Jobs/Zinc.DeveloperCenter.IntegrationTests.Jobs.csproj
    COPY --chown=node:node ./tests/App/Zinc.DeveloperCenter.IntegrationTests.Messaging/Zinc.DeveloperCenter.IntegrationTests.Messaging.csproj ./tests/App/Zinc.DeveloperCenter.IntegrationTests.Messaging/Zinc.DeveloperCenter.IntegrationTests.Messaging.csproj
    COPY --chown=node:node ./tests/App/Zinc.DeveloperCenter.IntegrationTests.Web/Zinc.DeveloperCenter.IntegrationTests.Web.csproj ./tests/App/Zinc.DeveloperCenter.IntegrationTests.Web/Zinc.DeveloperCenter.IntegrationTests.Web.csproj
    COPY --chown=node:node ./tests/App/Zinc.DeveloperCenter.UnitTests/Zinc.DeveloperCenter.UnitTests.csproj ./tests/App/Zinc.DeveloperCenter.UnitTests/Zinc.DeveloperCenter.UnitTests.csproj
    COPY --chown=node:node ./tests/Directory.Build.props ./tests/Directory.Build.props
    COPY --chown=node:node ./tests/RedLine/RedLine.UnitTests/RedLine.UnitTests.csproj ./tests/RedLine/RedLine.UnitTests/RedLine.UnitTests.csproj

    RUN find ./ -name node_modules -prune -o -type 'f' && \
        dotnet restore
    # ***** End csharp dependency layer *****

    # **** Start csharp cache layer
    COPY --chown=node:node . /build
    WORKDIR /build

    # Publish the only the artifacts we need for production (i.e., Host projects)
    ARG COMPILE_CONFIG
    ARG DEBUG_TYPE
    RUN dotnet build --no-restore -c "$COMPILE_CONFIG" /p:DebugType="$DEBUG_TYPE" && \
        for SRCDIR in src/App/Zinc.DeveloperCenter.Host.*; do \
            PROJECT=$(basename "${SRCDIR}") && \
            dotnet publish -v d --no-build -c "$COMPILE_CONFIG" -o "/publish/${PROJECT}" "/build/${SRCDIR}"/; \
            done && \
        dotnet publish --no-build -c "$COMPILE_CONFIG" -o /publish/Zinc.DeveloperCenter.Data.Migrations /build/src/App/Zinc.DeveloperCenter.Data.Migrations/ && \
        find /publish -name node_modules -prune -o -type 'f'

    # **** End csharp cache layer

# Build the runtime image that will get deployed...
FROM quay.io/gsfsgroup/hg-images:redline-release-aspnet-5.0-debian as release

    # This section enables debugging from inside container.
    # SHELL ["/bin/ash", "-o", "pipefail", "-c"]
    ARG COMPILE_CONFIG
    RUN if [ "$COMPILE_CONFIG" = "debug" ]; then \
            apt-get install -y curl=7.74.0-1.3+deb11u1 procps=2:3.3.17-5; \
            curl -sSL https://aka.ms/getvsdbgsh | sh /dev/stdin -v latest -l /vsdbg; \
        fi

    # Create a non-root user to run the application...
    # The gid and uid match the circleci user's on the machine executor in order to
    #   be able to write log files to a volume on the cicd build server (if desired)
    RUN addgroup --system --gid 1002 appuser && \
        adduser --system --gid 1002 appuser -uid 1001 -uid 1001 --disabled-password && \
        mkdir -p /app && \
        chown --recursive appuser:appuser /app && \
        find /app -name node_modules -prune -o -type 'f'

    USER appuser

    # Set default env vars. See also: https://docs.docker.com/develop/develop-images/dockerfile_best-practices/#env
    ENV RL_APP_NAME zn-developercenter
    ENV ASPNETCORE_ENVIRONMENT "prod"
    ENV ASPNETCORE_URLS https://+:5001

    # Copy the build output and launch the app...
    WORKDIR /app
        COPY --from=build --chown=appuser:appuser /publish .
        COPY --from=build --chown=appuser:appuser /build/.cicd/build/entrypoints/run_app.sh .
        COPY --from=build-spa --chown=appuser:appuser /build-spa/dist /app/Zinc.DeveloperCenter.Host.Web/wwwroot/dist
        RUN echo "${BUILD_INFO_JSON}" | tee /app/Zinc.DeveloperCenter.Host.Web/wwwroot/buildinfo.json; \
            echo "*** ON RELEASE IMAGE ***"; \
            find ./ -name node_modules -prune -o -type 'f'
        ENTRYPOINT ["/bin/sh", "run_app.sh"]

FROM build as tests-csharp
    # Publish all the artifacts (including test assets)
    WORKDIR /build

    ARG COMPILE_CONFIG
    RUN find tests -name "*.csproj" -exec dotnet publish --no-build -c "$COMPILE_CONFIG" -o "/publish-tests/$(basename {} .csproj)" {} \;

    USER node
    WORKDIR /publish-tests
    ENTRYPOINT ["/bin/bash", "/build/.cicd/build/entrypoints/run_tests_csharp.sh"]

FROM build-spa as tests-spa-unit
    WORKDIR /build-spa
    ENV SPA_UNIT_COVERAGE_PATH /build-spa/coverage
    COPY .cicd/build/entrypoints/run_tests_spa_unit.sh /build-spa
    CMD ["/bin/bash", "./run_tests_spa_unit.sh"]
