using SecurityExplorer.Abstractions;

namespace SecurityExplorer.Plugin.Demo;

public class HeaderSecurityCheck : IsecurityTest
{
    string IsecurityTest.Name => "Missing Security Headers";

    string IsecurityTest.Category => "Passive";

    Task<TestResult> IsecurityTest.ExecuteAsync(TestContext context)
    {
        return Task.FromResult(new TestResult
        {
            Status = TestStatus.Sucess,
            Title = "Security Headers Present",
            Description = "The target application includes the recommended HTTP security headers. No missing security headers were detected during this demonstration scan.",
            Evidence =
                    @"X-Content-Type-Options: nosniff
                    X-Frame-Options: DENY
                    Content-Security-Policy: default-src 'self'
                    Referrer-Policy: strict-origin-when-cross-origin"
        });
    }
}