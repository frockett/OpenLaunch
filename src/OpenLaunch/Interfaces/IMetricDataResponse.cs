namespace OpenLaunch.Interfaces;

public interface IMetricDataResponse
{
    List<IMetricResult> Results { get; set; }
}