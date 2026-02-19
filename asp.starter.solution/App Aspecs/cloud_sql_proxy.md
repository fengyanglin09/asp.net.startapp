hat you have set up the [Application Default Credentials](application_default_credentials.md).

## HOWTO

## Logging in as a User

```shell
$ gcloud auth login --update-adc
```


## Setting the Project

```shell
$ gcloud projects list
```

```shell
$ gcloud config get-value project
```

```shell
$ gcloud config set project PROJECT_ID
```

* Submit Build
    * dev: smash-webapp-d-404913688383
        * Command
          ```shell
          $ gcloud builds submit --config=cloudbuild.yaml --substitutions=TAG_NAME="snapshot",_GCP_ARTIFACT_REPO="us-central1-docker.pkg.dev/smash-webapp-d-404913688383/artifact-repository"
          ```

### Connecting over TCP for LINUX/MacOS
In a new terminal, start the Cloud SQL Auth Proxy for your instance. If you run more than one proxy, make sure you use a
unique port number for each proxy. It is recommended you use the standard TCP for MySQL (3306).
```shell
$ ./cloud-sql-proxy INSTANCE_CONNECTION_NAME &
```
# Or Windows (with cloud-sql-proxy on the path and the specific instance specified)
# Dev
```shell
$ cloud-sql-proxy spaa-dashboard-d:us-central1:spaa-dashboard-d-db-dlmp-cad-dev
```

# Test
```shell
$ cloud-sql-proxy spaa-dashboard-t:us-central1:spaa-dashboard-t-db-dlmp-cad-test
```