using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<DataGenerator>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEmployeeContextService, EmployeeContextService>();


builder.Services.AddDbContext<WarehouseManagerDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("WarehouseDbConnectionString")));

var authenticationSettings = AuthenticationSettings.NewSettings();
builder.ConfigureToken(authenticationSettings);
builder.Services.AddSingleton(authenticationSettings);
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


var app = builder.Build();


// ConfigureToken the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope();
var dataGenerator = scope.ServiceProvider.GetService<DataGenerator>();
await dataGenerator.Seeder();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
