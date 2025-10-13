namespace MTM_Template_Application.Models.Diagnostics;

/// <summary>
/// Display model for environment variables in Debug Terminal
/// </summary>
public record EnvironmentVariableDisplay(
    string Key,
    string Value,
    bool IsFiltered);
