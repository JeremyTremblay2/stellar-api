using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StellarApi.Business;
using StellarApi.Infrastructure.Business;
using StellarApi.Infrastructure.Repository;
using StellarApi.Repository.Context;
using StellarApi.Repository.Repositories;
using StellarApi.RestApi.Auth;
using System.Text;
using System.Text.Json.Serialization;

/// <summary>
/// Represents the main entry point for the application.
/// </summary>
public static partial class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    /// <param name="args">The arguments for the application.</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);
        ConfigureSwagger(builder);
        ConfigureAuthentication(builder);
        ConfigureLogging(builder);

        var app = builder.Build();

        ConfigureMiddleware(app);

        app.Run();
    }

    /// <summary>
    /// Configures the services for the application.
    /// <param name="builder">The builder for the application.</param>
    /// </summary>
    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers()
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"));
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        builder.Services.AddDbContext<SpaceDbContextSeed>(options =>
            options.UseSqlite(builder.Configuration["ConnectionStrings:DatabaseLocalPath"])
        );

        builder.Services.AddScoped<SpaceDbContext, SpaceDbContextSeed>();

        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<ICelestialObjectService, CelestialObjectService>();
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.AddScoped<ICelestialObjectRepository, CelestialObjectRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);
    }

    /// <summary>
    /// Configures the swagger for the application.
    /// <param name="builder">The builder for the application.</param>
    /// </summary>
    private static void ConfigureSwagger(WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Stellar API", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[]{}
            }
        });
        });
    }

    /// <summary>
    /// Configures the authentication for the application.
    /// <param name="builder">The builder for the application.</param>
    /// </summary>
    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        var validIssuer = builder.Configuration.GetValue<string>("JwtTokenSettings:ValidIssuer");
        var validAudience = builder.Configuration.GetValue<string>("JwtTokenSettings:ValidAudience");
        var symmetricSecurityKey = builder.Configuration.GetValue<string>("JwtTokenSettings:SymmetricSecurityKey");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.IncludeErrorDetails = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = validIssuer,
                ValidAudience = validAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(symmetricSecurityKey))
            };
        });
    }

    /// <summary>
    /// Configures the logging for the application.
    /// </summary>
    /// <param name="builder">The builder for the application.</param>
    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
        }
        else
        {
            builder.Logging.AddApplicationInsights(
            configureTelemetryConfiguration: (config) =>
                config.ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"],
                configureApplicationInsightsLoggerOptions: (options) => { }
            );
        }

        builder.Logging.AddFilter("", LogLevel.Trace);
    }

    /// <summary>
    /// Configures the middleware for the application.
    /// </summary>
    /// <param name="app">The application to configure.</param>
    static void ConfigureMiddleware(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthorization();

        app.MapControllers();
        app.MapGet("/", () => "Hello World !");
    }
}

