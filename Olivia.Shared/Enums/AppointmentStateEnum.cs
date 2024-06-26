// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Shared.Enums;
using System.ComponentModel;

/// <summary>
/// Represents the state of an appointment.
/// </summary>
public enum AppointmentStateEnum
{
    /// <summary>
    /// Pending approval.
    /// </summary>
    [Description("Pending approval")]
    PendingApproval = 1,

    /// <summary>
    /// Pending payment.
    /// </summary>
    [Description("Pending payment")]
    PendingPayment = 2,

    /// <summary>
    /// Confirmed.
    /// </summary>
    [Description("Confirmed")]
    Confirmed = 3,

    /// <summary>
    /// Rejected.
    /// </summary>
    /// [Description("Rejected")]
    Rejected = 4,
}
