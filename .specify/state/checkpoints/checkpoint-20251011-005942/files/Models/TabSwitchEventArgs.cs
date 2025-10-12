using System;

namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// Event arguments for tab switch lifecycle notifications.
/// </summary>
public sealed class TabSwitchEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TabSwitchEventArgs"/> class.
    /// </summary>
    /// <param name="context">The context describing the tab switch.
    /// </param>
    /// <param name="success">Whether the tab switch completed successfully.</param>
    /// <param name="errorMessage">Optional error message when the switch fails.</param>
    public TabSwitchEventArgs(TabSwitchContext context, bool success, string? errorMessage = null)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Success = success;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Gets the context captured for the tab switch lifecycle.
    /// </summary>
    public TabSwitchContext Context { get; }

    /// <summary>
    /// Gets a value indicating whether the tab switch completed successfully.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the error message captured when the tab switch fails.
    /// </summary>
    public string? ErrorMessage { get; }
}
