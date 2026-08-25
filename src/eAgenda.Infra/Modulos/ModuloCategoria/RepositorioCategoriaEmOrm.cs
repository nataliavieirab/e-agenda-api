using eAgenda.Infra.Compartilhado.Orm;
using eAgenda.Dominio.Modulos.ModuloCategoria;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infra.Modulos.ModuloCategoria;

public sealed class RepositorioCategoriaEmOrm(EAgendaDbContext dbContext) :
    RepositorioBaseEmOrm<Categoria>(dbContext), IRepositorioCategoria
{
    public override Categoria? SelecionarPorId(Guid idSelecionado)
    {
        Categoria? categoria = registros
            .Include(c => c.Despesas)
            .SingleOrDefault(c => c.Id == idSelecionado);

        OrdenarDespesas(categoria);

        return categoria;
    }

    private static void OrdenarDespesas(Categoria? categoria)
    {
        if (categoria == null)
            return;

        categoria.Despesas = categoria.Despesas
            .OrderBy(d => d.Descricao)
            .ToList();
    }
}
