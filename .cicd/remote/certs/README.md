# Cluster Configuration Scripts

## make-cluster-ca-cert-bundle.sh

* Frequency: Once per namespace
* Purpose:

    Generate a CA cert bundle containing the cluster CA public key using our release image bundle as a  base. If the deployment overrides its container's bundle file, the container will trust other containers secured by certs generated from the cluster CA. This configMap can be shared between all applications in the namespace.

* Usage:

    This script must be run by an administrator. Since it is not run often and it is specific to this cluster, callers can set environment variables to customize some of the naming conventions.

    *note: be sure to have kubectl configured and a current-cluster configured*

* Environment Variables:
  * K8S_NAMESPACE
    * default value: **default**
  * RELEASE_DOCKER_IMAGE
    * default value: **microsoft/dotnet:2.2-aspnetcore-runtime-alpine**
    * base image whose:
      * CA bundle is modified to include the cluster's CA
      * CA bundle is then exported to be used to override our compiled release image's CA bundle

* Successful Output
  * cluster objects
    * a configmap containing
      * public CA certificate for the cluster
      * public CA bundle for the cluster that includes the certificate
      * `kubectl get configmap`

## make-app-certs.sh

* Frequency: Once per installation in a cluster's namespace
* Purpose: Request certificate from the cluster CA (Certificate Authority) for use as TLS and token signing
* Type of certificate requested:
  * TLS
    * digital signature
    * key encipherment
    * server auth
  * Token Signing
    * cert sign

* Usage:

    This script must be run by an administrator. Since it is not run often and it is specific to this project, callers can set environment variables to customize some of the naming conventions.

    *note: be sure to have kubectl configured and a current-cluster configured*

* Environment Variables:
  * RL_APP_NAME
    * default value: **h-datacatalog**
  * K8S_NAMESPACE
    * default value: **default**
  * SERVICE_NAME
    * default value: **h-datacatalog-svc**
  * CERT_X509_PASSWORD
    * default value: **password**
  * CERT_FILE_NAME
    * default value: **aspnetapp**

* Successful Output:
  * local files
    | Name | Purpose |
    | --- | --- |
    | *.csr | a request file that has been submitted to the cluster CA |
    | *-key.pem | private key (generated along with the request) |
    | *.crt | public key (downloaded from the CA after the request is approved) |
    | *.pfx | public/private key pair (to secure the aspnet host) |

  * cluster objects
    * an approved csr
      * `kubectl get csr`
    * a configmap containing the public certificate
      * `kubectl get configmap`
    * a secret containing the *.pfx file and a password
      * `kubectl get secret`

* TBD:
  * Certificate Rollover
  * Ensure certificate types
