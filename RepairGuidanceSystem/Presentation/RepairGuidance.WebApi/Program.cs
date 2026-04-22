using Microsoft.OpenApi.Models;
using Polly;
using RepairGuidance.Application.DependencyResolvers;
using RepairGuidance.Application.Managers;
using RepairGuidance.Infrastructure.ExternalServices;
using RepairGuidance.InnerInfrastructure.DependencyResolvers;
using RepairGuidance.Persistence.DependencyResolvers;
using RepairGuidance.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthenticationService(builder.Configuration);
builder.Services.AddDbContextService(builder.Configuration);
builder.Services.AddLoggerService(builder.Configuration);
builder.Services.AddValidatorServices();
builder.Services.AddRepositoryServices();
builder.Services.AddManagerServices();
builder.Services.AddMapperService();
builder.Services.AddHttpClient<IAiService, GroqAiService>()
    .AddTransientHttpErrorPolicy(policy =>
        policy.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Repair Guidance - Technical Decision Support System",
        Description = "Yapay Zeka Destekli Basit Arýza Tamir Destek Sistemi"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
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
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options => {
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:5174") 
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
