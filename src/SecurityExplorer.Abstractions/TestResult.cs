
namespace SecurityExplorer.Abstractions;

public enum TestStatus { Sucess, Failed, Warning, Error }

public class TestResult
{
    public TestStatus Status { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string? Evidence { get; set; }


}