using Ambulance.Api.Extensions;
using Ambulance.Api.MiddleWear;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers( option =>
{
    option.SuppressAsyncSuffixInActionNames = false;
}).AddJsonOptions(option =>
{
    option.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
});

//Service Registration 
builder.Services.AddApplicationServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.Configuration(builder);

app.Run();
