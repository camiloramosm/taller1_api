using AspNetCoreRateLimit;
using TodoList.Modules.Todos.Infrastructure;
using TodoList.Modules.Todos.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddApplicationPart(typeof(TodoList.Modules.Todos.Presentation.Controllers.TodoItemsController).Assembly);
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TodoList API",
        Version = "v1",
        Description = "API para gestión de listas de tareas con ASP.NET Core 9 y Supabase"
    });
});

// Add Todos Module
builder.Services.AddTodosModule(builder.Configuration);

// Add Health Checks
builder.Services.AddHealthChecks();

// Add Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware de manejo de excepciones global
app.UseExceptionHandling();

// Rate Limiting
app.UseIpRateLimiting();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoList API v1");
    });
}

// Health Checks endpoint
app.MapHealthChecks("/health");

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

// Verificar conexión a Supabase
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var supabaseClient = services.GetRequiredService<Supabase.Client>();
        await supabaseClient.InitializeAsync();
        
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("✅ Conexión a Supabase establecida correctamente usando SDK");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ Error al conectar con Supabase. Verifica la configuración en appsettings.json");
    }
}

app.Run();

