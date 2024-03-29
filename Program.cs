using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NLog.Web;
using WarehouseScannersAPI.Authentication;
using WarehouseScannersAPI.Entities;
using WarehouseScannersAPI.Middleware;
using WarehouseScannersAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<ResponseTimeMiddleware>();
builder.Services.AddScoped<DataGenerator>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEmployeeContextService, EmployeeContextService>();

builder.Services.AddDbContext<WarehouseScannersDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("WarehouseDbConnectionString")));

var authenticationSettings = AuthenticationSettings.NewSettings();
builder.ConfigureToken(authenticationSettings);
builder.Services.AddSingleton(authenticationSettings);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Picker", policy => policy.RequireClaim("picking"));
    options.AddPolicy("Inbound", policy => policy.RequireClaim("inbound"));
});

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
app.UseMiddleware<ResponseTimeMiddleware>();

app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
