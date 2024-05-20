// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Interfaces;

/// <summary>
/// Mail settings interface.
/// </summary>
public interface IMailSettings
{
    /// <summary>
    /// Gets or sets the host.
    /// </summary>
    string Host { get; set; }

    /// <summary>
    /// Gets or sets the port.
    /// </summary>
    int Port { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets or sets the mail.
    /// </summary>
    string Mail { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    string Key { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the SSL is enabled.
    /// </summary>
    bool EnableSsl { get; set; }
}
