using Google.Cloud.BigQuery.V2;

namespace HistoricViewer.Infrastructure.gcp.bigQuery;

// ── DTOs ────────────────────────────────────────────────────────────────────
public class BigQueryResult
{
    public bool Success { get; set; }
    public List<Dictionary<string, object?>> Rows { get; set; } = new();
    public List<BigQueryColumnInfo>? Schema { get; set; }
    public long TotalRows { get; set; }
    public string? ErrorMessage { get; set; }
    public long BytesProcessed { get; set; }
}

public class BigQueryColumnInfo
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
}

public class BigQueryQueryOptions
{
    public string? ProjectId { get; set; }
    public TimeSpan? Timeout { get; set; }
    public bool UseLegacySql { get; set; } = false;
    public int? PageSize { get; set; }
}


