var builder = WebApplication.CreateBuilder(args);

var envPath = Environment.GetEnvironmentVariable("ORG_TICKETFLOW_ENV_PATH")
    ?? Path.Combine(Directory.GetCurrentDirectory(), ".env");

DotNetEnv.Env.Load(envPath);    // Load environment variables from .env file
var port = Environment.GetEnvironmentVariable("PORT") ?? "7070";
var environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development";


builder.Environment.EnvironmentName = environment;  
var app = builder.Build();

// Set the port for the application
app.Urls.Clear(); // Clear any default URLs
app.Urls.Add($"http://127.0.0.1:{port}");   



app.MapGet("/", () => "Hello World!");



// Simple GET /status endpoint
app.MapGet("/status", StatusHandler.GetStatusAsync);
app.Run();
