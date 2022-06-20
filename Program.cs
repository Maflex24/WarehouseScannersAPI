using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services.Services;
using WarehouseManagerAPI;
using WarehouseManagerAPI.Authentication;
using WarehouseManagerAPI.Entities;
using WarehouseManagerAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPasswordHasher<Employee>, PasswordHasher<Employee>>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();

builder.Services.AddDbContext<WarehouseManagerDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("WarehouseDbConnectionString")));

var authenticationSettings = AuthenticationSettings.NewSettings();
builder.ConfigureToken(authenticationSettings);
builder.Services.AddSingleton(authenticationSettings);


var app = builder.Build();


// ConfigureToken the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
