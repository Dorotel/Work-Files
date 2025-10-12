using System;

namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// Event arguments raised when overlay visibility changes.
/// </summary>
public sealed class OverlayVisibilityChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OverlayVisibilityChangedEventArgs"/> class.
    /// </summary>
    /// <param name="overlayId">The identifier for the overlay whose visibility changed.</param>
    /// <param name="isVisible">Indicates whether the overlay is now visible.</param>
    /// <param name="priority">The priority assigned to the overlay.</param>
    public OverlayVisibilityChangedEventArgs(string overlayId, bool isVisible, OverlayPriority priority)
    {
        if (string.IsNullOrWhiteSpace(overlayId))
        {
            throw new ArgumentException("Overlay identifier cannot be null or whitespace.", nameof(overlayId));
        }

        OverlayId = overlayId;
        IsVisible = isVisible;
        Priority = priority;
    }

    /// <summary>
    /// Gets the identifier of the overlay whose visibility changed.
    /// </summary>
    public string OverlayId { get; }

    /// <summary>
    /// Gets a value indicating whether the overlay is currently visible.
    /// </summary>
    public bool IsVisible { get; }

    /// <summary>
    /// Gets the priority that was assigned to the overlay at the time of the change.
    /// </summary>
    public OverlayPriority Priority { get; }
}
