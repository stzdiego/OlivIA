// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Interfaces;

/// <summary>
/// Represents a mail service.
/// </summary>
public interface IMailService
{
    /// <summary>
    /// Sends an email.
    /// </summary>
    /// <param name="subject">Subject.</param>
    /// <param name="message">Message.</param>
    /// <param name="emails">Emails.</param>
    /// <param name="isHtml">Is HTML.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SendEmailAsync(string subject, string message, IEnumerable<string> emails, bool isHtml = false);

    /// <summary>
    /// Sends an email template.
    /// </summary>
    /// <param name="template">Template.</param>
    /// <param name="emails">Emails.</param>
    /// <param name="parameters">Parameters.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SendEmailTemplateAsync(string template, IEnumerable<string> emails, object parameters);
}
