using Microsoft.Extensions.Diagnostics.HealthChecks;
using eAgenda.Aplicacao;
using eAgenda.WebApp.Compartilhado.Apresentacao;
using eAgenda.Infra;
using eAgenda.Infra.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuração do container de injeção de dependência
builder.Services.AddInfraRepositories(builder.Configuration, builder.Logging, builder.Environment);

builder.Services.AddApplicationServices();

builder.Services.AddPresentationConfig(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddDbContextCheck<EAgendaDbContext>(
        name: "database_check",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["ready"]
    );

var app = builder.Build();

// Aplica migrações automaticamente em Desenvolvimento
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<EAgendaDbContext>();

    dbContext.Database.Migrate();
}


// Middlewares de roteamento
app.UseRouting();
app.MapDefaultControllerRoute();

app.MapHealthChecks("/health");

// Execução do Servidor
app.Run();
