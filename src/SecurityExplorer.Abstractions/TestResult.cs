namespace SecurityExplorer.Abstractions;

public enum TestStatus 
{ 
    Success, 
    Failed, 
    Warning, 
    Error 
}

public enum TestType 
{ 
    Active, 
    Passive 
}

public class TestResult
{
    public TestStatus Status { get; set; } = TestStatus.Success;
    
    public TestType Type { get; set; } = TestType.Active;

    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;

    public string? Evidence { get; set; }
}