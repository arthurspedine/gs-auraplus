using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using HealthChecks.UI.Client;
using AuraPlus.Web.Data;
using AuraPlus.Web.Repositories;
using AuraPlus.Web.Services;
using AuraPlus.Web.HealthCheck;
using AuraPlus.Web.Tracing;

var builder = WebApplication.CreateBuilder(args);

const string serviceName = "AuraPlus.Api";

// OpenTelemetry Configuration for Tracing
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(tracing => tracing
        .AddSource(Tracing.Source.Name)
        .AddAspNetCoreInstrumentation(options => 
        {
            options.RecordException = true;
        })
        .AddEntityFrameworkCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(opts =>
        {
            opts.Endpoint = new Uri("http://localhost:4318/v1/traces");
            opts.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
        }));

// Database Configuration - Oracle apenas
builder.Services.AddDbContext<OracleDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// Repository and Service Registration
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEquipeRepository, EquipeRepository>();
builder.Services.AddScoped<IReconhecimentoRepository, ReconhecimentoRepository>();
builder.Services.AddScoped<ISentimentosRepository, SentimentosRepository>();
builder.Services.AddScoped<IRelatorioRepository, RelatorioRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEquipeService, EquipeService>();
builder.Services.AddScoped<IReconhecimentoService, ReconhecimentoService>();
builder.Services.AddScoped<ISentimentosService, SentimentosService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();

// ML.NET Prediction Service (Singleton para reutilizar o modelo)
builder.Services.AddSingleton<MLPredictionService>();

// JWT Authentication Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] 
    ?? throw new InvalidOperationException("JWT SecretKey não configurada no appsettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Em produção, defina como true
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero // Remove o delay padrão de 5 minutos
    };
    
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Health Checks Configuration with custom checks
builder.Services.AddHealthChecks()
    .AddCheck<ApiHealthCheck>("api_health_check")
    .AddDbContextCheck<OracleDbContext>(
        name: "oracle_database",
        tags: new[] { "db", "oracle" });

// Controllers Configuration
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

// Swagger Configuration with JWT Support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "AuraPlus REST API - v1", 
        Version = "v1.0",
        Description = @"API RESTful para gestão de bem-estar e reconhecimento de equipes utilizando .NET 9.

## Versão 1.0

Esta é a versão inicial da API com funcionalidades básicas.

## Domínio de Negócio

O **AuraPlus** é um sistema de gestão de bem-estar e reconhecimento em equipes que permite:
- **Equipes**: Grupos de colaboradores organizados por times
- **Usuários**: Membros das equipes com funções e cargos específicos
- **Sentimentos**: Registro de estados emocionais e pontuações de bem-estar
- **Reconhecimentos**: Sistema de reconhecimento entre membros da equipe
- **Relatórios**: Análises individuais e de equipe sobre bem-estar e desempenho

### Justificativa das Entidades:
1. **Equipe**: Representa times ou departamentos da organização
2. **Users**: Colaboradores com suas informações e vinculação a equipes
3. **Sentimentos**: Rastreamento do estado emocional dos colaboradores
4. **Reconhecimento**: Valorização e feedback positivo entre membros
5. **RelatorioPessoa**: Análise individual de desempenho e bem-estar
6. **RelatorioEquipe**: Visão consolidada do estado da equipe

## Autenticação

Para acessar os endpoints protegidos:
1. Registre-se em `/api/v1/auth/register` ou faça login em `/api/v1/auth/login`
2. Copie o token JWT retornado
3. Clique no botão **Authorize** (🔒) no topo desta página
4. Digite: `{seu_token}`
5. Clique em **Authorize** e feche o modal"
    });

    // Include XML comments for better documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // JWT Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Autenticação JWT - Cole apenas o token (sem 'Bearer').

O Swagger irá adicionar automaticamente o prefixo 'Bearer' ao token.

Exemplo: Cole apenas '12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = Microsoft.AspNetCore.Mvc.Versioning.ApiVersionReader.Combine(
        new Microsoft.AspNetCore.Mvc.Versioning.UrlSegmentApiVersionReader()
    );
});

// API Versioning Configuration
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuraPlus API v1.0");
        c.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

// Health Checks Endpoint with detailed UI response
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpsRedirection();

// Authentication & Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Torna a classe Program acessível para testes
public partial class Program { }
