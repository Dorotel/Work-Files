namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// Represents the priority levels that dictate overlay display ordering.
/// </summary>
public enum OverlayPriority
{
    /// <summary>
    /// Standard overlay priority. Overlays at this level wait for currently visible overlays to dismiss.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// High priority overlays can preempt normal overlays.
    /// </summary>
    High = 1,

    /// <summary>
    /// Critical overlays interrupt any visible overlay and display immediately.
    /// </summary>
    Critical = 2,
}
