##### premise:

- SA-A → Cloud Run runtime service account

- SA-B → BigQuery access service account

Your Cloud Run service runs as SA-A, but when it needs to access BigQuery it asks Google IAM for a temporary token for SA-B.

```shell
Cloud Run Service
      |
      | runs as
      v
SA-A (runtime service account)
      |
      | permission: roles/iam.serviceAccountTokenCreator
      v
IAM Credentials API
      |
      | generates short-lived token
      v
SA-B (target service account)
      |
      | BigQuery permissions
      v
BigQuery
```
So when BigQuery receives the request, it thinks:

- "This request is from SA-B, which has the necessary permissions to access BigQuery. I will allow it."



-----------------------------------------
#### Allow SA-A to impersonate SA-B:
```terraform
resource "google_service_account_iam_member" "impersonation" {
  service_account_id = google_service_account.bigquery_sa.name
  role               = "roles/iam.serviceAccountTokenCreator"
  member             = "serviceAccount:cloud-run-sa@project.iam.gserviceaccount.com"
}
```

#### services needed for this to work in the project:
You’ll enable APIs in the project that is making the calls (your Cloud Run project in Org A):
- BigQuery API (bigquery.googleapis.com) — required for normal BigQuery queries / jobs.
- Service Account Credentials API (iamcredentials.googleapis.com) — required to mint short-lived tokens for impersonation.


##### Optional / depends on how you read/write
- BigQuery Storage API (bigquerystorage.googleapis.com) — only needed if you’re using the Storage Read/Write APIs or some connectors that use it for faster reads/large results. For Storage Read, it’s typically enabled automatically when BigQuery API is enabled, but it still appears as a separate API in the console.

Quick Terraform snippet (Org A project)
```terraform
resource "google_project_service" "bigquery" {
  project = var.project_id
  service = "bigquery.googleapis.com"
}

# Only if you impersonate another SA
resource "google_project_service" "iamcredentials" {
  project = var.project_id
  service = "iamcredentials.googleapis.com"
}

# Optional (only if needed by your library/connector)
resource "google_project_service" "bigquerystorage" {
  project = var.project_id
  service = "bigquerystorage.googleapis.com"
}
```

------------------------------------------
