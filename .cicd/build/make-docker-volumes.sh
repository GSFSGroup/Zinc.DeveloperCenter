#!/bin/bash
set -o pipefail

# One-time helper script that will create volumes and copy healthchecks

# https://stackoverflow.com/questions/5947742/how-to-change-the-output-color-of-echo-in-linux
# https://github.com/ryanoasis/public-bash-scripts/blob/master/unix-color-codes-not-escaped.sh
REDBI='\033[1;91m'
BLUEL='\033[1;36m'
NC='\033[0m'

if [[ -z $RL_DOCKER_VOLS_ROOT ]]; then
    echo -e "${REDBI}Must define RL_DOCKER_VOLS_ROOT${NC}"
    exit 1
fi

export VOLS_TO_CREATE=(
    ${RL_DOCKER_VOLS_ROOT}/in/certs
    ${RL_DOCKER_VOLS_ROOT}/in/ca-certs
    ${RL_DOCKER_VOLS_ROOT}/out/app
    ${RL_DOCKER_VOLS_ROOT}/out/coverage/csharp
    ${RL_DOCKER_VOLS_ROOT}/out/coverage/spa
    ${RL_DOCKER_VOLS_ROOT}/out/csharp
    ${RL_DOCKER_VOLS_ROOT}/out/db
    ${RL_DOCKER_VOLS_ROOT}/out/rabbitmq
    ${RL_DOCKER_VOLS_ROOT}/out/spa
)
#echo "VOLS_TO_CREATE: $VOLS_TO_CREATE"
#echo ${VOLS_TO_CREATE[*]}

echo -e "\n${BLUEL}Making docker volumes...${NC}"
for VOLUME in ${VOLS_TO_CREATE[@]};
do
    echo "VOLUME -> ${VOLUME}"
    mkdir -p -v ${VOLUME};
    # Because containers might be running with a different user account, we should allow them to read and write to these folders.
    chmod a+rw ${VOLUME}
done
