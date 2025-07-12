using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public static class StatusHandler
{
    public static async Task<IResult> GetStatusAsync()
    {
        var config = new AuthInput
        {
            Username = Environment.GetEnvironmentVariable("JIRA_USERNAME"),
            Token = Environment.GetEnvironmentVariable("JIRA_TOKEN"),
            BaseUrl = Environment.GetEnvironmentVariable("JIRA_BASEURL")
        };

        var httpClient = new HttpClient();

        try
        {
            if (string.IsNullOrWhiteSpace(config.BaseUrl))
            {
                var error = ApiResponseFactory.Error<StatusCheckResult>(
                    "Base URL is required", 400
                );
                return Results.Json(error, statusCode: 400);
            }

            string authHeader = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{config.Username}:{config.Token}")
            );

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", authHeader);

            string endpoint = $"{config.BaseUrl.TrimEnd('/')}/rest/api/3/myself";
            var response = await httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                var error = ApiResponseFactory.Error<StatusCheckResult>(
                    $"Jira API responded with {response.StatusCode}: {response.ReasonPhrase}",
                    (int)response.StatusCode
                );
                return Results.Json(error, statusCode: (int)response.StatusCode);
            }

            var json = await response.Content.ReadAsStringAsync();

            var userInfo = JsonSerializer.Deserialize<JiraUserInfo>(json);

            var result = new StatusCheckResult
            {
                JiraUserDisplayName = userInfo?.displayName,
                AccountId = userInfo?.accountId,
            };

            var ok = ApiResponseFactory.Ok(result, "Connected to Jira", (int)response.StatusCode);
            return Results.Json(ok, statusCode: (int)response.StatusCode);
        }
        catch (HttpRequestException e)
        {
            var error = ApiResponseFactory.Error<StatusCheckResult>(
                $"Connection error: {e.Message}", 500
            );
            return Results.Json(error, statusCode: 500);
        }
        catch (Exception e)
        {
            var error = ApiResponseFactory.Error<StatusCheckResult>(
                $"Unexpected error: {e.Message}", 500
            );
            return Results.Json(error, statusCode: 500);
        }
    }
}
