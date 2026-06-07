const string CorsPolicyName = "NutriTECCors";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// TODO: Configurar SQL Server y Entity Framework cuando existan entidades del dominio.

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGet("/openapi/v1.json", () => Results.Json(CreateOpenApiDocument()));
    app.MapGet("/swagger/v1/swagger.json", () => Results.Json(CreateOpenApiDocument()));
}

app.UseHttpsRedirection();
app.UseCors(CorsPolicyName);

app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "NutriTEC SQL Server API",
    timestamp = DateTimeOffset.UtcNow
}));

app.MapControllers();

app.Run();

static object CreateOpenApiDocument()
{
    // Documento minimo para exponer OpenAPI sin agregar paquetes externos todavia.
    return new Dictionary<string, object>
    {
        ["openapi"] = "3.0.1",
        ["info"] = new Dictionary<string, object>
        {
            ["title"] = "NutriTEC SQL Server API",
            ["version"] = "v1"
        },
        ["paths"] = new Dictionary<string, object>
        {
            ["/health"] = CreateHealthPath("Verifica el estado de la API de SQL Server."),
            ["/api/health"] = CreateHealthPath("Verifica el estado de la API de SQL Server desde controlador.")
        }
    };
}

static object CreateHealthPath(string summary)
{
    return new Dictionary<string, object>
    {
        ["get"] = new Dictionary<string, object>
        {
            ["summary"] = summary,
            ["responses"] = new Dictionary<string, object>
            {
                ["200"] = new Dictionary<string, object>
                {
                    ["description"] = "API disponible"
                }
            }
        }
    };
}
