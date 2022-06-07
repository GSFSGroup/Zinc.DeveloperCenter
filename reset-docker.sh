#!/bin/bash

# This script is a big hammer to reset your docker experience in a number of scenarios:
#   1. clean up a bunch of docker images
#   2. fix an error with docker volumes on docker-compose up
#   3. use latest images from server (by default, `docker-compose up` will use the requested image locally first)
#   4. cleanup docker-volume output filled up from a lot of test runs
#   5. prune the entire docker build cache
#
# usage: source ./reset-docker.sh

# https://stackoverflow.com/questions/5947742/how-to-change-the-output-color-of-echo-in-linux
# https://github.com/ryanoasis/public-bash-scripts/blob/master/unix-color-codes-not-escaped.sh
YELLOWL='\033[1;33m'
NC='\033[0m'

echo "Shutting down any remaining running docker containers...."
docker rm --force $(docker ps -aq)

echo "Pruning the entire docker build cache"
docker builder prune --all --force

echo "Removing any and all docker images/layers..."
docker rmi --force $(docker image ls -aq)

echo "Removing any and all docker volumes..."
docker volume rm --force $(docker volume ls -q)

if [[ -z $RL_DOCKER_VOLS_ROOT ]]; then
    echo -e "${YELLOWL}RL_DOCKER_VOLS_ROOT environment variable not set; not deleting files...${NC}"
else
    RL_DOCKER_VOLS_OUT="${RL_DOCKER_VOLS_ROOT}/out"

    echo -e "\nDeleting docker-volumes output files ($RL_DOCKER_VOLS_OUT)\n"
    FILES_FOUND=$(find $RL_DOCKER_VOLS_OUT \( -name \*.txt -o -name \*.trx -o -name \*.json \) -type f)

    echo -e "${FILES_FOUND}\n" | sed 's/^/  /'
    find $RL_DOCKER_VOLS_OUT \( -name \*.txt -o -name \*.trx -o -name \*.json \) -type f -delete
fi

#echo "Clearing environment variables..."
#source .cicd/build/load-env.sh default.clear
