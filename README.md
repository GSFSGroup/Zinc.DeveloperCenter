# Zinc.DeveloperCenter

Congratulations! You have created a new micro-app using the `redline-app` template. However, your initial setup is not complete yet. Follow the steps outlined in the next section to deploy the micro-app in preprod environment:

- [Running Zinc.Developer Center Locally](#github-api-access)
- [Understand the solution structure](#understand-structure)
- [Build and run app with docker-compose](#build-and-run-app)
- [GitHub repository setup and first pull request](#git-repo-setup)
- [Diagnosing CircleCI build errors](#build-errors)
- [Create app certs in preprod](#create-app-certs)
- [Create DB user and grant access](#create-db-user)
- [Push AWS Parameter Store configuration](#aws-parameter-store)
- [Creating deployment files in Mercury.PreProd repo](#mercury-preprod)
- [Configuring a shell to load microapp](#configure-shell)
- [Configuring Administration](#configure-admin)
- [Configuring AuthN for service-to-service authentication](#configure-authn)
- [Configure AuthZ](#configure-authz)
- [Ensure logs are accessible through DataDog](#ensure-datadog)
- [Schedule a job](#schedule-a-job)

## <a id="github-api-access"></a>Running Zinc.Developer Center Locally
**Developer Center requires use of a GitHub Access Token**

#### Setting Environment Variable:

The GitHub API requires the use of a GitHub access token to access private repositories. In pre-production, Developer Center will pull a token from AWS parameter store. This can be found in PreProd at /apps/zn-developercenter/remote/hawking/GitHubApi/AccessToken. When working locally, you must first set a local environment variable. This takes two steps:

*   First, grab a valid GitHub access token value. You can utilize the GitHub accesss token in the parameter store. Simple visit AWS PreProd, search for "zn-developercenter", and view the GitHubApi/AccessToken parameter. You can also use your own token. If you do not already have one, create a new GitHub access token. The only permissions that are necessary for this token are "repo" and "gist". Note that having extra permissions is not dangerous, as the only queries made from Developer Center are of Read type.

* Second, set an environment variable on your machine with this token value. Name the varibale "GSFSGROUP_GITHUB_API_TOKEN".

You should now be able to run Developer Center locally and see up-to-date GSFS group GitHub data.

#### Renewing the Token:

The token being used for Developer Center does not have an expiration date. Nevertheless, if a new token ever needs to be created, you need to take two steps:

*   Create a new GitHub access token. The permissions need to be "repo" and "gist".

*   Set the value for the Developer Center access token in AWS parameter store. The value is located at /apps/zn-developercenter/remote/hawking/GitHubApi/AccessToken. Alternately, add the GSFSGROUP_GITHUB_API_TOKEN environment varable as a k8s secret.
## <a id='understand-structure'></a>Understand solution structure

When the solution is created, you will notice that the name of some projects in the solution are prefixed with `Zinc.DeveloperCenter`. These projects contain the code that are specific to this microservice. Projects that start with the name `RedLine` are platform-specific and intended to be left alone. You can think of them as generated code that gets replaced. As we improve the functionality provided by the `RedLine` platform, we will replace the code in `RedLine` projects. So, if you add any changes to those projects, they will get overwritten.

>**IMPORTANT**: Always put your changes in the projects prefixed with  `Zinc.DeveloperCenter`. Never edit `RedLine` projects. `RedLine` projects should be edited only in [](https://github.com/GSFSGroup/Zinc.DeveloperCenter) repository.

[Click here to see more information on angular project structure](./src/App/Zinc.DeveloperCenter.Client.Web/README.md)

There are 3 host projects that are the main entry points:
* __Web__ serves the REST API and front-end component assets.
* __Messaging__ listens to RabbitMQ messages and handles events.
* __Jobs__ runs background jobs.

All code is built into the same container with each project having their own folder. The folder structure in the container looks like:
```
/app/
    run_app.sh
    Zinc.DeveloperCenter.Host.Web/
        Zinc.DeveloperCenter.Hosting.Web.dll
        ...
    Zinc.DeveloperCenter.Host.Messaging/
        Zinc.DeveloperCenter.Hosting.Messaging.dll
        ...
    Zinc.DeveloperCenter.Host.Jobs/
        Zinc.DeveloperCenter.Host.Jobs.dll
        ...
    Zinc.DeveloperCenter.Data.Migrations/
        Zinc.DeveloperCenter.Data.Migrations.dll
        ...
```

The entry point `run_app.sh` uses `APP_ENTRYPOINT` environment variable to identify which project to run. It drops into that folder and runs the dll with the same name.

## <a id='build-and-run-app'></a>Build and run app

Because we have code analysis rules that require the using statements to be ordered, before moving on to running the app with docker-compose, you might need to fix those issues for the build. You can build the whole solution to see if any issues exist and fix it in your editor. Alternatively, `dotnet build` at the root of the solution will show you the issues.

At this point you should be able to build and run the app in docker-compose. First, pull the latest images from the container registry:

```cmd
$> rl-docker-compose pull
```

If you see any authentication errors, you will need to login to quay image registry with `docker login quay.io`. For further help with login, see the ___CLI Password___ section in your account settings page at `https://quay.io/user/{USERNAME}?tab=settings` (replace `{USERNAME}` with your  quay.io user name).

Next, build and bring up the services:
```cmd
$> rl-docker-compose up -d --build app
```

If this command fails with an error similar to `ERROR: Service failed to build: unauthorized: access to the requested resource is not authorized`. This happens when docker-compose is not able to pull the images used in the `Dockerfile`. If this happens to you, try downloading images that are used in the dockerfile:

```cmd
$> docker pull quay.io/gsfsgroup/hg-images:redline-build-dotnet-5.0
$> docker pull quay.io/gsfsgroup/hg-images:redline-release-aspnet-5.0
```
Assuming you didn't get any errors, you should be able to see your application after logging in at <https://host.docker.internal:5701/#/developercenter>

> **INFO:** You can stop the application by running `rl-docker-compose down -v` from a terminal at the root of the solution (where you have the `docker-compose.yml` file). The `-v` argument removes the volumes defined in the `docker-compose.yml` file. Removing volumes helps side-stepping issues that might come up.

### How to debug a host project locally

In order to debug a host project (web, messaging, or jobs), you can start the dependencies in docker-compose and start a debug session for the host project. The following command will bring up all dependencies including the migrations:
```
$> rl-docker-compose up --build -d deps
```

However, if you want to debug the data migrations, you will have to bring up just the services that `developercenter-migrations` depends on.
```
$> rl-docker-compose up -d postgres
```

## <a id="git-repo-setup"></a>GitHub repository setup and first pull request

1. If [GitHub repository for Zinc.DeveloperCenter](https://github.com/gsfsgroup/Zinc.DeveloperCenter) is not set up, you will need to [create it before moving on to the next task](https://gsfsgroup.atlassian.net/wiki/spaces/IPD/pages/1345748993/How+to+Set+Up+a+New+GitHub+Repository). If you don't have privileges to create the repository, seek assistance from your manager.

2. If you have not forked [GitHub repository for Zinc.DeveloperCenter](https://github.com/gsfsgroup/Zinc.DeveloperCenter) already, fork it on GitHub.

3. If you have not cloned from your fork to create `Zinc.DeveloperCenter` folder, you will need to initialize git. Following commands do that:
    ```cmd
    $> git init
    $> git remote add origin git@github.com:{GITHUB_USERNAME}/Zinc.DeveloperCenter # your fork
    ```

4. Make sure the `upstream` is set up correctly. You can use `git remote -v` to check that. Output should be similar to:
    ```cmd
    $> git remote -v
    origin  git@github.com:{GITHUB_USERNAME}/Zinc.DeveloperCenter.git (fetch)
    origin  git@github.com:{GITHUB_USERNAME}/Zinc.DeveloperCenter.git (push)
    upstream        git@github.com:GSFSGroup/Zinc.DeveloperCenter (fetch)
    upstream        git@github.com:GSFSGroup/Zinc.DeveloperCenter (push)
    ```

    If, you don't see upstream in the output, you can add it:
    ```cmd
    git remote add upstream git@github.com:GSFSGroup/Zinc.DeveloperCenter       # upstream
    ```

5. You can now commit your changes and push to your fork.
    ```cmd
    git add .                      # Add all changes
    git commit -m 'Initial load'   # Commit.
    git push -u origin master      # Push changes to your fork.
    ```

6. Next, you can visit your github fork and create a pull request.

> **INFO:** If you are using [GitHub CLI](https://github.com/cli/cli#installation) you can use the following command to create your PR: `gh pr create --web`

## <a id="build-errors"></a>Diagnosing CircleCI build errors

You might encounter build errors during CICD builds. These pointers might help in that case:

-   If the build fails at ___Build docker images___ step, the output in the step will have the errors that docker encountered during the build. You can see the same errors on your machine when you run `rl-docker-compose-test build`.

-   If the build fails at ___Bring up the images___ step, you can check a few more things to diagnose.

    -   Check the output from ___Document the runtime environment___ step. It contains the output from `rl-docker-compose-test ps`, which shows all services and their statuses. If one of the services failed, you could figure out which one by looking at this output.

    -   Artifacts tab (at the top of the build page in CirleCI) includes logs captured during the docker-compose run for each service. You can see each service's logs to see what might have gone wrong.

-   If the build fails at ___Run the security scan over the Docker image___ step, the output will show the details of the vulnerability that is discovered. Fixing that requires updating the [Dockerfile for the release image](https://github.com/GSFSGroup/Mercury.Images/blob/master/redline-build-dotnet-5.0/Dockerfile) to remove the vulnerability.

-   If the build fails at one of the ___Publish the Release Docker image___ steps, it will most likely be an unauthorized error. This happens when the image repository on quay.io is not set up properly. Review [quay.io instructions in configuration instructions](https://gsfsgroup.atlassian.net/wiki/spaces/IPD/pages/1345748993/How+to+Set+Up+a+New+GitHub+Repository#Configure-the-Tooling).
## <a id="create-app-certs"></a>Create app certs in preprod

Each microservice deployed in the kubernetes cluster must have a certificate signed by the cluster's CA for identity and to establish trust. This work has been simplified by a shell script in `.cicd/remote/certs/make-app-certs.sh`.

> __CAUTION__: This script makes use of [cfssl and cfssljson version 1.6.0 ](https://github.com/cloudflare/cfssl/releases) and [kubectl](https://kubernetes.io/docs/tasks/tools/). Make sure you have them installed before moving on. [Check out this document to have kubectl installed and configured for the preprod cluster](https://github.com/GSFSGroup/Zinc.DeveloperSetup/blob/master/FIRST-TIME-SETUP.md#install-kubectl). On Windows, you will need to use __Git-Bash__ and make sure `kubectl.exe`, `cfssl.exe` and `cfssljson.exe` are in the path. For MacOS and Linux, `kubectl`, `cfssl`, and `cfssljson` should be in the path.

The script requires no parameters, but three main environment variables:

-   `RL_APP_NAME` is the name of this application (`zn-developercenter`).
-   `RL_HOST_NAME` is the name of the host that the cert will be for (for instance, `zn-developercenter-jobs`).
-   `CERT_X509_PASSWORD` is a random password. You can use your password manager to create one.

You can follow these instructions to create the app certificates in the kubernetes cluster before first deployment:

```sh
# First, make sure that you are in the correct context.
kubectl config current-context
# This should give an output that shows the pre-production cluster (like, hawking-dimebox-io) is selected. If not, run `kubectl config use-context hawking-dimebox-io` to select that context.

# If the kube context is correct:
export RL_APP_NAME=zn-developercenter && export RL_HOST_NAME=zn-developercenter && export CERT_X509_PASSWORD=$(base64 /dev/urandom | head -c 16) && bash .cicd/remote/certs/make-app-certs.sh

# one more time for messaging
export RL_APP_NAME=zn-developercenter && export RL_HOST_NAME=zn-developercenter-messaging && export CERT_X509_PASSWORD=$(base64 /dev/urandom | head -c 16) && bash .cicd/remote/certs/make-app-certs.sh

# one more time for jobs
export RL_APP_NAME=zn-developercenter && export RL_HOST_NAME=zn-developercenter-jobs && export CERT_X509_PASSWORD=$(base64 /dev/urandom | head -c 16) && bash .cicd/remote/certs/make-app-certs.sh
```

## <a id="create-db-user"></a>Create DB user and grant access

A PostgreSQL database is required for storing the data in this microservice, unless you change the code for using another type of storage. Assuming it's the one that came from the template, you will need to create a service account in PostgreSQL for this microservice.

You will need admin access to the PostgreSQL server that will be used to host the database.  The admin credentials can be retrieved from AWS parameter store from the following key: `/rds/accounts/serviceaccount/{{cluster}}-{{city}}-io-postgressql/admin`.  As an example, the hawking cluster on dimebox uses the following key: `/rds/accounts/serviceaccount/hawking-dimebox-io-postgressql/admin`.

If you are connecting to the RDS instance from an off-premise location, such as from home, you first need to add your IP address to the `DevAtHomeAccess-{{cluster}}` security group.  Below are the steps to accomplish that:

1.  Login to the AWS account for the cluster
2.  Go to `EC2` from the list of services
3.  Select `Security Groups` from the navigation
4.  Search for `DevAtHomeAccess-{{cluster}}`, e.g. `DevAtHomeAccess-Hawking`, and select the security group.
5.  Click on the `Inbound Rules` tab and click on `Edit inbound rules`
6.  Click `Add rule` at the bottom of the screen and fill in the following values:
    -   **Type**: Select `PostgreSQL` from the drop-down
    -   **Source**: Select `My IP` from the drop-down
    -   **Description**: Use the following pattern `{{Your Name}} - {{Location}} - {{Service}}`
7.  Click `Save rules`

You can use your favorite DB tool to connect to the server with the admin account. Run  SQL like below to create the service account:

```sql
-- Replace {random-name} and {random-password} with any alphanumeric text (no spaces) of at least 16 characters.
CREATE USER "svc-{random-name}" PASSWORD '{random-password}' CREATEDB;
```

> __IMPORTANT__: `CREATEDB` privilege must be given to the user, because the microservice creates the database at first deployment.

Take a note of the username and password you created for the service account. You will need to use that when setting up AWS parameter store. For the sake of brevity, assume that `$POSTGRES_USER` and `$POSTGRES_PASSWORD`, in the rest of this document, refer to the user name and password set up at this step.

## <a id="aws-parameter-store"></a>Push AWS Parameter Store configuration

The configuration for the microservice is pulled from AWS parameter store, when it is in the remote context, as in Kubernetes environment. CircleCI builds do not use the remote context. You can review the `AddParameterStoreSettings` method in [ConfigurationExtensions.cs](./src/RedLine/RedLine.Extensions.Hosting/ConfigurationBuilderExtensions.cs) to see how this is done.

Before deploying to Kubernetes environments, you need to set up AWS parameter store configurations.

*   If you haven't installed aws cli tool already, [install and configure aws account for preprod](https://github.com/GSFSGroup/Zinc.DeveloperSetup/blob/master/FIRST-TIME-SETUP.md#install-aws-cli).

*   Create a JSON file, (assuming that you will save it as `zn-developercenter-preprod.json` for ease of reference), that has the configuration like:

    ```json
    {
        "ApplicationContext": {
            "PostgresUser": "<$POSTGRES_USER from the previous step>",
            "PostgresPassword": "<$POSTGRES_PASSWORD from the previous step>",
            "ServiceAccountName": "zn-developercenter@redline.services"
        },
        "Swagger": {
            "OAuth2RedirectUrl": "https://developercenter.hawking.dimebox.io",
            "Disabled": "false"
        }
    }
    ```
*   Apply this configuration using the `mercury` tool:

    ```sh
    mercury redline create-application-configuration --aws-region us-east-1 --app zn-developercenter --context remote --env hawking --config zn-developercenter-preprod.json
    ```

The command assumes that we are deploying to `hawking` and the configuration is saved in `zn-developercenter-preprod.json` file. Follow the on-screen instructions to apply the configuration.

## <a id="mercury-preprod"></a>Creating deployment files in Mercury.PreProd repo

This template includes the barebones of kubernetes yaml files for deployment, service, ingress, and horizontal pod autoscaler resources. They provide a good starting point to configure resources. You will need to [have kubectl installed and configured for the preprod cluster](https://github.com/GSFSGroup/Zinc.DeveloperSetup/blob/master/FIRST-TIME-SETUP.md#install-kubectl).

-   Look into the container image repository in quay.io to get the latest hashed tag and update all instances of `image: quay.io/gsfsgroup/zn-developercenter:be764a5` in `.cicd/remote/zn.developercenter` folder to use that latest tag.
-   Copy the `.cicd/remote/zn.developercenter` folder from this repository into Mercury.PreProd `clusters/hawking/configs/managed/zn.developercenter`.
-   Look into the other ingress yamls in the same cluster and update `zalando.org/aws-load-balancer-ssl-cert` annotation in line 10 of `zn-developercenter.ingress.yaml` with the correct certificate ARN. You can copy this value from an existing ingress in the same cluster.
-   In order to make sure the deployment will work after push, apply the deployment file using kubectl. Assuming that you are in the same `zn.developercenter` folder in Mercury.PreProd:

```sh
# (1) next line applies the deployment configuration to cluster.
kubectl apply -f zn-developercenter.deployment.yaml

# (2) now we can watch the kubernetes events. This will block the terminal, since it will keep watching. Exit that with Ctrl-C when you are ready.
kubectl get events -w

# (3) you can use this next line to see pod descriptions and the lifecycle events. It describes all pods that have app=zn-developercenter label (-l) in default namespace (-n):
kubectl describe pod -l app=zn-developercenter -n default

# (4) You can watch the logs of all pods using the following command. (-f) parameter tells kubectl to keep watching. Exit that with Ctrl-C when you are ready.
kubectl logs -l app=zn-developercenter -n default -f

# (5) Or, you can see logs from a specific pod. First list the pods and then grab a pod name from the list to plug in the next command.
kubectl get pods -l app=zn-developercenter -n default
kubectl logs <pod-name-from-above-output>

# (6) When pods seem to be running without an issue, you can delete the deployment to allow the automated deployment create the resources.
kubectl delete -f zn-developercenter.deployment.yaml
```

-   If there are any errors with volumes or pulling the container image, they will show up in the output of the `(2)` and/or `(3)`. If there were no problems with container creation and volume mapping, you will need to use commands in `(4)` or `(5)` to see the logs and diagnose the issue.
-   Once the pods seem to be running without an issue, you can remove delete the deployment using command `(6)`. Commit your changes to the Mercury.PreProd and create a pull request to deploy. When the pull request gets merged, the resources you defined will be applied to the cluster automatically.

## <a id="configure-shell"></a>Configuring a shell to load microapp

In order to see the microapp from a shell, it needs to be added to its configuration. Assuming that you want to deploy to `kr-shell`, you can follow these steps to add to the shell configuration:

-   Retrieve existing configuration for the shell. Note the last item in `Shell.Applications`. Assuming that it is `9`. Configuration for this microapp will be at index `10`.

```sh
# This command will print a JSON of the configuration. Note the largest number in `Shell.Applications`.
mercury redline get-application-configuration --aws-region us-east-1 --json --app kr-shell --context remote --env hawking
```

-   Create a json file for the config. We'll assume `kr-shell-updates.json`. Add the content for the microapp to it:

```json
{
    "Shell": {
        "Applications": {
            "10": {
                "ApplicationComponent": "zn-developercenter",
                "IngressUrl": "https://developercenter.hawking.dimebox.io",
                "Route": "developercenter",
                "description": "Zinc.DeveloperCenter",
                "icon": {
                    "abbreviation": "R",
                    "category": "RedLine",
                    "type": "element",
                    "value": "zinc"
                },
                "name": "Zinc.DeveloperCenter",
                "title": "developercenter"
            }
        }
    }
}
```

-   Push this configuration using `mercury` tool.

```sh
mercury redline create-application-configuration --aws-region us-east-1 --app kr-shell --context remote --env hawking --config kr-shell-updates.json
```

-   Restart the shell pods to make the configuration take effect. Assuming `kr-shell` is used, you can use the next command:

```sh
kubectl delete pods -l app=kr-shell
```

## <a id="configure-admin"></a>Configuring Administration

In order to see the microapp in Administration, it needs to be added to its configuration. Assuming that you want to deploy to `kr-administration`, you can follow these steps to add to the application configuration:

-   Retrieve existing configuration for Administration. Note the last item in `DistributedConfiguration.Applications`. Assuming that it is `9`. Configuration for this microapp will be at index `10`.

```sh
# This command will print a JSON of the configuration. Note the largest number in `DistributedConfiguration.Applications`.
mercury redline get-application-configuration --aws-region us-east-1 --json --app kr-administration --context remote --env hawking
```

-   Create a json file for the config. We'll assume `kr-administration-updates.json`. Add the content for the microapp to it:

```json
{
    "DistributedConfiguration": {
        "Applications": {
            "10": {
                "Name": "Zinc.DeveloperCenter",
                "ServiceEndpoint": "https://zn-developercenter-svc.default.svc.cluster.local"
            }
        }
    }
}
```

-   Push this configuration using `mercury` tool.

```sh
mercury redline create-application-configuration --aws-region us-east-1 --app kr-administration --context remote --env hawking --config kr-administration-updates.json
```

-   Restart the Administration pods to make the configuration take effect. Assuming `kr-administration` is used, you can use the next command:

```sh
kubectl delete pods -l app=kr-administration
```

Administration also makes use of the `ApplicationContext.ApplicationDisplayName` setting for this microapp. Set this in the 4 `appsettings.json`: `Zinc.DeveloperCenter.Host.Web`, `Zinc.DeveloperCenter.Host.Messaging`, `Zinc.DeveloperCenter.Host.Jobs`, `Zinc.DeveloperCenter.Data.Migrations`.

## <a id="configure-authn"></a>Configuring AuthN for service-to-service authentication

Service to service communication requires authentication by authn service. It does this by setting up a client for each service and uses the public certificate of that service to ensure authenticity. Follow these steps to configure AuthN for this service:

*   Open `kr-authentication.deployment.yaml` from [Mercury.PreProd](https://github.com/gsfsgroup/mercury.preprod) in your editor. Make sure the cluster name is correct (e.g. edit the file under `hawking` folder for that cluster).

*   Check the `svc-public-keys` volume. If not added yet, add the following mapping to the end:

    ```yaml
            - configMap:
                name: zn-developercenter-svc-cert-public
                items:
                - key: redline-public.crt
                    path: zn-developercenter-public.crt
    ```

*   Create a pull request with this change.

*   Use `mercury` tool to figure out how many clients are defined.

    ```cmd
    mercury redline get-application-configuration --aws-region us-east-1 --app kr-authentication --context remote --env hawking --json
    ```
    Read the output to find the biggest index number for `$.Authentication.Clients`. Let's say it is 15.

*   Create a json file, `kr-authentication-hawking.json`, to use with `mercury` tool for the update:

    ```json
    {
        "Authentication": {
            "Clients": {
                "15": {
                    "AllowedGrantTypes": {
                    "0": "client_credentials"
                    },
                    "AllowedScopes": {
                    "0": "redline.app"
                    },
                    "Claims": {
                    "0": {
                        "Type": "upn",
                        "Value": "zn-developercenter@redline.services"
                    },
                    "1": {
                        "Type": "email",
                        "Value": "zn-developercenter@redline.services"
                    },
                    "2": {
                        "Type": "name",
                        "Value": "Finance ACL"
                    }
                    },
                    "ClientClaimsPrefix": " ",
                    "ClientId": "zn-developercenter@redline.services",
                    "ClientSecrets": {
                        "0": {
                            "Type": "X509CertificatePemFile",
                            "Value": "/svc-public-keys/zn-developercenter-public.crt"
                        }
                    }
                }
            }
        }
    }
    ```
*   Apply the configuration using `mercury` tool:
    ```cmd
    mercury redline create-application-configuration --aws-region us-east-1 --app kr-authentication --context remote --env hawking --config kr-authentication-hawking.json
    ```

    Notice that the `--config` parameter takes the path to the json file we created in the previous step.

## <a id="configure-authz"></a>Configure AuthZ

Setup application and grants as [Krypton.Authorization documentation suggests](https://github.com/GSFSGroup/Krypton.Authorization#distributed-application-workflows). You can [use this migration as an example](https://github.com/GSFSGroup/Krypton.Authorization/blob/a3fac7a2201ab72c2527e99e25641a5b708231e3/src/Krypton.Authorization.Data/Migrations/Migration_2022010401_KrAdminSevcAcctGrants.cs).

## <a id="ensure-datadog"></a>Ensure logs are accessible through DataDog

As the last step, go ahead and check whether you can see the logs for this microapp in DataDog. If you cannot see them, there must be some problem, however diagnosing those outside the scope of this document.

## <a id="schedule-a-job"></a>Schedule a Job
**Jobs are scheduled based on the UTC+0 timezone.**

A cron expression is a string comprised of 6 or 7 fields separated by white space. Fields can contain any of the allowed values, along with various combinations of the allowed special characters for that field. The fields are explained in the following table:

| Field Name       | Mandatory? | Allowed Values   | Allowed special Characters |
| :--------------- | :--------- | :--------------- | :------------------------- |
| Seconds          | Yes        | 0-59             | , - * /                    |
| Minutes          | Yes        |0-59              | , - * /                    |
| Hours            | Yes        | 0-23             | , - * /                    |
| Day of the Month | Yes        | 1-31             | , - * ? / L W              |
| Month            | Yes        | 1-12 or JAN-DEC  | , - * /                    |
| Day of the Week  | Yes        | 1-7 OR SUN-SAT   | , - * ? / L #              |
| Year             | No         | EMPTY, 1970-2099 | , - * /                    |

#### Examples:

To schedule a job for 12:05pm CST on january 6th 2040 you would use
**"0 5 18 6 1 * 2040"**

| Second | Minute | Hour                       | Day  | Month | Day-of-week | Year |
| :----- | :----- | :------------------------- | :--- | :---- | :---------- | :--- |
| 0      | 5      | 12(UTC) + 6(offset) = 18   | 6    | 1     |             | 2040 |
