#!/bin/bash
set -eo pipefail

# Runs openssl commands to generate x509 certs in order to enable
# 1. run webservers using https locally
# 2. sign tokens for authn
#
# Running this on OSX results in the certs being automatically trusted
# todos:
#   1. auto-trust on Windows
#   2. auto-trust on Linux

USE_DOTNET_DEVCERTS=false

# reset so the script can be run again
OPTIND=1

usage() { echo "Usage: $0 [-o ~/docker-volumes/in/certs] [-n <string>] [-p <string>]" 1>&2; exit 1; }
while getopts ":o:n:p:" opt; do
    case $opt in
        o)
            export CERTS_DIRECTORY=${OPTARG}
            echo "Certs directory: $CERTS_DIRECTORY"
            ;;
        n)
            export CERT_NAME=${OPTARG}
            echo "Cert name: $CERT_NAME"
            ;;
        p)
            export PASSWORD=${OPTARG}
            echo "Password entered..."
            ;;
        \?)
            echo "Invalid option: -$OPTARG" >&2
            ;;
        :)
            echo "Option -$OPTARG requires an argument." >&2
            ;;
        *)
            usage
            ;;
    esac
done

if [ -z "${CERTS_DIRECTORY}" ] || [ -z "${CERT_NAME}" ] || [ -z "${PASSWORD}" ]; then
    usage
    exit 1;
fi

PFX_FILE="${CERT_NAME}.pfx"
PFX_PATH="${CERTS_DIRECTORY}/${PFX_FILE}"
echo "PFX_PATH: $PFX_PATH"

ORIGINAL_DIR=$(pwd)
cd $CERTS_DIRECTORY
    if $USE_DOTNET_DEVCERTS; then
        dotnet dev-certs https -ep $PFX_FILE -p $PASSWORD

        echo "OSTYPE!!: $OSTYPE"
        if [[ "$OSTYPE" != "linux-gnu" ]]; then
            echo "Running Windows/OSX trust command..."
            dotnet dev-certs https --trust
        fi

        openssl pkcs12 -in "$CERT_NAME.pfx" -nokeys -out "$CERT_NAME-public.crt" -passin env:PASSWORD
        openssl pkcs12 -in "$CERT_NAME.pfx" -nocerts -out "$CERT_NAME-private.key" -nodes -passin env:PASSWORD
    else
        CERT_SUBJECT=localhost # ${CERT_NAME}
        echo "CERT_SUBJECT: $CERT_SUBJECT"
        openssl req -x509 -newkey rsa:4096 -keyout "${CERT_NAME}-private.key" -out "${CERT_NAME}-public.crt" -days 365 -nodes -subj "/CN=${CERT_SUBJECT}" -config "${ORIGINAL_DIR}/.cicd/build/ssl-selfsigned.cnf"
        openssl pkcs12 -export -out "${CERT_NAME}.pfx" -inkey "${CERT_NAME}-private.key" -in "${CERT_NAME}-public.crt" -name "Local dev cert for ${CERT_NAME}" -passout env:PASSWORD

        #echo "OSTYPE: $OSTYPE"
        #echo "OS: ${OS:-Not Set}"
        if [[ "$OSTYPE" =~ ^darwin ]]; then
            echo "Running OSX trust command..."
            sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain "${CERT_NAME}-public.crt"
        else
            echo "Need to find the native trust command for your OS: ${OSTYPE:-Not Set}"
        fi
    fi
cd $ORIGINAL_DIR
