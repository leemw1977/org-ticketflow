using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetEnv;

namespace OrgTicketflowCli
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // Load .env file (safe to call always — it skips if file is missing)
            // Always resolve .env from project root relative to binary
            var envPath = Path.Combine(AppContext.BaseDirectory, "..", "..", ".env");
            Console.Error.WriteLine($"Loading environment variables from: {envPath}");
            Env.Load(envPath);

            // Get command
            string? command = args.Length > 0 ? args[0].ToLowerInvariant() : null;

            if (string.IsNullOrWhiteSpace(command))
            {
                Console.Error.WriteLine("❌ No command provided. Usage: OrgTicketFlowCLI test-connection");
                return 1;
            }

            // Try to read stdin
            string input = string.Empty;
            if (Console.IsInputRedirected)
            {
                input = await Console.In.ReadToEndAsync();
            }
          

            AuthInput? config = null;

            if (!string.IsNullOrWhiteSpace(input))
            {
                try
                {
                    config = JsonSerializer.Deserialize<AuthInput>(input.Trim());
                }
                catch (JsonException ex)
                {
                    Console.Error.WriteLine($"❌ Failed to parse input JSON: {ex.Message}");
                    return 1;
                }
            }

            // If stdin was empty or failed, fall back to environment variables
            if (config == null || string.IsNullOrEmpty(config.BaseUrl) || string.IsNullOrEmpty(config.Username) || string.IsNullOrEmpty(config.Token))
            {
                config = new AuthInput
                {
                    Username = Environment.GetEnvironmentVariable("JIRA_USERNAME"),
                    Token = Environment.GetEnvironmentVariable("JIRA_TOKEN"),
                    BaseUrl = Environment.GetEnvironmentVariable("JIRA_BASEURL")
                };
            }

            // Validate
            if (string.IsNullOrWhiteSpace(config.Username) ||
                string.IsNullOrWhiteSpace(config.Token) ||
                string.IsNullOrWhiteSpace(config.BaseUrl))
            {
                Console.Error.WriteLine("❌ Missing required input. Provide via stdin JSON or environment variables.");
                return 1;
            }

            // Dispatch
            if (command == "test-connection")
            {
                return await TestConnectionAsync(config);
            }
            else
            {
                Console.Error.WriteLine($"❌ Unknown command: {command}");
                return 1;
            }
        }

        static async Task<int> TestConnectionAsync(AuthInput config)
        {
            using var httpClient = new HttpClient();

            string authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.Username}:{config.Token}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

            string baseUrl = config.BaseUrl ?? string.Empty;
            string endpoint = $"{baseUrl.TrimEnd('/')}/rest/api/3/myself";

            try
            {
                if (baseUrl is null || baseUrl.Length == 0)
                {
                    Console.Error.WriteLine("❌ Base URL is null or empty.");
                    return 1;
                }

                var response = await httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode)
                {
                    Console.Error.WriteLine($"❌ HTTP Error: {response.StatusCode}");
                    return 1;
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine(json); // Send full JSON back to Emacs
                return 0;
            }
            catch (HttpRequestException e)
            {
                Console.Error.WriteLine($"❌ Connection error: {e.Message}");
                return 1;
            }
        }

        class AuthInput
        {
            public string? BaseUrl { get; set; }
            public string? Username { get; set; }
            public string? Token { get; set; }
        }
    }
}
