using Microsoft.Extensions.Logging;

namespace SecurityExplorer.Abstractions;

public class TestContext
{

    public HttpClient HttpClient { get; set; } = null!;
    public string BaseAddress { get; set; } = string.Empty;
    public ILogger Logger { get; set; } = null!;

}
