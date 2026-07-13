using System.Linq.Expressions;
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


    public async Task<List<SecurityTestOutput>> ExecuteAllTestsAsync(string BaseAddress)
    {
        var Results = new List<SecurityTestOutput>();
        using var HttpClient = new HttpClient { BaseAddress = new Uri(BaseAddress) };
        try
        {

            foreach (var test in _secTests)
            {
                TestContext context = new TestContext
                {

                    HttpClient = HttpClient,
                    BaseAddress = BaseAddress,
                    Logger = _logger

                };

                _logger.LogInformation("Running Test : {TestName}", test.Name);

                var Result = await test.ExecuteAsync(context);

                Results.Add(Result);


            }
        }
        catch (Exception ex)
        {

            _logger.LogError(ex.Message);

            Results.Add(new SecurityTestOutput
            {
                Result = new TestResult
                {
                    Status = TestStatus.Error,
                    Title = "Exception Occured",
                    Description = ex.Message,
                    Evidence = ex.StackTrace
                }
            });

        }


        return Results;
    }
}