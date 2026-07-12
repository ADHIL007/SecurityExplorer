namespace SecurityExplorer.Abstractions;

public class SecurityTestOutput
{
    public TestResult Result { get; set; } = new();
    
    public TestWidget? Widget { get; set; }
    
    public TestPage? Page { get; set; }
}
