hat you have set up the [Application Default Credentials](application_default_credentials.md).

## HOWTO

## Logging in as a User

```shell
$ gcloud auth login --update-adc
```

## Sets up local ADC to already act as the BQ SA
```shell

gcloud auth application-default login --impersonate-service-account=bq-sa@project-a.iam.gserviceaccount.com
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


### Connecting over TCP for LINUX/MacOS
In a new terminal, start the Cloud SQL Auth Proxy for your instance. If you run more than one proxy, make sure you use a
unique port number for each proxy. It is recommended you use the standard TCP for MySQL (3306).
```shell
$ ./cloud-sql-proxy INSTANCE_CONNECTION_NAME &
```
# Or Windows (with cloud-sql-proxy on the path and the specific instance specified)
# Dev
```shell
$ cloud-sql-proxy lis-hist-vwr-d:us-central1:lis-hist-vwr-d-db-dlmp-cad-dev
```

# Test
```shell

```