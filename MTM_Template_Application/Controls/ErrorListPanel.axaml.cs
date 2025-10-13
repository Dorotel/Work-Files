using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MTM_Template_Application.Controls;

/// <summary>
/// A panel control that displays a list of error entries with severity icons and timestamps.
/// </summary>
public class ErrorListPanel : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Errors"/> property.
    /// </summary>
    public static readonly StyledProperty<IEnumerable<ErrorEntry>?> ErrorsProperty =
        AvaloniaProperty.Register<ErrorListPanel, IEnumerable<ErrorEntry>?>(
            nameof(Errors),
            defaultValue: null);

    /// <summary>
    /// Gets or sets the collection of error entries to display.
    /// </summary>
    public IEnumerable<ErrorEntry>? Errors
    {
        get => GetValue(ErrorsProperty);
        set => SetValue(ErrorsProperty, value);
    }

    public ErrorListPanel()
    {
        Errors = new ObservableCollection<ErrorEntry>();
    }
}

/// <summary>
/// Represents an error entry for display in ErrorListPanel.
/// </summary>
public class ErrorEntry
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error severity level.
    /// </summary>
    public ErrorSeverity Severity { get; set; } = ErrorSeverity.Error;

    /// <summary>
    /// Gets or sets the timestamp when the error occurred.
    /// </summary>
    public System.DateTime Timestamp { get; set; } = System.DateTime.Now;

    /// <summary>
    /// Gets the severity icon based on the severity level.
    /// </summary>
    public string SeverityIcon => Severity switch
    {
        ErrorSeverity.Critical => "🔴",
        ErrorSeverity.Error => "⚠️",
        ErrorSeverity.Warning => "⚡",
        ErrorSeverity.Info => "ℹ️",
        _ => "•"
    };
}

/// <summary>
/// Represents the severity level of an error.
/// </summary>
public enum ErrorSeverity
{
    /// <summary>Informational message.</summary>
    Info,
    /// <summary>Warning that doesn't prevent operation.</summary>
    Warning,
    /// <summary>Error that affects functionality.</summary>
    Error,
    /// <summary>Critical error requiring immediate attention.</summary>
    Critical
}
