using eAgenda.Dominio.Modulos.ModuloCategoria;
using eAgenda.Dominio.Modulos.ModuloCompromisso;
using eAgenda.Dominio.Modulos.ModuloContato;
using eAgenda.Dominio.Modulos.ModuloDespesa;
using eAgenda.Dominio.Modulos.ModuloTarefa;
using eAgenda.Infra.Compartilhado.Logging;
using eAgenda.Infra.Compartilhado.Orm;
using eAgenda.Infra.Compartilhado.Sql;
using eAgenda.Infra.Modulos.ModuloCategoria;
using eAgenda.Infra.Modulos.ModuloCompromisso;
using eAgenda.Infra.Modulos.ModuloContato;
using eAgenda.Infra.Modulos.ModuloDespesa;
using eAgenda.Infra.Modulos.ModuloTarefa;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace eAgenda.Infra;

public static class InjecaoDependencia
{
    public static void AddInfraRepositories(
        this IServiceCollection services,
        IConfiguration configuration,
        ILoggingBuilder logging,
        IHostEnvironment environment
    )
    {
        // Injeta logs do Serilog
        Serilog.ILogger logger = SerilogFactory.Create(configuration, environment);

        logging.ClearProviders();

        services.AddSerilog(logger, dispose: true);

        // Injeta o DbContext do EF
        services.AddDbContext<EAgendaDbContext>(options =>
        {
            string? connectionString = configuration.GetConnectionString("SqlServerEF");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    $"A connection string \"SqlServerEF\" não foi encontrada."
                );
            }

            options.UseSqlServer(connectionString, opt =>
            {
                opt.EnableRetryOnFailure(3);
            });
        });

        services.AddDbContext<EAgendaDbContext>(options =>
       {
           string? connectionString = configuration.GetConnectionString("SqlServerEF");

           if (string.IsNullOrWhiteSpace(connectionString))
           {
               throw new InvalidOperationException(
                   $"A connection string \"SqlServerEF\" não foi encontrada."
               );
           }

           options.UseSqlServer(connectionString, opt =>
           {
               opt.EnableRetryOnFailure(3);
           });
       });

        services.AddScoped<IRepositorioContato, RepositorioContatoEmOrm>();
        services.AddScoped<IRepositorioCompromisso, RepositorioCompromissoEmOrm>();
        services.AddScoped<IRepositorioCategoria, RepositorioCategoriaEmOrm>();
        services.AddScoped<IRepositorioDespesa, RepositorioDespesaEmOrm>();
        services.AddScoped<IRepositorioTarefa, RepositorioTarefaEmOrm>();
    }
}
