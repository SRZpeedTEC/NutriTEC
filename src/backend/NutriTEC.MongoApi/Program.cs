using NutriTEC.MongoApplication.Extensions;
using NutriTEC.MongoInfrastructure.Extensions;
using NutriTEC.MongoApi.Middlewares;

const string CorsPolicyName = "NutriTECMongoCors";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMongoApplicationServices();
builder.Services.AddMongoInfrastructureServices(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors(CorsPolicyName);

app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "NutriTEC MongoDB API",
    timestamp = DateTimeOffset.UtcNow
}));

app.MapControllers();

app.Run();
