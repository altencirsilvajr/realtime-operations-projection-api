using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealtimeOperationsProjection.Api;
using RealtimeOperationsProjection.Application;
using RealtimeOperationsProjection.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<OperationExceptionHandler>();
builder.Services.AddDbContext<OperationsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("OperationsDb") ?? "Data Source=operations.db"));
builder.Services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("sqlite");
builder.Services.AddSignalR();
builder.Services.AddCors(options => options.AddPolicy("learning-dashboard", policy => policy
    .WithOrigins("http://localhost:5408")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()));
builder.Services.AddScoped<IOperationStore, SqliteOperationStore>();
builder.Services.AddScoped<OperationService>();
builder.Services.AddScoped<IOperationProjectionPublisher, SignalROperationProjectionPublisher>();

var app = builder.Build();
app.UseExceptionHandler();
app.UseCors("learning-dashboard");
app.Use(async (context, next) =>
{
    var requestId = context.TraceIdentifier;
    app.Logger.LogInformation("Handling {Method} {Path} with {RequestId}", context.Request.Method, context.Request.Path, requestId);
    await next(context);
    app.Logger.LogInformation("Completed {StatusCode} for {RequestId}", context.Response.StatusCode, requestId);
});
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<OperationsDbContext>().Database.EnsureCreatedAsync();
}

var operations = app.MapGroup("/api/operations");
operations.MapPost("", async (CreateOperationRequest request, OperationService service, CancellationToken cancellationToken) =>
{
    var snapshot = await service.RegisterAsync(request.Name, cancellationToken);
    return TypedResults.Created($"/api/operations/{snapshot.Id}", snapshot.ToResponse());
});
operations.MapPost("/{operationId:guid}/transitions", async (Guid operationId, TransitionOperationRequest request, OperationService service, CancellationToken cancellationToken) =>
{
    var snapshot = await service.TransitionAsync(operationId, request.Status, cancellationToken);
    return TypedResults.Ok(snapshot.ToResponse());
});
operations.MapGet("/{operationId:guid}", async (Guid operationId, OperationService service, CancellationToken cancellationToken) =>
{
    var snapshot = await service.GetAsync(operationId, cancellationToken);
    return TypedResults.Ok(snapshot.ToResponse());
});

app.MapHub<OperationsHub>("/hubs/operations");
app.MapHealthChecks("/health");
app.Run();

public partial class Program;
