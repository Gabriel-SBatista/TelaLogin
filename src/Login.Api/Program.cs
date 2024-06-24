using Login.Api.Middlewares;
using Login.Cross.DependencyInjection;
using Login.Data.Context;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSqlServer(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddValidators();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<ErrorMiddleware>();

app.MapControllers();

app.Run();
