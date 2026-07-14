namespace SecurityExplorer.Abstractions;

/// <summary>
/// Represents a single discrete finding surfaced by a security test.
/// Used to populate the findings table on the plugin detail page.
/// </summary>
public class Finding
{
    /// <summary>
    /// Short label identifying what was found (e.g., "Endpoint", "Response Code", "Attack Vector").
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// The value or detail for this finding (e.g., "/api/login", "429", "Repeated POST").
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Severity classification of this specific finding.
    /// </summary>
    public FindingSeverity Severity { get; set; } = FindingSeverity.Info;
}
