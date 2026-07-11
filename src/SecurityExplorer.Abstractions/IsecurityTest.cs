namespace SecurityExplorer.Abstractions;

/// <summary>
/// Represents a single security test or check.
/// Can be an active attack (SQLi) or a passive check (Log config).
/// </summary>
public interface IsecurityTest
{
    /// <summary>
    /// Unique name displayed in the dashboard (e.g., "SQL Injection", "Log Monitor")
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Category for grouping in the UI (e.g., "Injection", "Configuration", "Auth")
    /// </summary>
    string Category { get; }
    /// <summary>
    /// The core logic executed by the engine.
    /// </summary>
    Task<TestResult> ExecuteAsync(TestContext context);

}
