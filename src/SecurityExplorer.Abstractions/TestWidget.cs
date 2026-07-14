namespace SecurityExplorer.Abstractions;

/// <summary>
/// Structured data for a plugin's card in the main Dashboard grid.
/// The UI renders this data — plugins must NOT produce raw HTML.
/// </summary>
public class TestWidget
{
    /// <summary>
    /// One-line summary displayed prominently on the dashboard card.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Key-value stats displayed as metric blocks on the card.
    /// Example: { "Requests Sent", "100" }, { "Requests Blocked", "80" }
    /// </summary>
    public Dictionary<string, string> Metrics { get; set; } = new();

    /// <summary>
    /// Optional short badge label shown on the card (e.g., "RATE LIMITED", "VULNERABLE").
    /// </summary>
    public string? BadgeLabel { get; set; }
}
