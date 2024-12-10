using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using System.Text.RegularExpressions;
using OpenLaunch.Features.Unsubscribe.Services;
using OpenLaunch.Interfaces;
using OpenLaunch.Models;

namespace OpenLaunch.Services;

public class AWSEmailService : IEmailService
{
    private readonly AmazonSimpleEmailServiceV2Client _sesClient;
    private readonly UnsubscribeLinkService _unsubscribe;

    public AWSEmailService(AmazonSimpleEmailServiceV2Client client, UnsubscribeLinkService unsubscribe)
    {
        _sesClient = client;
        _unsubscribe = unsubscribe;
    }
    
    public async Task<EmailSendResult> SendEmailAsync(string fromEmailAddress,
        List<string> toEmailAddresses,
        string? subject,
        string? htmlContent,
        string? textContent,
        string? displayName = null,
        string? templateName = null,
        string? templateData = null,
        string? contactListName = null)
    {
        var result = new EmailSendResult();

        foreach (var toEmailAddress in toEmailAddresses)
        {
            // This is to help avoid issues with surpassing requests/second limits. Can make it a UI option later.
            Thread.Sleep(100);
            
            var request = new SendEmailRequest
            {
                FromEmailAddress = string.IsNullOrEmpty(displayName)
                    ? fromEmailAddress
                    : $"\"{displayName}\" <{fromEmailAddress}>",
                Destination = new Destination {ToAddresses = new List<string> {toEmailAddress}}
            };

            var unsubscribeLink = await _unsubscribe.GenerateUnsubscribeLink(toEmailAddress);

            var headers = new List<MessageHeader>
            {
                new MessageHeader
                {
                    Name = "List-Unsubscribe",
                    Value =
                        $"<{unsubscribeLink}>, <mailto:unsubscribe@yourdomain.com?subject=unsubscribe>",
                },
                new MessageHeader
                {
                    Name = "List-Unsubscribe-Post",
                    Value = $"<{unsubscribeLink}>",
                }
            };

            if (toEmailAddresses.Any())
            {
                request.Destination = new Destination { ToAddresses = toEmailAddresses };
            }

            if (!string.IsNullOrEmpty(templateName))
            {
                request.Content = new EmailContent()
                {
                    Template = new Template
                    {
                        TemplateName = templateName,
                        TemplateData = templateData,
                    }
                };
            }
            else
            {
                request.Content = new EmailContent
                {
                    Simple = new Message
                    {
                        Subject = new Content { Data = subject },
                        Body = new Body
                        {
                            Html = new Content { Data = htmlContent },
                            Text = new Content { Data = textContent },
                        },
                        Headers = headers
                    }
                };
            }

            if (!string.IsNullOrEmpty(contactListName))
            {
                request.ListManagementOptions = new ListManagementOptions
                {
                    ContactListName = contactListName
                };
            }
            
            try
            {
                var response = await _sesClient.SendEmailAsync(request);
                result.SuccessfullySentEmails.Add(toEmailAddress);
            }
            catch (AccountSuspendedException ex)
            {
                Console.WriteLine("The account's ability to send email has been permanently restricted.");
                Console.WriteLine(ex.Message);
                result.FailedEmails.Add((toEmailAddress, ex.Message));
            }
            catch (MailFromDomainNotVerifiedException ex)
            {
                Console.WriteLine("The sending domain is not verified.");
                Console.WriteLine(ex.Message);
                result.FailedEmails.Add((toEmailAddress, ex.Message));
            }
            catch (MessageRejectedException ex)
            {
                Console.WriteLine("The message content is invalid.");
                Console.WriteLine(ex.Message);
                result.FailedEmails.Add((toEmailAddress, ex.Message));
            }
            catch (SendingPausedException ex)
            {
                Console.WriteLine("The account's ability to send email is currently paused.");
                Console.WriteLine(ex.Message);
                result.FailedEmails.Add((toEmailAddress, ex.Message));
            }
            catch (TooManyRequestsException ex)
            {
                Console.WriteLine("Too many requests were made. Please try again later.");
                Console.WriteLine(ex.Message);
                result.FailedEmails.Add((toEmailAddress, ex.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while sending the email: {ex.Message}");
                result.FailedEmails.Add((toEmailAddress, ex.Message));
            }
        }

        return result;
    }
}