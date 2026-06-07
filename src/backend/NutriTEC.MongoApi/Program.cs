const string CorsPolicyName = "NutriTECMongoCors";

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

// TODO: Configurar MongoDB cuando existan documentos y colecciones del dominio.

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
    service = "NutriTEC MongoDB API",
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
            ["title"] = "NutriTEC MongoDB API",
            ["version"] = "v1"
        },
        ["paths"] = new Dictionary<string, object>
        {
            ["/health"] = CreateHealthPath("Verifica el estado de la API de MongoDB."),
            ["/api/health"] = CreateHealthPath("Verifica el estado de la API de MongoDB desde controlador.")
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
