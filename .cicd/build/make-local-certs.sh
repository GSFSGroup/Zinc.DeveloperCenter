#!/bin/bash
set -o pipefail

# Orchestrates:
# 1. cert creation
# 2. making an alpine-based certificate bundle in order to inject them into alpine-based containers that will then trust our custom certs
# 3. places certs in $RL_DOCKER_VOLS_ROOT/in/certs
# 4. places alpine certificate bundle in $RL_DOCKER_VOLS_ROOT/in/ca-certs/alpine-3.10.3

# optional overrides
# CERT_PASSWORD: this script uses `password` by default for the x509 cert pairs
# ALPINE_LABEL: this script uses 3.10.3 by default

ALPINE_LABEL=${ALPINE_LABEL:-"3.10.3"};

# https://stackoverflow.com/questions/5947742/how-to-change-the-output-color-of-echo-in-linux
# https://github.com/ryanoasis/public-bash-scripts/blob/master/unix-color-codes-not-escaped.sh
REDBI='\033[1;91m'
NC='\033[0m'

if [[ -z $RL_DOCKER_VOLS_ROOT ]]; then
    echo -e "${REDBI}Must define RL_DOCKER_VOLS_ROOT${NC}"
    exit 1
fi

RL_DOCKER_VOLS_IN=${RL_DOCKER_VOLS_ROOT}/in

CERT_DIR="${RL_DOCKER_VOLS_IN}/certs"
source .cicd/build/make-certs.sh -o $CERT_DIR -n redline -p ${CERT_PASSWORD:-password}
CERT_SCRIPT_RESULT=$?
if [[ $CERT_SCRIPT_RESULT > 0 ]]; then
    echo -e "${REDBI}Cert creation script failed...${NC}";
    exit 1
fi

CERT_CA_DIR="${RL_DOCKER_VOLS_IN}/ca-certs"
docker run -t -e TZ --volume ${CERT_DIR}/redline-public.crt:/usr/local/share/ca-certificates/redline-public.crt --volume ${CERT_CA_DIR}/alpine-${ALPINE_LABEL}:/ca-certs-out alpine:${ALPINE_LABEL} /bin/sh -c "apk add ca-certificates; update-ca-certificates; cp /etc/ssl/certs/ca-certificates.crt /ca-certs-out/ca-certificates.crt"

echo -e "\nCert directory listings:"
ls -alR $CERT_DIR $CERT_CA_DIR
