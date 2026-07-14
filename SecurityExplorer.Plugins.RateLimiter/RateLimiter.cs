using Microsoft.Extensions.Logging;
using SecurityExplorer.Abstractions;

namespace SecurityExplorer.Plugins.RateLimiter;

/// <summary>
/// Active security test that probes the target for HTTP rate-limiting behaviour.
/// Sends a rapid burst of requests and inspects response codes to determine
/// whether the server enforces any throttling (HTTP 429 Too Many Requests).
/// </summary>
public class RateLimiterTest : ISecurityTest
{
    private const int BurstCount  = 30;
    private const string ProbeUrl = "/"; // lightweight endpoint to probe

    public string Name     => "Rate Limiter";
    public string Category => TestType.Active.ToString();

    public async Task<SecurityTestOutput> ExecuteAsync(TestContext context)
    {
        int sent    = 0;
        int blocked = 0;   // HTTP 429
        int errors  = 0;   // network / unexpected failures
        var findings = new List<Finding>();

        for (int i = 0; i < BurstCount; i++)
        {
            try
            {
                var response = await context.HttpClient.GetAsync(ProbeUrl);
                sent++;

                var statusCode = (int)response.StatusCode;

                if (statusCode == 429)
                {
                    blocked++;
                    findings.Add(new Finding
                    {
                        Label    = $"Request #{i + 1}",
                        Value    = $"HTTP {statusCode} — Too Many Requests (blocked)",
                        Severity = FindingSeverity.Info
                    });
                }
                else if (statusCode >= 500)
                {
                    errors++;
                    findings.Add(new Finding
                    {
                        Label    = $"Request #{i + 1}",
                        Value    = $"HTTP {statusCode} — Server error",
                        Severity = FindingSeverity.Medium
                    });
                }
            }
            catch (Exception ex)
            {
                errors++;
                context.Logger.LogWarning(ex, "Rate limiter probe request #{Count} failed.", i + 1);
            }
        }

        bool rateLimitDetected = blocked > 0;
        var overallStatus = rateLimitDetected ? TestStatus.Success : TestStatus.Warning;
        var summary       = rateLimitDetected
            ? $"Rate limiting is enforced — {blocked}/{sent} requests were throttled (HTTP 429)."
            : $"No rate limiting detected — all {sent} burst requests were accepted.";

        // Add an overall finding row summarising the burst
        findings.Insert(0, new Finding
        {
            Label    = "Rate Limiting Detected",
            Value    = rateLimitDetected ? "Yes" : "No",
            Severity = rateLimitDetected ? FindingSeverity.Info : FindingSeverity.High
        });

        return new SecurityTestOutput
        {
            Result = new TestResult
            {
                Status      = overallStatus,
                Type        = TestType.Active,
                Title       = "Rate Limiter",
                Description = summary
            },

            Widget = new TestWidget
            {
                Summary    = summary,
                BadgeLabel = rateLimitDetected ? "PROTECTED" : "UNPROTECTED",
                Metrics    = new Dictionary<string, string>
                {
                    ["Requests Sent"]    = sent.ToString(),
                    ["Requests Blocked"] = blocked.ToString(),
                    ["Errors"]           = errors.ToString()
                }
            },

            Page = new TestPage
            {
                Title = "Rate Limiter — Detailed Report",
                Sections =
                [
                    new TestPageSection
                    {
                        Heading = "How the Test Works",
                        Content = $"This test sends a rapid burst of {BurstCount} HTTP GET requests " +
                                  $"to the '{ProbeUrl}' endpoint in quick succession. " +
                                  "A well-configured server should respond with HTTP 429 (Too Many Requests) " +
                                  "once the threshold is exceeded, demonstrating effective rate limiting."
                    },
                    new TestPageSection
                    {
                        Heading = "Result Summary",
                        Content = summary
                    },
                    new TestPageSection
                    {
                        Heading = "Recommendation",
                        Content = rateLimitDetected
                            ? "Rate limiting is active. Verify the threshold is appropriate for your traffic profile " +
                              "and that rate-limit bypass headers (X-Forwarded-For, X-Real-IP) cannot be spoofed."
                            : "No rate limiting was detected. Consider implementing rate limiting middleware " +
                              "(e.g., ASP.NET Core RateLimiter, Nginx limit_req) to protect against brute force " +
                              "and denial-of-service attacks."
                    }
                ],
                RawFindings = findings
            }
        };
    }
}
