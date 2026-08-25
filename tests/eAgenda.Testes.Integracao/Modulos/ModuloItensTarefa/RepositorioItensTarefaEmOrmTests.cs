using eAgenda.Dominio.Modulos.ModuloTarefa;
using eAgenda.Testes.Integracao.Compartilhado.Orm;
using FizzWare.NBuilder;

namespace eAgenda.Testes.Integracao.Modulos.ModuloItensTarefa;

[TestClass]
public class RepositorioDespesaEmOrmTests : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public void AdicionarItem_TarefaExistente_DevePersistirComSucesso()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Organizar festa")
            .With(t => t.Prioridade = PrioridadeTarefa.Alta)
            .Build();

        repositorioTarefa.Cadastrar(tarefa);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefa.Id);
        tarefaSelecionada!.AdicionarItem(new ItemTarefa("Comprar bebidas"));

        repositorioTarefa.Editar(tarefaSelecionada.Id, tarefaSelecionada);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaAtualizada = repositorioTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsNotNull(tarefaAtualizada);
        Assert.AreEqual("Organizar festa", tarefaAtualizada.Titulo);
        Assert.AreEqual(PrioridadeTarefa.Alta, tarefaAtualizada.Prioridade);

        Assert.IsNotNull(tarefaAtualizada.Itens);
        Assert.HasCount(1, tarefaAtualizada.Itens);
        Assert.AreEqual("Comprar bebidas", tarefaAtualizada.Itens[0].Titulo);
        Assert.IsFalse(tarefaAtualizada.Itens[0].Concluido);
    }

    [TestMethod]
    public void ConcluirItem_TarefaComQuatroItensPendentes_DeveAtualizarPercentual()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Organizar festa")
            .With(t => t.Prioridade = PrioridadeTarefa.Alta)
            .With(t => t.Itens = new List<ItemTarefa>
            {
            new ItemTarefa("Comprar bebidas"),
            new ItemTarefa("Comprar salgados"),
            new ItemTarefa("Comprar doces"),
            new ItemTarefa("Reservar salão")
            })
            .Build();

        repositorioTarefa.Cadastrar(tarefa);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefa.Id);
        Assert.IsNotNull(tarefaSelecionada);

        ItemTarefa itemConcluido = tarefaSelecionada!.Itens[0];
        tarefaSelecionada.AlterarConclusaoItem(itemConcluido.Id, true);

        repositorioTarefa.Editar(tarefaSelecionada.Id, tarefaSelecionada);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaAtualizada = repositorioTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsNotNull(tarefaAtualizada);
        Assert.AreEqual("Organizar festa", tarefaAtualizada.Titulo);
        Assert.AreEqual(PrioridadeTarefa.Alta, tarefaAtualizada.Prioridade);

        Assert.AreEqual(25, tarefaAtualizada.PercentualConcluido);
        Assert.IsFalse(tarefaAtualizada.Concluida);
        Assert.IsNull(tarefaAtualizada.DataConclusao);
    }

    [TestMethod]
    public void ConcluirTodosOsItens_TarefaComQuatroItens_DeveLevarPercentualA100()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Organizar festa")
            .With(t => t.Prioridade = PrioridadeTarefa.Alta)
            .With(t => t.Itens = new List<ItemTarefa>
            {
            new ItemTarefa("Comprar bebidas"),
            new ItemTarefa("Comprar salgados"),
            new ItemTarefa("Comprar doces"),
            new ItemTarefa("Reservar salão")
            })
            .Build();

        repositorioTarefa.Cadastrar(tarefa);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefa.Id);
        Assert.IsNotNull(tarefaSelecionada);

        tarefaSelecionada!.Itens = tarefaSelecionada.Itens
            .Select(i => { i.Concluido = true; return i; })
            .ToList();

        tarefaSelecionada.RecalcularConclusao();

        repositorioTarefa.Editar(tarefaSelecionada.Id, tarefaSelecionada);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaAtualizada = repositorioTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsNotNull(tarefaAtualizada);
        Assert.AreEqual("Organizar festa", tarefaAtualizada.Titulo);
        Assert.AreEqual(PrioridadeTarefa.Alta, tarefaAtualizada.Prioridade);

        Assert.AreEqual(100, tarefaAtualizada.PercentualConcluido);
        Assert.IsTrue(tarefaAtualizada.Concluida);
        Assert.IsNotNull(tarefaAtualizada.DataConclusao);
        Assert.AreEqual(DateTime.Today, tarefaAtualizada.DataConclusao!.Value.Date);
    }

    [TestMethod]
    public void ReabrirItemConcluido_TarefaComQuatroItens_DeveReduzirPercentual()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Organizar festa")
            .With(t => t.Prioridade = PrioridadeTarefa.Alta)
            .With(t => t.Itens = new List<ItemTarefa>
            {
            new ItemTarefa("Comprar bebidas") { Concluido = true },
            new ItemTarefa("Comprar salgados") { Concluido = true },
            new ItemTarefa("Comprar doces") { Concluido = true },
            new ItemTarefa("Reservar salão") { Concluido = true }
            })
            .Build();

        tarefa.RecalcularConclusao();

        repositorioTarefa.Cadastrar(tarefa);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefa.Id);
        Assert.IsNotNull(tarefaSelecionada);

        ItemTarefa itemReaberto = tarefaSelecionada!.Itens[0];
        tarefaSelecionada.AlterarConclusaoItem(itemReaberto.Id, false);

        repositorioTarefa.Editar(tarefaSelecionada.Id, tarefaSelecionada);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaAtualizada = repositorioTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsNotNull(tarefaAtualizada);
        Assert.AreEqual("Organizar festa", tarefaAtualizada.Titulo);
        Assert.AreEqual(PrioridadeTarefa.Alta, tarefaAtualizada.Prioridade);

        Assert.AreEqual(75, tarefaAtualizada.PercentualConcluido);
        Assert.IsFalse(tarefaAtualizada.Concluida);
        Assert.IsNull(tarefaAtualizada.DataConclusao);
    }

    [TestMethod]
    public void RemoverItem_TarefaComQuatroItensUmConcluido_DeveRecalcularPercentual()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Organizar festa")
            .With(t => t.Prioridade = PrioridadeTarefa.Alta)
            .With(t => t.Itens = new List<ItemTarefa>
            {
            new ItemTarefa("Comprar bebidas") { Concluido = true },
            new ItemTarefa("Comprar salgados"),
            new ItemTarefa("Comprar doces"),
            new ItemTarefa("Reservar salão")
            })
            .Build();

        tarefa.RecalcularConclusao();

        repositorioTarefa.Cadastrar(tarefa);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefa.Id);
        Assert.IsNotNull(tarefaSelecionada);

        ItemTarefa itemRemovido = tarefaSelecionada!.Itens.First(i => !i.Concluido);
        tarefaSelecionada.RemoverItem(itemRemovido.Id);

        repositorioTarefa.Editar(tarefaSelecionada.Id, tarefaSelecionada);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaAtualizada = repositorioTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsNotNull(tarefaAtualizada);
        Assert.AreEqual("Organizar festa", tarefaAtualizada.Titulo);
        Assert.AreEqual(PrioridadeTarefa.Alta, tarefaAtualizada.Prioridade);

        Assert.AreEqual(33, tarefaAtualizada.PercentualConcluido);
        Assert.IsFalse(tarefaAtualizada.Concluida);
        Assert.IsNull(tarefaAtualizada.DataConclusao);
    }

    [TestMethod]
    public void RemoverUltimoItem_TarefaComUmItem_DeveRecalcularPercentualParaZero()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Organizar festa")
            .With(t => t.Prioridade = PrioridadeTarefa.Alta)
            .With(t => t.Itens = new List<ItemTarefa>
            {
            new ItemTarefa("Comprar bebidas")
            })
            .Build();

        tarefa.RecalcularConclusao();

        repositorioTarefa.Cadastrar(tarefa);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefa.Id);
        Assert.IsNotNull(tarefaSelecionada);

        ItemTarefa itemRemovido = tarefaSelecionada!.Itens[0];
        bool conseguiuRemover = tarefaSelecionada.RemoverItem(itemRemovido.Id);

        repositorioTarefa.Editar(tarefaSelecionada.Id, tarefaSelecionada);
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(conseguiuRemover);

        Tarefa? tarefaAtualizada = repositorioTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsNotNull(tarefaAtualizada);
        Assert.AreEqual("Organizar festa", tarefaAtualizada.Titulo);
        Assert.AreEqual(PrioridadeTarefa.Alta, tarefaAtualizada.Prioridade);

        Assert.AreEqual(0, tarefaAtualizada.PercentualConcluido);
        Assert.IsFalse(tarefaAtualizada.Concluida);
        Assert.IsNull(tarefaAtualizada.DataConclusao);
        Assert.IsEmpty(tarefaAtualizada.Itens);
    }

    [TestMethod]
    public void EditarTituloItem_TarefaComItemExistente_DevePersistirAlteracao()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Organizar festa")
            .With(t => t.Prioridade = PrioridadeTarefa.Alta)
            .With(t => t.Itens = new List<ItemTarefa>
            {
            new ItemTarefa("Comprar bebidas")
            })
            .Build();

        repositorioTarefa.Cadastrar(tarefa);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefa.Id);
        Assert.IsNotNull(tarefaSelecionada);

        ItemTarefa itemEditado = tarefaSelecionada!.Itens[0];
        itemEditado.Titulo = "Comprar refrigerantes";

        repositorioTarefa.Editar(tarefaSelecionada.Id, tarefaSelecionada);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaAtualizada = repositorioTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsNotNull(tarefaAtualizada);
        Assert.AreEqual("Organizar festa", tarefaAtualizada.Titulo);
        Assert.AreEqual(PrioridadeTarefa.Alta, tarefaAtualizada.Prioridade);

        Assert.IsNotNull(tarefaAtualizada.Itens);
        Assert.HasCount(1, tarefaAtualizada.Itens);
        Assert.AreEqual("Comprar refrigerantes", tarefaAtualizada.Itens[0].Titulo);
    }

    [TestMethod]
    public void ListarItens_TarefaComDoisItens_DeveRetornarItensComSucesso()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Organizar festa")
            .With(t => t.Prioridade = PrioridadeTarefa.Alta)
            .With(t => t.Itens = new List<ItemTarefa>
            {
            new ItemTarefa("Comprar bebidas"),
            new ItemTarefa("Reservar salão")
            })
            .Build();

        repositorioTarefa.Cadastrar(tarefa);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsNotNull(tarefaSelecionada);
        Assert.AreEqual("Organizar festa", tarefaSelecionada.Titulo);
        Assert.AreEqual(PrioridadeTarefa.Alta, tarefaSelecionada.Prioridade);

        Assert.IsNotNull(tarefaSelecionada.Itens);
        Assert.HasCount(2, tarefaSelecionada.Itens);

        Assert.AreEqual("Comprar bebidas", tarefaSelecionada.Itens[0].Titulo);
        Assert.AreEqual("Reservar salão", tarefaSelecionada.Itens[1].Titulo);
    }

}