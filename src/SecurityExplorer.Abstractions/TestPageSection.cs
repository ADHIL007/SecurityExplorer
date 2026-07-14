namespace SecurityExplorer.Abstractions;

/// <summary>
/// A titled content section displayed on a plugin's detail page.
/// The UI layer is responsible for rendering these into HTML — plugins supply only data.
/// </summary>
public class TestPageSection
{
    /// <summary>
    /// Section heading displayed as a sub-title (e.g., "How the Test Works", "Evidence").
    /// </summary>
    public string Heading { get; set; } = string.Empty;

    /// <summary>
    /// Descriptive text content for this section. Supports plain text.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// When true, <see cref="Content"/> is rendered in a monospace code/pre block (e.g., for HTTP dumps, stack traces).
    /// </summary>
    public bool IsCode { get; set; } = false;
}
