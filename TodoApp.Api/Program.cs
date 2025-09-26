
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TodoApp.Core.Identity;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Services;
using Serilog;
using TodoApp.Application.Interfaces.RepoInterfaces;
using TodoApp.Application.Interfaces.ServicesInterfaces;
using FluentValidation;
using Microsoft.OpenApi.Models;
using TodoApp.Application.Validators;
using FluentValidation.AspNetCore;
using TodoApp.Application.Mapping;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.OpenApi.Any;
using TodoApp.Domain.Entities;
using System.Text.Json.Serialization;
using TodoApp.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authentication;
using TodoApp.Core.Integration;


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)
    ));
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor();

// JWT
if (builder.Environment.EnvironmentName == "IntegrationTests")
{
    builder.Services.AddAuthentication("Test")
        .AddScheme<AuthenticationSchemeOptions, CustomAuthHandler>("Test", options => { });
    builder.Services.AddAuthorization();
}
else
{
    var jwtKey = config["Jwt:Key"] ?? throw new Exception("Jwt:Key not set");
    var key = Encoding.UTF8.GetBytes(jwtKey);
    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "name",
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
}



builder.Services.AddAuthorization();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();


builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<ICategoryService, CategoryService > ();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<IUserService, UserService>();


builder.Services.AddScoped<IInvitationRepository, InvitationRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();


builder.Services.Configure<SmtpSettings>(config.GetSection("Smtp"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<SmtpSettings>>().Value);

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers()
    .AddMvcOptions(options =>
    {
        options.ModelValidatorProviders.Clear();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateInvitationDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateTodoDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateTodoDtoValidator>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Description = "Enter 'Bearer' followed by your token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });

    options.MapType<Priority>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetValues(typeof(Priority))
            .Cast<Priority>()
            .Select(e => (IOpenApiAny)new OpenApiString(e.ToString())) 
            .ToList()
    });
});




Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/todoapp.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddScoped<DataSeeder>();


var app = builder.Build();
if (!app.Environment.IsEnvironment("IntegrationTests"))
{

    using (var scope = app.Services.CreateScope())
    {
        var sp = scope.ServiceProvider;
        var ctx = sp.GetRequiredService<AppDbContext>();
        var logger = sp.GetRequiredService<ILogger<DataSeeder>>();
        ctx.Database.Migrate();


        await DataSeeder.SeedAsync(sp, logger);
    }
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
public partial class Program { }
