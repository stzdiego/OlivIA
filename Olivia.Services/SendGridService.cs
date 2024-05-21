// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Services;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using Olivia.Shared.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

/// <summary>
/// Represents a mail service.
/// </summary>
public class SendGridService : IMailService
{
    private readonly IMailSettings mailSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendGridService"/> class.
    /// </summary>
    /// <param name="mailSettings">Mail settings.</param>
    public SendGridService(IMailSettings mailSettings)
    {
        this.mailSettings = mailSettings;
    }

    /// <summary>
    /// Sends an email template.
    /// </summary>
    /// <param name="template">Template.</param>
    /// <param name="emails">Emails.</param>
    /// <param name="parameters">Parameters.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task SendEmailTemplateAsync(string template, IEnumerable<string> emails, object parameters)
    {
        try
        {
            var apiKey = this.mailSettings.Key;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(this.mailSettings.Mail, this.mailSettings.Name);
            var tos = emails.Select(email => new EmailAddress(email)).ToList();
            var msg = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(from, tos, template, parameters);
            var response = await client.SendEmailAsync(msg);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
