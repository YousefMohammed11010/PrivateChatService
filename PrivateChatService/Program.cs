using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PrivateChatService.Data;
using PrivateChatService.Hubs;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using PrivateChatService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("RemoteConnection"); 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string not found"))
           .EnableSensitiveDataLogging() 
           .EnableDetailedErrors());

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string not found"))
           .EnableSensitiveDataLogging() 
           .EnableDetailedErrors());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = builder.Environment.IsProduction(),
            ValidateAudience = builder.Environment.IsProduction(),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"], 
            ValidAudience = builder.Configuration["Jwt:Issuer"], 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration")))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

//builder.Services.AddIdentity<AspNetUser,AspNetRole>().AddEntityFrameworkStores<AppDbContext>();


builder.Services.AddSignalR();

builder.Services.AddSingleton<IUserIdProvider, EmailBasedUserIdProvider>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://yourfrontend.com", "http://localhost:3000") 
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Information);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();