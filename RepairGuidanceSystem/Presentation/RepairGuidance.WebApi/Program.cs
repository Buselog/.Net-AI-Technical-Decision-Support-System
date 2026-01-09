using RepairGuidance.Persistence.DependencyResolvers;
using RepairGuidance.Application.DependencyResolvers;
using RepairGuidance.InnerInfrastructure.DependencyResolvers;
using RepairGuidance.Application.Managers;
using Polly;
using RepairGuidance.Infrastructure.ExternalServices;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextService(builder.Configuration);
builder.Services.AddRepositoryServices();
builder.Services.AddManagerServices();
builder.Services.AddMapperService();
builder.Services.AddHttpClient<IAiService, GroqAiService>()
    .AddTransientHttpErrorPolicy(policy =>
        policy.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
