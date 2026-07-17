using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoListApp.WebApi.Authentication;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Middleware;
using TodoListApp.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "TodoListApp.WebApi", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter the API key as: Bearer {key}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
            },
            Array.Empty<string>()
        },
    });
});

var connectionString = builder.Configuration.GetConnectionString("TodoListDb") ?? "Data Source=todolist.db";
builder.Services.AddDbContext<TodoListDbContext>(options => options.UseSqlite(connectionString));

builder.Services
    .AddAuthentication(ApiKeyAuthenticationOptions.SchemeName)
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationOptions.SchemeName,
        options => options.ApiKey = builder.Configuration["Authentication:ApiKey"] ?? string.Empty);
builder.Services.AddAuthorization();

builder.Services.AddScoped<ITodoListDatabaseService, TodoListDatabaseService>();
builder.Services.AddScoped<ITodoTaskDatabaseService, TodoTaskDatabaseService>();
builder.Services.AddScoped<ITagDatabaseService, TagDatabaseService>();
builder.Services.AddScoped<ICommentDatabaseService, CommentDatabaseService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoListDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// The entry point class, exposed for integration testing.
/// </summary>
public partial class Program
{
    /// <summary>
    /// Prevents direct instantiation while remaining usable as a generic type argument.
    /// </summary>
    protected Program()
    {
    }
}
