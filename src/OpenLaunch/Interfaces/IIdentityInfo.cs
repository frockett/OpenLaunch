namespace OpenLaunch.Interfaces;

public interface IIdentityInfo
{
    string IdentityName { get; set; }
    VerificationStatus VerificationStatus { get; set; }
    bool SendingEnabled { get; set; }
}

public enum VerificationStatus
{
    Pending,
    Success,
    Failure,
}