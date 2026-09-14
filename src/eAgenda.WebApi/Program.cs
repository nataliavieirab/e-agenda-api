using eAgenda.Infra;
using eAgenda.Aplicacao;
using eAgenda.Infra.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;
using eAgenda.WebApi.Compartilhado;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfraRepositories(builder.Configuration, builder.Logging, builder.Environment);
builder.Services.AddApplicationServices();
builder.Services.AddControllers();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        string? type = ProblemDetailsTypes.ObterPorStatus(context.ProblemDetails.Status);

        if (type is not null)
            context.ProblemDetails.Type = type;

        // TraceId
        context.ProblemDetails.Extensions["traceId"] =
            Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<EAgendaDbContext>();

    dbContext.Database.Migrate();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
