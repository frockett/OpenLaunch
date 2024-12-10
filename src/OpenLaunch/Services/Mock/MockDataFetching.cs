using OpenLaunch.Interfaces;

namespace OpenLaunch.Services;

public class MockDataFetching : IExternalDataFetching
{
    public async Task<IMetricDataResponse> FetchBatchMetricsAsync(DateTime startDate, DateTime endDate)
        { 
            await Task.Delay(100);
            
            var totalDays = (endDate - startDate).Days + 1;

            var fakeData = new MetricDataResponse
            {
                Results = new List<IMetricResult>
                {
                    new MetricResult
                    {
                        Id = "send_1",
                        Timestamps = GenerateTimestamps(startDate, endDate),
                        Values = GenerateRandomValues(totalDays)
                    },
                    // Delivery metric
                    new MetricResult
                    {
                        Id = "delivery_2",
                        Timestamps = GenerateTimestamps(startDate, endDate),
                        Values = GenerateRandomValues(totalDays)
                    },
                    // Open metric
                    new MetricResult
                    {
                        Id = "opens_3",
                        Timestamps = GenerateTimestamps(startDate, endDate),
                        Values = GenerateRandomValues(totalDays)
                    },
                    // Bounce metric
                    new MetricResult
                    {
                        Id = "bounce_4",
                        Timestamps = GenerateTimestamps(startDate, endDate),
                        Values = GenerateRandomValues(totalDays)
                    },
                    // Block metric
                    new MetricResult
                    {
                        Id = "block_5",
                        Timestamps = GenerateTimestamps(startDate, endDate),
                        Values = GenerateRandomValues(totalDays)
                    },
                    // Spam complaint metric
                    new MetricResult
                    {
                        Id = "spam_complaint_6",
                        Timestamps = GenerateTimestamps(startDate, endDate),
                        Values = GenerateRandomValues(totalDays)
                    }
                }
            };

            return fakeData;
        }

        public async Task<List<IIdentityInfo>> ListIdentitiesAsync()
        {
            await Task.Delay(100);

            var fakeIdentities = new List<IIdentityInfo>
            {
                new IdentityInfo
                {
                    IdentityName = "mocked@example.com",
                    VerificationStatus = VerificationStatus.Success,
                    SendingEnabled = true
                },
                new IdentityInfo
                {
                    IdentityName = "unverified@example.com",
                    VerificationStatus = VerificationStatus.Failure,
                    SendingEnabled = false
                }
            };

            return fakeIdentities;
        }
        
        private List<DateTime> GenerateTimestamps(DateTime startDate, DateTime endDate)
        {
            var timestamps = new List<DateTime>();
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                timestamps.Add(currentDate);
                currentDate = currentDate.AddDays(1);
            }

            return timestamps;
        }
        
        private List<double> GenerateRandomValues(int numDays)
        {
            var random = new Random();
            return Enumerable.Range(0, numDays)  
                             .Select(_ => random.NextDouble() * 100)
                             .ToList();
        }
}