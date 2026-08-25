using eAgenda.Dominio.Modulos.ModuloTarefa;
using eAgenda.Testes.Integracao.Compartilhado.Orm;
using FizzWare.NBuilder;
namespace eAgenda.Testes.Integracao.Modulos.ModuloTarefa;

[TestClass]
public class RepositorioTarefaEmOrmTests : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public void Cadastrar_ComDadosValidos_RegistraTarefa()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Organizar festa")
            .With(t => t.Prioridade = PrioridadeTarefa.Alta)
            .Build();

        repositorioTarefa.Cadastrar(tarefa);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsNotNull(tarefaSelecionada);
        Assert.AreEqual("Organizar festa", tarefaSelecionada.Titulo);
        Assert.AreEqual(PrioridadeTarefa.Alta, tarefaSelecionada.Prioridade);
    }

    [TestMethod]
    public void Verificar_DadosRegistrados_TarefaRecemCriada()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Pagar contas")
            .With(t => t.Prioridade = PrioridadeTarefa.Normal)
            .With(t => t.Concluida = false)
            .With(t => t.PercentualConcluido = 0)
            .With(t => t.DataConclusao = null)
            .Build();

        repositorioTarefa.Cadastrar(tarefa);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsNotNull(tarefaSelecionada);
        Assert.IsFalse(tarefaSelecionada.Concluida);
        Assert.AreEqual(0, tarefaSelecionada.PercentualConcluido);
        Assert.AreEqual(DateTime.Today, tarefaSelecionada.DataCriacao);
        Assert.IsNull(tarefaSelecionada.DataConclusao);
    }

    [TestMethod]
    public void Cadastrar_SemItens_RegistraTarefa()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Enviar relatório")
            .With(t => t.Prioridade = PrioridadeTarefa.Baixa)
            .With(t => t.Itens = new List<ItemTarefa>())
            .With(t => t.Concluida = false)
            .With(t => t.PercentualConcluido = 0)
            .With(t => t.DataConclusao = null)
            .Build();

        repositorioTarefa.Cadastrar(tarefa);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsNotNull(tarefaSelecionada);
        Assert.IsNotNull(tarefaSelecionada.Itens);
        Assert.IsEmpty(tarefaSelecionada.Itens);
        Assert.IsFalse(tarefaSelecionada.Concluida);
        Assert.AreEqual(0, tarefaSelecionada.PercentualConcluido);
    }

    [TestMethod]
    public void Cadastrar_ComItens_RegistraTarefa()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Planejar viagem")
            .With(t => t.Prioridade = PrioridadeTarefa.Normal)
            .With(t => t.Itens = new List<ItemTarefa>
            {
                new ItemTarefa("Comprar passagens"),
                new ItemTarefa("Reservar hotel")
            })
            .With(t => t.Concluida = false)
            .With(t => t.PercentualConcluido = 0)
            .With(t => t.DataConclusao = null)
            .Build();

        repositorioTarefa.Cadastrar(tarefa);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsNotNull(tarefaSelecionada);
        Assert.IsNotNull(tarefaSelecionada.Itens);
        Assert.HasCount(2, tarefaSelecionada.Itens);
        Assert.IsFalse(tarefaSelecionada.Concluida);
        Assert.AreEqual(0, tarefaSelecionada.PercentualConcluido);
    }

    [TestMethod]
    public void Editar_ComDadosValidos_AtualizaTarefa()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Comprar presente")
            .With(t => t.Prioridade = PrioridadeTarefa.Baixa)
            .Persist();

        Tarefa tarefaAtualizada = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Comprar presente de aniversário")
            .With(t => t.Prioridade = PrioridadeTarefa.Alta)
            .Build();

        bool conseguiuEditar = repositorioTarefa.Editar(tarefa.Id, tarefaAtualizada);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.IsNotNull(tarefaSelecionada);
        Assert.AreEqual("Comprar presente de aniversário", tarefaSelecionada.Titulo);
        Assert.AreEqual(PrioridadeTarefa.Alta, tarefaSelecionada.Prioridade);
    }

    [TestMethod]
    public void Excluir_ComItensVinculados_RemoveTarefaEItens()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Montar apresentação")
            .With(t => t.Prioridade = PrioridadeTarefa.Normal)
            .With(t => t.Itens = new List<ItemTarefa>
            {
                new ItemTarefa("Criar slides"),
                new ItemTarefa("Revisar texto")
            })
            .Persist();

        bool conseguiuExcluir = repositorioTarefa.Excluir(tarefa.Id);
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(repositorioTarefa.SelecionarPorId(tarefa.Id));
    }

    [TestMethod]
    public void ConcluirTarefa_AlteraDataConclusaoEStatus()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Fazer matrícula")
            .With(t => t.Prioridade = PrioridadeTarefa.Alta)
            .Persist();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefa.Id);
        Assert.IsNotNull(tarefaSelecionada);

        tarefaSelecionada!.AlterarConclusaoManual(true);
        repositorioTarefa.Editar(tarefaSelecionada.Id, tarefaSelecionada);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaAtualizada = repositorioTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsNotNull(tarefaAtualizada);
        Assert.IsTrue(tarefaAtualizada.Concluida);
        Assert.AreEqual(100, tarefaAtualizada.PercentualConcluido);
        Assert.IsNotNull(tarefaAtualizada.DataConclusao);
        Assert.AreEqual(DateTime.Today, tarefaAtualizada.DataConclusao!.Value.Date);
    }

    [TestMethod]
    public void ReabrirTarefa_AlteraDataConclusaoEStatus()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Enviar propostas")
            .With(t => t.Prioridade = PrioridadeTarefa.Normal)
            .Persist();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefa.Id);
        Assert.IsNotNull(tarefaSelecionada);

        tarefaSelecionada!.AlterarConclusaoManual(true);
        repositorioTarefa.Editar(tarefaSelecionada.Id, tarefaSelecionada);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaConcluida = repositorioTarefa.SelecionarPorId(tarefa.Id);
        Assert.IsNotNull(tarefaConcluida);
        tarefaConcluida!.AlterarConclusaoManual(false);
        repositorioTarefa.Editar(tarefaConcluida.Id, tarefaConcluida);
        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaAtualizada = repositorioTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsNotNull(tarefaAtualizada);
        Assert.IsFalse(tarefaAtualizada.Concluida);
        Assert.AreEqual(0, tarefaAtualizada.PercentualConcluido);
        Assert.IsNull(tarefaAtualizada.DataConclusao);
    }

    [TestMethod]
    public void SelecionarPorId_RetornaTarefaEItens()
    {
        Tarefa tarefa = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Publicar blog")
            .With(t => t.Prioridade = PrioridadeTarefa.Normal)
            .With(t => t.Itens = new List<ItemTarefa>
            {
                new ItemTarefa("Escrever texto"),
                new ItemTarefa("Revisar conteúdo")
            })
            .Persist();

        dbContext.ChangeTracker.Clear();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsNotNull(tarefaSelecionada);
        Assert.AreEqual(tarefa.Id, tarefaSelecionada.Id);
        Assert.IsNotNull(tarefaSelecionada.Itens);
        Assert.HasCount(2, tarefaSelecionada.Itens);
        Assert.AreEqual("Escrever texto", tarefaSelecionada.Itens[0].Titulo);
        Assert.AreEqual("Revisar conteúdo", tarefaSelecionada.Itens[1].Titulo);
    }

    [TestMethod]
    public void SelecionarTodos_RetornaTodasAsTarefas()
    {
        Builder<Tarefa>
            .CreateListOfSize(3)
            .All()
            .With(t => t.Titulo = "Tarefa " + Guid.NewGuid())
            .With(t => t.Prioridade = PrioridadeTarefa.Normal)
            .Persist();

        dbContext.ChangeTracker.Clear();

        List<Tarefa> tarefas = repositorioTarefa.SelecionarTodos();

        Assert.HasCount(3, tarefas);
    }

    [TestMethod]
    public void SelecionarTodos_ComFiltroPendente_RetornaApenasTarefasPendentes()
    {
        Tarefa tarefaPendente = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Lavar carro")
            .With(t => t.Prioridade = PrioridadeTarefa.Baixa)
            .Persist();

        Tarefa tarefaConcluida = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Enviar nota fiscal")
            .With(t => t.Prioridade = PrioridadeTarefa.Alta)
            .Persist();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefaConcluida.Id);
        Assert.IsNotNull(tarefaSelecionada);
        tarefaSelecionada!.AlterarConclusaoManual(true);
        repositorioTarefa.Editar(tarefaSelecionada.Id, tarefaSelecionada);
        dbContext.ChangeTracker.Clear();

        List<Tarefa> tarefasPendentes = repositorioTarefa.SelecionarTodos()
            .Where(t => !t.Concluida)
            .ToList();

        Assert.IsTrue(tarefasPendentes.All(t => !t.Concluida));
        Assert.IsTrue(tarefasPendentes.Any(t => t.Id == tarefaPendente.Id));
        Assert.IsFalse(tarefasPendentes.Any(t => t.Id == tarefaConcluida.Id));
    }

    [TestMethod]
    public void SelecionarTodos_ComFiltroConcluida_RetornaApenasTarefasConcluidas()
    {
        Tarefa tarefaPendente = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Pagar água")
            .With(t => t.Prioridade = PrioridadeTarefa.Normal)
            .Persist();

        Tarefa tarefaConcluida = Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Pagar luz")
            .With(t => t.Prioridade = PrioridadeTarefa.Alta)
            .Persist();

        Tarefa? tarefaSelecionada = repositorioTarefa.SelecionarPorId(tarefaConcluida.Id);
        Assert.IsNotNull(tarefaSelecionada);
        tarefaSelecionada!.AlterarConclusaoManual(true);
        repositorioTarefa.Editar(tarefaSelecionada.Id, tarefaSelecionada);
        dbContext.ChangeTracker.Clear();

        List<Tarefa> tarefasConcluidas = repositorioTarefa.SelecionarTodos()
            .Where(t => t.Concluida)
            .ToList();

        Assert.IsTrue(tarefasConcluidas.All(t => t.Concluida));
        Assert.IsTrue(tarefasConcluidas.Any(t => t.Id == tarefaConcluida.Id));
        Assert.IsFalse(tarefasConcluidas.Any(t => t.Id == tarefaPendente.Id));
    }

    [TestMethod]
    public void SelecionarTodos_AgrupadasPorPrioridade_RetornaTarefasAgrupadas()
    {
        Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Organizar arquivos")
            .With(t => t.Prioridade = PrioridadeTarefa.Baixa)
            .Persist();

        Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Estudar para prova")
            .With(t => t.Prioridade = PrioridadeTarefa.Normal)
            .Persist();

        Builder<Tarefa>
            .CreateNew()
            .With(t => t.Titulo = "Entregar projeto")
            .With(t => t.Prioridade = PrioridadeTarefa.Alta)
            .Persist();

        dbContext.ChangeTracker.Clear();

        var tarefasAgrupadas = repositorioTarefa.SelecionarTodos()
            .GroupBy(t => t.Prioridade)
            .ToDictionary(g => g.Key, g => g.ToList());

        Assert.IsTrue(tarefasAgrupadas.ContainsKey(PrioridadeTarefa.Baixa));
        Assert.IsTrue(tarefasAgrupadas.ContainsKey(PrioridadeTarefa.Normal));
        Assert.IsTrue(tarefasAgrupadas.ContainsKey(PrioridadeTarefa.Alta));
        Assert.HasCount(1, tarefasAgrupadas[PrioridadeTarefa.Baixa]);
        Assert.HasCount(1, tarefasAgrupadas[PrioridadeTarefa.Normal]);
        Assert.HasCount(1, tarefasAgrupadas[PrioridadeTarefa.Alta]);
    }
}
