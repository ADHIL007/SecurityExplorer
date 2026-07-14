using Microsoft.Extensions.Logging;
using SecurityExplorer.Abstractions;

namespace SecurityExplorer.Core;

public class SecurityTestRunner
{
    private readonly IEnumerable<ISecurityTest> _secTests;
    private readonly ILogger<SecurityTestRunner> _logger;

    public SecurityTestRunner(IEnumerable<ISecurityTest> secTests, ILogger<SecurityTestRunner> logger)
    {
        _secTests = secTests;
        _logger = logger;
    }

    public async Task<List<SecurityTestOutput>> ExecuteAllTestsAsync(string baseAddress)
    {
        var results = new List<SecurityTestOutput>();
        using var httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };

        foreach (var test in _secTests)
        {
            _logger.LogInformation("Running test: {TestName}", test.Name);

            try
            {
                var context = new TestContext
                {
                    HttpClient = httpClient,
                    BaseAddress = baseAddress,
                    Logger = _logger
                };

                var output = await test.ExecuteAsync(context);

                // Ensure TestName is always stamped — plugins don't need to set this.
                output.TestName = test.Name;

                results.Add(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test '{TestName}' threw an unhandled exception.", test.Name);

                // One failing plugin must not abort the rest of the suite.
                results.Add(new SecurityTestOutput
                {
                    TestName = test.Name,
                    Result = new TestResult
                    {
                        Status    = TestStatus.Error,
                        Title     = test.Name,
                        Description = $"Unhandled exception: {ex.Message}",
                        Evidence  = ex.StackTrace
                    }
                });
            }
        }

        return results;
    }
}