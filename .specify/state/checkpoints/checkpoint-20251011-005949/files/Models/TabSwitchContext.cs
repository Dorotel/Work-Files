using System;
using System.Collections.Generic;
using System.Threading;

namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// Carries context information that flows through the tab switching pipeline.
/// </summary>
public sealed class TabSwitchContext
{
    /// <summary>
    /// Gets or sets the name of the tab that the application is switching from.
    /// </summary>
    public string FromTab { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the tab that the application is switching to.
    /// </summary>
    public string ToTab { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp representing when the tab switch started.
    /// </summary>
    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the identifier of the user that initiated the tab switch.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets additional data that pipeline steps can share while the switch executes.
    /// </summary>
    public IDictionary<string, object?> Data { get; } = new Dictionary<string, object?>(StringComparer.Ordinal);

    /// <summary>
    /// Gets or sets the cancellation token for the active tab switch operation.
    /// </summary>
    public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
}
