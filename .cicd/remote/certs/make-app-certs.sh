#!/bin/bash
set -euo pipefail

# NOTE: For multi-host certs (e.g. with web, jobs, and messaging hosts), run the following command to generate certs for all three
#   RL_HOST_NAME=zn-developercenter .cicd/remote/certs/make-app-certs.sh && RL_HOST_NAME=zn-developercenter-messaging .cicd/remote/certs/make-app-certs.sh && RL_HOST_NAME=zn-developercenter-jobs .cicd/remote/certs/make-app-certs.sh

# Generate certs on the currently configured kubernetes cluster by
# 1. Requesting a cert from the Cluster CA
# 2. Approving that cert request
# 3. Creating a secret containing the public/private pair (pfx) and its associated password for use by this applicatoin
# 4. Creating a configMap containing the public key for use by other applications

export RL_APP_NAME=${RL_APP_NAME:-zn-developercenter} && echo "RL_APP_NAME: ${RL_APP_NAME}"
export RL_HOST_NAME=${RL_HOST_NAME:-zn-developercenter} && echo "RL_HOST_NAME: ${RL_HOST_NAME}"
export K8S_NAMESPACE=${K8S_NAMESPACE:-default} && echo "K8S_NAMESPACE: ${K8S_NAMESPACE}"
export CERT_X509_PASSWORD=${CERT_X509_PASSWORD:-password}

# NOTE: The below should be considered a template and recommend no customization after this point.
# If you need to change the code, please consider making a PR to https://github.com/GSFSGroup/Zinc.DeveloperSetup

# https://stackoverflow.com/questions/5947742/how-to-change-the-output-color-of-echo-in-linux
# https://github.com/ryanoasis/public-bash-scripts/blob/master/unix-color-codes-not-escaped.sh
GREENL='\033[1;32m'
BLUEL='\033[1;36m'
NC='\033[0m'

export CURRENT_CLUSTER=$(kubectl config current-context) && echo "CURRENT_CLUSTER: ${CURRENT_CLUSTER}"
export TEMPDIR=$(date +"${RL_APP_NAME}-certs-${CURRENT_CLUSTER}_%Y-%m-%d_%H-%M-%S")
mkdir $TEMPDIR && cd $TEMPDIR && echo "TEMPDIR: ${TEMPDIR}"

# absolute naming: thing.namespace.thingtype.cluster.local
# relative naming: thing.namespace
export SERVICE_NAME=${SERVICE_NAME:-${RL_HOST_NAME}-svc} && echo "SERVICE_NAME: ${SERVICE_NAME}"
export CERT_FILE_NAME=${CERT_FILE_NAME:-redline} && echo "CERT_FILE_NAME: ${CERT_FILE_NAME}"
export RELEASE_DOCKER_IMAGE=${RELEASE_DOCKER_IMAGE:-"quay.io/gsfsgroup/hg-images:redline-release-aspnet-5.0"} && echo "RELEASE_DOCKER_IMAGE: ${RELEASE_DOCKER_IMAGE}"

export CERT_FILE_NAME_REQUEST=${CERT_FILE_NAME}.csr && echo "CERT_FILE_NAME_REQUEST: ${CERT_FILE_NAME_REQUEST}"
export CERT_FILE_NAME_PRIVATE_KEY=${CERT_FILE_NAME}-private.pem && echo "CERT_FILE_NAME_PRIVATE_KEY: ${CERT_FILE_NAME_PRIVATE_KEY}"
export CERT_FILE_NAME_PUBLIC_KEY=${CERT_FILE_NAME}-public.crt && echo "CERT_FILE_NAME_PUBLIC_KEY: ${CERT_FILE_NAME_PUBLIC_KEY}"
export CERT_FILE_NAME_X509=${CERT_FILE_NAME}.pfx && echo "CERT_FILE_NAME_X509: ${CERT_FILE_NAME_X509}"
export SERVICE_DNS_NAME=${SERVICE_DNS_NAME:-${SERVICE_NAME}.${K8S_NAMESPACE}} && echo "SERVICE_DNS_NAME: ${SERVICE_DNS_NAME}"

echo -e "\nCurrent cluster information..."
kubectl cluster-info

echo -e "\nTesting for existing CSRs for the same service ${SERVICE_DNS_NAME}..."
HOW_MANY_CSRS_FOR_SVC=$(kubectl get csr ${SERVICE_DNS_NAME} -o name --ignore-not-found | wc -l)
if [[ $HOW_MANY_CSRS_FOR_SVC -gt 0 ]]; then
    echo "There is already a request for ${SERVICE_DNS_NAME} on this cluster."
    echo "    Please point to a server that does not contain a request for the service or..."
    echo "    Execute 'kubectl delete csr ${SERVICE_DNS_NAME}'"
    exit 1;
