using ClaimsModule.Application;
using ClaimsModule.Infrastructure;
using ClaimsModule.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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

app.MapControllers();
app.MapGet("/health", () => Results.Text("OK", "text/plain"))
    .WithName("HealthCheck")
    .Produces<string>(StatusCodes.Status200OK);

app.Run();
