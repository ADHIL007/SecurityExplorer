namespace SecurityExplorer.Abstractions;

/// <summary>
/// The complete structured output produced by a single <see cref="ISecurityTest"/> execution.
/// Contains only data — no HTML. The UI layer is solely responsible for rendering.
/// </summary>
public class SecurityTestOutput
{
    /// <summary>
    /// The name of the test that produced this output. Mirrors <see cref="ISecurityTest.Name"/>.
    /// Makes each output self-describing regardless of execution order.
    /// </summary>
    public string TestName { get; set; } = string.Empty;

    /// <summary>
    /// Top-level result: overall status, title, and description.
    /// Always populated.
    /// </summary>
    public TestResult Result { get; set; } = new();

    /// <summary>
    /// Optional summary card for the Dashboard grid.
    /// If null, the framework renders a default card from <see cref="Result"/>.
    /// </summary>
    public TestWidget? Widget { get; set; }

    /// <summary>
    /// Optional structured detail page shown when clicking the plugin from the sidebar.
    /// If null, the plugin does not appear in the sidebar.
    /// </summary>
    public TestPage? Page { get; set; }
}
