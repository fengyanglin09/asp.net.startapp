using Google.Cloud.BigQuery.V2;
using HistoricViewer.Infrastructure.gcp.bigQuery;
using Microsoft.AspNetCore.Mvc;


namespace HistoricViewer.Application.Controllers;


// http://localhost:5107/BigQueryMock?from=2026-01-01T00%3A00%3A00Z&to=2026-03-11T23%3A59%3A59Z&limit=1000
// from 2026-01-01T00:00:00Z
// to 2026-03-11T23:59:59Z
[ApiController]
[Route("api/bigquery-testing")]
// [ApiExplorerSettings(GroupName = "BigQuery Testing")]
public class BigQueryTestingController(IBigQueryProxyService bigQuery) : ControllerBase
{ 
    
    
    private readonly IBigQueryProxyService _bigQuery = bigQuery;

    //todo - test this WHERE ORDER_CREATED_DATETIME BETWEEN '2020-09-18' AND '2022-02-12'

    [HttpGet("softlab-orders-tests", Name = "test-softlab-orders")]
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
        FROM `ml-mps-adl-mndsa-dlmpds-d-23a9.phi_dlmpds_stg_us_d.T_STG_SOFTLAB_ORDERS_TESTS`
        WHERE TIMESTAMP(ORDER_CREATED_DATETIME) BETWEEN @from AND @to
        ORDER BY ORDER_KEY DESC
        LIMIT {limit}";


        // var sql2 = $@"
        //           SELECT * FROM `ml-mps-adl-mndsa-dlmpds-d-23a9.phi_dlmpds_stg_us_d.EXT_Division_Test_List` 
        //           LIMIT 1000
        //             ";

        var result = await _bigQuery.ExecuteQueryAsync(sql, new Dictionary<string, object>
        {
            ["from"] = fromUtc,
            ["to"]   = toUtc
        });

        // var result = await _bigQuery.ExecuteQueryAsync(sql);

        //Return proper HTTP status based on success
        if (!result.Success)
            return StatusCode(500, new { error = result.ErrorMessage });
        
        return Ok(result);
    }
    
}