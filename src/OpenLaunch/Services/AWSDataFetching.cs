using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;

namespace OpenLaunch.Services;

public class AWSDataFetching
{
    private readonly AmazonSimpleEmailServiceV2Client _sesClient;

    public AWSDataFetching(AmazonSimpleEmailServiceV2Client client)
    {
        _sesClient = client;
    }

    public async Task<BatchGetMetricDataResponse> FetchBatchMetricsAsync(DateTime startDate, DateTime endDate)
    {
        BatchGetMetricDataRequest request = new BatchGetMetricDataRequest();
        var metrics = new[]
        {
            new { Id = "send_1", Metric = Metric.SEND },
            new { Id = "delivery_2", Metric = Metric.DELIVERY },
            new { Id = "opens_3", Metric = Metric.OPEN },
            new { Id = "clicks_4", Metric = Metric.CLICK },
            new { Id = "transientBounce_5", Metric = Metric.TRANSIENT_BOUNCE },
            new { Id = "permanentBounce_6", Metric = Metric.PERMANENT_BOUNCE },
            new { Id = "complaints_7", Metric = Metric.COMPLAINT }
        };

        foreach (var metric in metrics)
        {
            request.Queries.Add(new BatchGetMetricDataQuery
            {
                Id = metric.Id,
                Namespace = MetricNamespace.VDM,
                Metric = metric.Metric,
                StartDate = startDate,
                EndDate = endDate
            });
        }
        
        return await _sesClient.BatchGetMetricDataAsync(request);
    }
}