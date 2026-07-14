namespace SecurityExplorer.Abstractions;

/// <summary>
/// The overall outcome of a security test run.
/// </summary>
public enum TestStatus 
{ 
    Success, 
    Failed, 
    Warning, 
    Error 
}

/// <summary>
/// Whether the test actively probes the target or passively inspects configuration.
/// </summary>
public enum TestType 
{ 
    Active, 
    Passive 
}

/// <summary>
/// The severity of an individual finding within a test's detail report.
/// Separate from <see cref="TestStatus"/> — a passing test can still expose informational findings.
/// </summary>
public enum FindingSeverity
{
    Critical,
    High,
    Medium,
    Low,
    Info
}

/// <summary>
/// The top-level result summary for a security test.
/// </summary>
public class TestResult
{
    /// <summary>Overall pass/fail/warn/error outcome.</summary>
    public TestStatus Status { get; set; } = TestStatus.Success;
    
    /// <summary>Whether this test actively attacks or passively inspects.</summary>
    public TestType Type { get; set; } = TestType.Active;

    /// <summary>Short display title (e.g., "Rate Limiter").</summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>Human-readable summary of what the test found.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Optional raw evidence string (stack trace, HTTP response, etc.).</summary>
    public string? Evidence { get; set; }
}