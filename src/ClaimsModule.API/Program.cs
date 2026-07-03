using ClaimsModule.API.Middleware;
using ClaimsModule.Application;
using ClaimsModule.Infrastructure;
using ClaimsModule.Persistence;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ValidationExceptionMiddleware>();

app.MapControllers();
app.MapGet("/health", () => Results.Text("OK", "text/plain"))
    .WithName("HealthCheck")
    .Produces<string>(StatusCodes.Status200OK);

app.Run();
