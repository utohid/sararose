using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using SaraRose.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "SARA ROSE Nigeria Limited API",
        Version = "v1",
        Description = "Company profile, equipment portfolio and enquiry API."
    });
});

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is missing.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 8_000_000;
});

var allowedOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseStartup");
    await DatabaseStartup.InitializeAsync(db, logger);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("frontend");
app.UseStaticFiles();
app.MapControllers();
app.MapGet("/api/health", () => Results.Ok(new { status = "ok", company = "SARA ROSE NIGERIA LIMITED" }));

app.Run();
