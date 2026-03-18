using AuthService.DAL.AuthDal;
using AuthService.Services.AuthService;
using AuthService.Services.VideoService;
using Microsoft.OpenApi;
using Npgsql;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Auth Service API",
        Version = "v1",
        Description = "API for user authentication and registration"
    });

    // 🔐 Add JWT Authentication
    var bearerScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer YOUR_TOKEN_HERE'"
    };

    options.AddSecurityDefinition("Bearer", bearerScheme);

    options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});

// Register database connection
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var connection = new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
    return connection;
});
// Register DAL and Service
builder.Services.AddScoped<IAuthDal, AuthDal>();
builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService.AuthService>();
builder.Services.AddScoped<IVideoService, VideoService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
