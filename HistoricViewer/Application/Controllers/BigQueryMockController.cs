using Google.Cloud.BigQuery.V2;
using HistoricViewer.Infrastructure.gcp.bigQuery;
using Microsoft.AspNetCore.Mvc;

namespace HistoricViewer.Application.Controllers;


// http://localhost:5107/BigQueryMock?from=2026-01-01T00%3A00%3A00Z&to=2026-03-11T23%3A59%3A59Z&limit=1000
// from 2026-01-01T00:00:00Z
// to 2026-03-11T23:59:59Z
[ApiController]
[Route("[controller]")]
public class BigQueryMockController(IBigQueryProxyService bigQuery) : ControllerBase
{
    
    
    private readonly IBigQueryProxyService _bigQuery = bigQuery;


    [HttpGet]
    public async Task<IActionResult> Get(
        DateTime? from = null,
        DateTime? to = null,
        int limit = 1000
        )
    {
        
        // Default: last 24 hours in UTC
        var fromUtc = DateTime.SpecifyKind(from ?? DateTime.UtcNow.AddDays(-1), DateTimeKind.Utc);
        var toUtc   = DateTime.SpecifyKind(to   ?? DateTime.UtcNow,             DateTimeKind.Utc);


        var sql = $@"
        SELECT *
        FROM `ml-mps-adl-mndsa-dlmpds-p-c37b.phi_dlmpds_us_p.T_CLIENT`
        WHERE ROW_LOADED_DTM BETWEEN @from AND @to
        ORDER BY ROW_LOADED_DTM DESC
        LIMIT {limit}";

        var result = await _bigQuery.ExecuteQueryAsync(sql, new Dictionary<string, object>
        {
            ["from"] = fromUtc,
            ["to"]   = toUtc
        });

        //Return proper HTTP status based on success
        if (!result.Success)
            return StatusCode(500, new { error = result.ErrorMessage });
        
        return Ok(result);
    }
    
}