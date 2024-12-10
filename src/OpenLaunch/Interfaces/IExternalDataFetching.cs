using Amazon.SimpleEmailV2.Model;
using OpenLaunch.Interfaces;

namespace OpenLaunch.Services;

public interface IExternalDataFetching
{
    Task<IMetricDataResponse> FetchBatchMetricsAsync(DateTime startDate, DateTime endDate);
    Task<List<IIdentityInfo>> ListIdentitiesAsync();
}