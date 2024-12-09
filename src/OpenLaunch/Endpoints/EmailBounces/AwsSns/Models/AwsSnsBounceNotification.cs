using System.Text.Json.Serialization;

namespace OpenLaunch.Endpoints.EmailBounces.AwsSns.Models;

    public class BounceNotification
    {
        [JsonPropertyName("Type")]
        public string NotificationType { get; set; }
        
        [JsonPropertyName("MessageId")]
        public string MessageId { get; set; }
        
        [JsonPropertyName("TopicArn")]
        public string TopicArn { get; set; }
        
        [JsonPropertyName("Message")]
        public string Message { get; set; } // This is the actual bounce message (JSON string)
        
        [JsonPropertyName("Timestamp")]
        public DateTime Timestamp { get; set; }
        
        [JsonPropertyName("SignatureVersion")]
        public string SignatureVersion { get; set; }
        
        [JsonPropertyName("Signature")]
        public string Signature { get; set; }
        
        [JsonPropertyName("SigningCertURL")]
        public string SigningCertURL { get; set; }
        
        [JsonPropertyName("UnsubscribeURL")]
        public string UnsubscribeURL { get; set; }
    }

    public class BounceMessage
    {
        [JsonPropertyName("notificationType")]
        public string NotificationType { get; set; }  // "Bounce"
        
        [JsonPropertyName("bounce")]
        public BounceDetails Bounce { get; set; }
        
        [JsonPropertyName("mail")]
        public MailDetails Mail { get; set; }
    }

    public class BounceDetails
    {
        [JsonPropertyName("feedbackId")]
        public string FeedbackId { get; set; }
        
        [JsonPropertyName("bounceType")]
        public string BounceType { get; set; }    // "Permanent" or "Transient"
        
        [JsonPropertyName("bounceSubType")]
        public string BounceSubType { get; set; }  // "General" or other subtypes
        
        [JsonPropertyName("bouncedRecipients")]
        public List<BouncedRecipient> BouncedRecipients { get; set; }
        
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
        
        [JsonPropertyName("remoteMtaIp")]
        public string RemoteMtaIp { get; set; }
        
        [JsonPropertyName("reportingMta")]
        public string ReportingMta { get; set; }
    }

    public class BouncedRecipient
    {
        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; }
        
        [JsonPropertyName("action")]
        public string Action { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; }
        
        [JsonPropertyName("diagnosticCode")]
        public string DiagnosticCode { get; set; }
    }

    public class MailDetails
    {
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
        
        [JsonPropertyName("source")]
        public string Source { get; set; }
        
        [JsonPropertyName("sourceArn")]
        public string SourceArn { get; set; }
        
        [JsonPropertyName("sourceIp")]
        public string SourceIp { get; set; }
        
        [JsonPropertyName("callerIdentity")]
        public string CallerIdentity { get; set; }
        
        [JsonPropertyName("sendingAccountId")]
        public string SendingAccountId { get; set; }
        
        [JsonPropertyName("messageId")]
        public string MessageId { get; set; }
        
        [JsonPropertyName("destination")]
        public List<string> Destination { get; set; }
    }