#!/bin/bash
set -euo pipefail

# Generate an alpine-based cert bundle on the currently configured kubernetes cluster by
# 1. Grabbing the cluster's CA cert from the default serviceaccount for the configured namespace (all service accounts on this cluster have a reference to the same CA public key)
# 2. Running a command on an alpine-based image to add this cluster's CA public key to the list of trusted CAs
# 3. Create a configMap with the certificate bundle so that deployments can override their docker image's certificate bundle
#
# This only needs to be run once per namespace; multiple apps will share this generic configMap (example name: hawking.dimebox.io-ca-bundle) since it contains the cluster's CA

export K8S_NAMESPACE=${K8S_NAMESPACE:-default} && echo "K8S_NAMESPACE: ${K8S_NAMESPACE}"
export RELEASE_DOCKER_IMAGE=${RELEASE_DOCKER_IMAGE:-"microsoft/dotnet:2.2-aspnetcore-runtime-alpine"} && echo "RELEASE_DOCKER_IMAGE: ${RELEASE_DOCKER_IMAGE}"

# NOTE: The below should be considered a template and recommend no customization after this point.
# If you need to change the code, please consider making a PR to https://github.com/GSFSGroup/Zinc.DeveloperSetup

# https://stackoverflow.com/questions/5947742/how-to-change-the-output-color-of-echo-in-linux
# https://github.com/ryanoasis/public-bash-scripts/blob/master/unix-color-codes-not-escaped.sh
GREENL='\033[1;32m'
BLUEL='\033[1;36m'
NC='\033[0m'

export CURRENT_CLUSTER=$(kubectl config current-context) && echo "CURRENT_CLUSTER: ${CURRENT_CLUSTER}"
export TEMPDIR=$(date +"ca-bundle-${CURRENT_CLUSTER}_%Y-%m-%d_%H-%M-%S")
mkdir $TEMPDIR && cd $TEMPDIR && echo "TEMPDIR: ${TEMPDIR}"

echo -e "\nCurrent cluster information..."
kubectl cluster-info

# let's create directories that we're going to turn into a configmap
mkdir -p public

# let's go get the k8s cluster cert
#   assuming default service account; but it doesn't matter all that much; all service accounts publish the same CA public crt
echo -e "\n${BLUEL}Generating a CA bundle from the release image (${RELEASE_DOCKER_IMAGE}) adding the current cluster's CA cert...${NC}"
export SERVICE_ACCOUNT_SECRET_NAME=$(kubectl get serviceaccount default -ojsonpath='{ .secrets[0].name }' -n $K8S_NAMESPACE) && echo "SERVICE_ACCOUNT_SECRET_NAME: ${SERVICE_ACCOUNT_SECRET_NAME}"
export CLUSTER_CA_FILE_NAME=$CURRENT_CLUSTER.ca.crt && echo "CLUSTER_CA_FILE_NAME: ${CLUSTER_CA_FILE_NAME}"
export CLUSTER_CA_ABSOLUTE_PATH="$(pwd)/${CLUSTER_CA_FILE_NAME}" && echo "CLUSTER_CA_ABSOLUTE_PATH: ${CLUSTER_CA_ABSOLUTE_PATH}"
kubectl get secret $SERVICE_ACCOUNT_SECRET_NAME -o "jsonpath={.data['ca\.crt']}" | base64 -D > $CLUSTER_CA_FILE_NAME
docker run -t \
    --volume ${CLUSTER_CA_ABSOLUTE_PATH}:/usr/local/share/ca-certificates/${CLUSTER_CA_FILE_NAME} \
    --volume $(pwd)/public:/ca-certs-out \
    ${RELEASE_DOCKER_IMAGE} \
    /bin/sh -c "apk add ca-certificates; update-ca-certificates; cp /etc/ssl/certs/ca-certificates.crt /ca-certs-out/ca-certificates.crt; ls -alR /usr/local/share/ca-certificates/ /ca-certs-out /etc/ssl/certs/ca-certificates.crt; "

# public key as config
echo -e "\n${BLUEL}Create a configmap (ns: ${K8S_NAMESPACE}) intended to be shared containing the ${CURRENT_CLUSTER} cluster CA public key...${NC}"
cp ${CLUSTER_CA_FILE_NAME} public && \
    export CM_NAME=${CURRENT_CLUSTER}-ca-bundle
    kubectl create configmap ${CM_NAME} --from-file=public -n ${K8S_NAMESPACE}
    kubectl label configmap ${CM_NAME} -n ${K8S_NAMESPACE} cluster=${CURRENT_CLUSTER}

echo -e "\n${GREENL}All resources labeled w/ \"cluster=${CURRENT_CLUSTER}\"...${NC}\n"
echo "To delete all of the kubernetes resources created by this script, execute the following: "
echo "    kubectl delete configmap ${CM_NAME} -n ${K8S_NAMESPACE}"
