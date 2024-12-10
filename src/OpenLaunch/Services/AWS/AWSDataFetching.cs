using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using OpenLaunch.Interfaces;
using Serilog;
using VerificationStatus = OpenLaunch.Interfaces.VerificationStatus;

namespace OpenLaunch.Services;

public class AWSDataFetching : IExternalDataFetching
{
    private readonly AmazonSimpleEmailServiceV2Client _sesClient;

    public AWSDataFetching(AmazonSimpleEmailServiceV2Client client)
    {
        _sesClient = client;
    }

    public async Task<IMetricDataResponse> FetchBatchMetricsAsync(DateTime startDate, DateTime endDate)
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

        try
        {
            var response = await _sesClient.BatchGetMetricDataAsync(request);
            
            var metricDataResponse = new MetricDataResponse
            {
                Results = response.Results.Select(result => (IMetricResult)new MetricResult
                {
                    Id = result.Id,
                    Timestamps = result.Timestamps.ToList(),
                    Values = result.Values.Select(x => (double)x).ToList(),
                }).ToList()
            };
            
            return metricDataResponse;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching metric data");
            throw;
        }
    }

    public async Task<List<IIdentityInfo>> ListIdentitiesAsync()
    {
        var identityType = IdentityType.DOMAIN;
        var result = new List<IIdentityInfo>();

        try
        {
            var response = await _sesClient.ListEmailIdentitiesAsync(
                new ListEmailIdentitiesRequest
                { 
                });
            
            result = response.EmailIdentities.Select(identity => (IIdentityInfo)new IdentityInfo
            {
                IdentityName = identity.IdentityName,
                SendingEnabled = identity.SendingEnabled,
                VerificationStatus = Enum
                    .TryParse(identity.VerificationStatus, true, out VerificationStatus status)
                    ? status
                    : VerificationStatus.Failure
            }).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to fetch email identities");
        }

        return result;
    }
}

/* These implement the interfaces for the metric and identity responses, IDK if other providers will require
 different implementations, so I will leave them here for now */
public class MetricDataResponse : IMetricDataResponse
{
    public List<IMetricResult> Results { get; set; } = new List<IMetricResult>();
}

public class MetricResult : IMetricResult
{
    public string Id { get; set; }
    public List<DateTime> Timestamps { get; set; } = new List<DateTime>();
    public List<double> Values { get; set; } = new List<double>();
}

public class IdentityInfo : IIdentityInfo
{
    public string IdentityName { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public bool SendingEnabled { get; set; }
}