namespace SecurityExplorer.Abstractions;

/// <summary>
/// Structured data for a plugin's full detail page, shown when selected from the sidebar.
/// The UI renders this data — plugins must NOT produce raw HTML.
/// </summary>
public class TestPage
{
    /// <summary>
    /// Main heading of the detail page (e.g., "Rate Limiter — Detailed Report").
    /// Falls back to <see cref="TestResult.Title"/> if left empty.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Ordered list of descriptive content sections (e.g., "How It Works", "Evidence", "Recommendations").
    /// </summary>
    public List<TestPageSection> Sections { get; set; } = new();

    /// <summary>
    /// Tabular findings for the detail page findings table.
    /// Each row represents one discrete observation with a label, value, and severity.
    /// </summary>
    public List<Finding> RawFindings { get; set; } = new();
}
