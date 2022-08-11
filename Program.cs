using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WarehouseManagerAPI;
using WarehouseManagerAPI.Authentication;
using WarehouseManagerAPI.Entities;
using WarehouseManagerAPI.Middleware;
using WarehouseManagerAPI.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<DataGenerator>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEmployeeContextService, EmployeeContextService>();


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

using var scope = app.Services.CreateScope();
var dataGenerator = scope.ServiceProvider.GetService<DataGenerator>();
//await dataGenerator.GeneratePermissions();
//await dataGenerator.AddProducts();
//await dataGenerator.AddOrders();
//await dataGenerator.GenerateOrderPositions();
//await dataGenerator.CreateStorages();
//await dataGenerator.AddStorageContent();
//await dataGenerator.CreatePalletsWithContent(4, "Euro8010", 2200, 800);
//await dataGenerator.CreatePalletsWithContent(4, "EuroLow8010", 650, 800);
//await dataGenerator.CreatePalletsWithContent(4, "Block1010", 2200, 1000);

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
