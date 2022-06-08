#!/bin/bash
set -euo pipefail

# This is the entrypoint script for csharp unit and integration tests; currently referred by the Dockerfile test image target

ARTIFACTS_DIR=/artifacts
COVERAGE_DIR=/coverage/csharp
DEPENDENCY_TRY_LIMIT=${DEPENDENCY_TRY_LIMIT:-10}
DEPENDENCY_TIME_PER_TRY=${DEPENDENCY_TIME_PER_TRY:-5}

TMP_COVERAGE=$(mktemp -d -t ci-$(date +%Y-%m-%d-%H-%M-%S)-XXXXXXXXXX)
echo "TMP_COVERAGE: $TMP_COVERAGE"

# https://stackoverflow.com/questions/5947742/how-to-change-the-output-color-of-echo-in-linux
# https://github.com/ryanoasis/public-bash-scripts/blob/master/unix-color-codes-not-escaped.sh
GREEN='\033[0;32m'
GREENL='\033[1;32m'
YELLOWL='\033[1;33m'
REDBI='\033[1;91m'
BLUE='\033[0;36m'
BLUEL='\033[1;36m'
NC='\033[0m'

format_test_results() {
    # go to the results directory
    cd test-results

    echo -e "\nConvert xml-based .trx files to json..."
    for TEST_RESULT in $(ls *.trx); do xml2json < $TEST_RESULT > $TEST_RESULT.json; done

    echo -e "\nCopying results to the build host..."
    if [[ -d $ARTIFACTS_DIR ]]
    then
        cp -r . $ARTIFACTS_DIR
    else
        echo -e "${REDBI}$ARTIFACTS_DIR mount does not exist... Not copying test results...${NC}"
    fi

    cd ..
}

exec_tests() {
    TESTS_TO_EXECUTE=$1
    if [[ -z $TESTS_TO_EXECUTE  ]]; then
        echo -e "${REDBI}No tests to run...${NC}"
        return 0;
    fi

    echo -e "\n${BLUE}List of tests:${BLUEL}\n$TESTS_TO_EXECUTE${NC}\n"
    # run them all piping the output to a directory;
    #  +e will go back to default shell
    set +e
    for TESTPRJ in $TESTS_TO_EXECUTE
    do
        pushd $(dirname $TESTPRJ)

        FILE_NAME=$(basename $TESTPRJ)
        coverlet $FILE_NAME --format cobertura \
            -o "$TMP_COVERAGE/${FILE_NAME}.coverage.xml" \
            --target "dotnet" \
            --targetargs "vstest --logger:trx --ResultsDirectory:/publish-tests/test-results $FILE_NAME" &
        popd
    done
    set -e
}

assert_test_results() {
    # if any of the test runs failed, then fail the build
    cd test-results
    TEST_RUNS_OUTCOMES=$(cat *.json | jq .TestRun.ResultSummary.outcome)
    if [[ $TEST_RUNS_OUTCOMES == *"Failed"* ]]; then
        echo "Failure!"
        exit 1
    fi
    echo "Success!"
    cd ..
}

wait_for_dependency() {
    DEPENDENCY="${1:-}"
    if [[ -n ${DEPENDENCY:-} ]]; then
        totalTime=$((DEPENDENCY_TIME_PER_TRY * DEPENDENCY_TRY_LIMIT))
        tries=0

        echo -e "\nWaiting for Web dependencies for integration tests: $baseUrl"
        until $(curl -X GET -k --output /dev/null --silent --head --fail $baseUrl); do
            if [ ${tries} -ge ${DEPENDENCY_TRY_LIMIT} ]
            then
                echo "$baseUrl failed to start after ${totalTime} seconds"
                echo "call result: "
                $(curl -v $baseUrl);
                exit 1
            fi
            tries=$(($tries + 1 ))
            echo "Waiting for dependency: $baseUrl (attempt #$tries)"
            sleep ${DEPENDENCY_TIME_PER_TRY};
        done
    else
        echo -e "${REDBI}No dependencies...${NC}"
    fi
}

# get a list of all test files; don't fail if no files are returned
set +e
ls -laR .
UNIT_TEST_FILES=$(find ./ -name "*.UnitTests.dll")
INTEGRATION_TEST_FILES=$(find ./ -name "*.IntegrationTests.*.dll")
FUNCTIONAL_TEST_FILES=$(find ./ -name "*.FunctionalTests.dll")
set -e

echo -e "\n*** Inspect the environment ***"
    echo "Current Dir: $(pwd)"
    # ls -laR .

    # let's see what volumes are built
    echo "Root (/) Listing:"
    ls -la /

echo -e "\n*** Running charp unit tests ***"
    exec_tests "$UNIT_TEST_FILES"

echo -e "\n*** Waiting for dependencies ***"
    wait_for_dependency ${baseUrl:-}
    echo "Dependencies ready."

echo -e "\n*** Running csharp integration tests ***"
    exec_tests "$INTEGRATION_TEST_FILES"

echo -e "\n*** Running csharp functional tests ***"
    exec_tests "$FUNCTIONAL_TEST_FILES"
    wait
    format_test_results

echo -e "\n*** Generating code coverage ***"
    mkdir -p /coverage/csharp
    reportgenerator "-reports:$TMP_COVERAGE/*.coverage.xml" "-targetdir:/coverage/csharp" "-reporttypes:Cobertura;Html;HtmlSummary" "-verbosity:Error"

echo -e "\n*** Process test results ***"
    assert_test_results