fi

# make the request file
echo -e "\n${BLUEL}Generating a certificate request for $SERVICE_DNS_NAME...${NC}"
cat <<EOF | cfssl genkey - | cfssljson -bare ${CERT_FILE_NAME}
{
  "hosts": [
    "${SERVICE_DNS_NAME}.svc.cluster.local"
  ],
  "CN": "${SERVICE_DNS_NAME}.svc.cluster.local",
  "key": {
    "algo": "rsa",
    "size": 4096
  }
}
EOF
mv "${CERT_FILE_NAME}-key.pem" "${CERT_FILE_NAME_PRIVATE_KEY}"

# submit it to the cluster ca
#   usages:
#       ssl
#         - digital signature
#         - key encipherment
#         - server auth
#       token signing
#         - cert sign
cat <<EOF | kubectl apply -n ${K8S_NAMESPACE} -f -
apiVersion: certificates.k8s.io/v1beta1
kind: CertificateSigningRequest
metadata:
  name: ${SERVICE_DNS_NAME}
  labels:
    app: ${RL_APP_NAME}
spec:
  groups:
  - system:authenticated
  request: $(cat ${CERT_FILE_NAME_REQUEST} | base64 | tr -d '\n')
  usages:
  - digital signature
  - key encipherment
  - server auth
  - cert sign
EOF

echo -e "\n${BLUEL}Approving the certificate request for ${SERVICE_DNS_NAME}${NC}..."
kubectl certificate approve $SERVICE_DNS_NAME -n ${K8S_NAMESPACE}

echo -e "\n${BLUEL}Getting the public key from the cluster CA...${NC}"
kubectl get csr $SERVICE_DNS_NAME -o jsonpath='{.status.certificate}' -n ${K8S_NAMESPACE} \
    | base64 --decode > ${CERT_FILE_NAME_PUBLIC_KEY}

echo -e "\n${BLUEL}Exporting an x509 cert containing public and private keys...${NC}"
openssl pkcs12 -export -out ${CERT_FILE_NAME_X509} -inkey ${CERT_FILE_NAME_PRIVATE_KEY} -in ${CERT_FILE_NAME_PUBLIC_KEY} -name "Cluster cert for ${SERVICE_DNS_NAME}" -passout env:CERT_X509_PASSWORD

# public key as config
echo -e "\n${BLUEL}Create a configmap (ns: ${K8S_NAMESPACE}) intended to be shared containing the public key...${NC}"
mkdir -p public && \
    cp ${CERT_FILE_NAME_PUBLIC_KEY} public && \
    export CM_NAME=${SERVICE_NAME}-cert-public
    kubectl create configmap ${CM_NAME} --from-file=public -n ${K8S_NAMESPACE} && \
    kubectl label configmap ${CM_NAME} -n ${K8S_NAMESPACE} app=${RL_APP_NAME}

# private key as secret
#   note RE echo: https://www.funkypenguin.co.nz/beware-the-hidden-newlines-in-kubernetes-secrets/
echo -e "\n${BLUEL}Create a secret (ns: ${K8S_NAMESPACE}) containing the public/private key pair protected by password...${NC}"
mkdir -p private && \
    cp ${CERT_FILE_NAME_X509} private && \
    echo -n ${CERT_X509_PASSWORD} > private/password && \
    export SECRET_NAME=${SERVICE_NAME}-cert-private
    kubectl create secret generic ${SECRET_NAME} --from-file=private -n ${K8S_NAMESPACE} && \
    kubectl label secret ${SECRET_NAME} -n ${K8S_NAMESPACE} app=${RL_APP_NAME}

echo -e "\n${GREENL}All resources labeled w/ \"app=${RL_APP_NAME}\"...${NC}\n    Example usage: \"kubectl get csr,cm,secret -lapp=${RL_APP_NAME} -o name\"\n"
echo "To delete all of the kubernetes resources created by this script, execute the following: "
echo -e "\n\n    kubectl delete csr ${SERVICE_DNS_NAME} -n ${K8S_NAMESPACE} && \\
        kubectl delete configmap ${CM_NAME} -n ${K8S_NAMESPACE} && \\
        kubectl delete secret ${SECRET_NAME} -n ${K8S_NAMESPACE}\n\n"
