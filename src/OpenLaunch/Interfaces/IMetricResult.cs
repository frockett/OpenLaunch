namespace OpenLaunch.Interfaces;

public interface IMetricResult
{
    string Id { get; set; }
    List<DateTime> Timestamps { get; set; }
    List<double> Values { get; set; }
}