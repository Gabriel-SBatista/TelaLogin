using Login.Api.Middlewares;
using Login.Core.Services.RabbitMQServices;
using Login.Cross.DependencyInjection;
using Login.Cross.HealthCheck;
using Login.Cross.Logging;
using Login.Cross.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       ValidateAudience = false,
                       ValidateIssuer = false,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTToken:StringKey"])),
                       ClockSkew = TimeSpan.Zero
                   };
               });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
.AddMonitoredItems(builder.Configuration);

builder.Services.AddSqlServer(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddValidators();
builder.Services.AddCustomSwagger();

var app = builder.Build();

app.UseCors(op =>
{
    op.AllowAnyOrigin();
    op.AllowAnyMethod();
    op.AllowAnyHeader();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var loggerFactory = builder.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
app.UseCustomLog(loggerFactory, builder.Configuration);

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ErrorMiddleware>();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
