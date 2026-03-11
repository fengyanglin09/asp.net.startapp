namespace HistoricViewer.Infrastructure.gcp.bigQuery;

public class BigQueryServiceOptions
{
    public const string SectionName = "BigQuery";

    /// <summary>Email of the BigQuery SA to impersonate (in Project A)</summary>
    public string BigQueryServiceAccountEmail { get; set; } = string.Empty;

    /// <summary>GCP Project ID where BigQuery datasets live (Project B)</summary>
    public string TargetProjectId { get; set; } = string.Empty;
}