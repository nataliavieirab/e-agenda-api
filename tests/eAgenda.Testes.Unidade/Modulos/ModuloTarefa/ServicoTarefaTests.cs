using eAgenda.Aplicacao.Modulos.ModuloTarefa;
using eAgenda.Dominio.Modulos.ModuloTarefa;
using FluentResults;
using Moq;

namespace eAgenda.Testes.Unidade.Modulos.ModuloTarefa;

[TestClass]
public sealed class ServicoTarefaTestes
{
    [TestMethod]
    public void Cadastrar_ComDadosValidos_PersisteTarefa()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new();

        repositorioTarefa.Setup(r => r.SelecionarTodos()).Returns([]);

        Tarefa? tarefaCadastrada = null;

        repositorioTarefa
            .Setup(r => r.Cadastrar(It.IsAny<Tarefa>()))
            .Callback<Tarefa>(tarefa => tarefaCadastrada = tarefa);

        ServicoTarefa servicoTarefa = new(repositorioTarefa.Object);

        Result resultado = servicoTarefa.Cadastrar(new CadastrarTarefaDto(
            "Faxina",
            PrioridadeTarefa.Alta
        ));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(tarefaCadastrada);
        Assert.AreEqual("Faxina", tarefaCadastrada.Titulo);
        Assert.AreEqual(PrioridadeTarefa.Alta, tarefaCadastrada.Prioridade);

        repositorioTarefa.Verify(r => r.Cadastrar(It.IsAny<Tarefa>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_SemItens_PersisteTarefa()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new();

        repositorioTarefa.Setup(r => r.SelecionarTodos()).Returns(new List<Tarefa>());

        Tarefa? tarefaCadastrada = null;

        repositorioTarefa
            .Setup(r => r.Cadastrar(It.IsAny<Tarefa>()))
            .Callback<Tarefa>(tarefa => tarefaCadastrada = tarefa);

        ServicoTarefa servicoTarefa = new(repositorioTarefa.Object);

        Result resultado = servicoTarefa.Cadastrar(new CadastrarTarefaDto(
            "Estudar",
            PrioridadeTarefa.Normal
        ));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(tarefaCadastrada);
        Assert.AreEqual("Estudar", tarefaCadastrada!.Titulo);
        Assert.AreEqual(PrioridadeTarefa.Normal, tarefaCadastrada.Prioridade);
        Assert.HasCount(0, tarefaCadastrada.Itens);

        repositorioTarefa.Verify(r => r.Cadastrar(It.IsAny<Tarefa>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_ComItens_PersisteTarefa()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new();

        repositorioTarefa.Setup(r => r.SelecionarTodos()).Returns(new List<Tarefa>());

        Tarefa? tarefaCadastrada = null;

        repositorioTarefa
            .Setup(r => r.Cadastrar(It.IsAny<Tarefa>()))
            .Callback<Tarefa>(tarefa => tarefaCadastrada = tarefa);

        repositorioTarefa
            .Setup(r => r.SelecionarPorId(It.IsAny<Guid>()))
            .Returns(() => tarefaCadastrada);

        ServicoTarefa servicoTarefa = new(repositorioTarefa.Object);

        Result resultadoCadastro = servicoTarefa.Cadastrar(new CadastrarTarefaDto(
            "Compras",
            PrioridadeTarefa.Baixa
        ));

        Assert.IsTrue(resultadoCadastro.IsSuccess);
        Assert.IsNotNull(tarefaCadastrada);

        Result resultadoItem1 = servicoTarefa.AdicionarItem(new AdicionarItemTarefaDto(
            tarefaCadastrada!.Id,
            "Comprar pão"
        ));

        Result resultadoItem2 = servicoTarefa.AdicionarItem(new AdicionarItemTarefaDto(
            tarefaCadastrada.Id,
            "Comprar leite"
        ));

        Assert.IsTrue(resultadoItem1.IsSuccess);
        Assert.IsTrue(resultadoItem2.IsSuccess);
        Assert.AreEqual(2, tarefaCadastrada.Itens.Count);
        Assert.AreEqual(0, tarefaCadastrada.PercentualConcluido);
        Assert.IsFalse(tarefaCadastrada.Concluida);
        Assert.IsNull(tarefaCadastrada.DataConclusao);
        Assert.AreEqual("Comprar pão", tarefaCadastrada.Itens[0].Titulo);
        Assert.AreEqual("Comprar leite", tarefaCadastrada.Itens[1].Titulo);
    }

    [TestMethod]
    public void Cadastrar_ComTituloVazio_RetornaErro()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new();

        repositorioTarefa.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoTarefa servicoTarefa = new(
            repositorioTarefa.Object
        );

        Result resultado = servicoTarefa.Cadastrar(new CadastrarTarefaDto(
            string.Empty,
            PrioridadeTarefa.Baixa
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "O campo \"Título\" deve ser preenchido.",
            resultado.Errors.First().Message
        );

        repositorioTarefa.Verify(r => r.Cadastrar(It.IsAny<Tarefa>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_ComPrioridadeInvalida_RetornaErro()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new();

        repositorioTarefa.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoTarefa servicoTarefa = new(
            repositorioTarefa.Object
        );

        Result resultado = servicoTarefa.Cadastrar(new CadastrarTarefaDto(
            "Planejar viagem",
            (PrioridadeTarefa)999
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "O campo \"Prioridade\" deve ter valores válidos.",
            resultado.Errors.First().Message
        );

        repositorioTarefa.Verify(r => r.Cadastrar(It.IsAny<Tarefa>()), Times.Never);
    }

    [TestMethod]
    public void Verificar_DadosCadastrais_TarefaRecemCriada()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new();

        repositorioTarefa.Setup(r => r.SelecionarTodos()).Returns(new List<Tarefa>());

        Tarefa? tarefaCadastrada = null;

        repositorioTarefa
            .Setup(r => r.Cadastrar(It.IsAny<Tarefa>()))
            .Callback<Tarefa>(tarefa => tarefaCadastrada = tarefa);

        ServicoTarefa servicoTarefa = new(repositorioTarefa.Object);

        Result resultado = servicoTarefa.Cadastrar(new CadastrarTarefaDto(
            "Faxina",
            PrioridadeTarefa.Alta
        ));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(tarefaCadastrada);
        Assert.AreEqual(DateTime.Today, tarefaCadastrada!.DataCriacao);
        Assert.IsNull(tarefaCadastrada.DataConclusao);
        Assert.IsFalse(tarefaCadastrada.Concluida);
        Assert.AreEqual(0, tarefaCadastrada.PercentualConcluido);
        Assert.HasCount(0, tarefaCadastrada.Itens);
    }

    [TestMethod]
    public void Editar_ComDadosValidos_PersisteTarefa()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new();

        Tarefa tarefaExistente = new Tarefa(
            "Estudar",
            PrioridadeTarefa.Baixa
        );

        List<Tarefa> tarefas = new() { tarefaExistente };

        repositorioTarefa.Setup(r => r.SelecionarTodos()).Returns(() => tarefas);
        repositorioTarefa.Setup(r => r.SelecionarPorId(It.IsAny<Guid>()))
            .Returns<Guid>(id => tarefas.FirstOrDefault(t => t.Id == id));
        repositorioTarefa
            .Setup(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Tarefa>()))
            .Callback<Guid, Tarefa>((id, tarefaAtualizada) =>
            {
                tarefaAtualizada.Id = id;
                int index = tarefas.FindIndex(c => c.Id == id);
                if (index >= 0)
                    tarefas[index].Atualizar(tarefaAtualizada);
            })
            .Returns<Guid, Tarefa>((id, contatoAtualizado) => tarefas.Any(c => c.Id == id));

        ServicoTarefa servicoTarefa = new ServicoTarefa(
            repositorioTarefa.Object
        );

        Result resultado = servicoTarefa.Editar(new EditarTarefaDto(
            tarefaExistente.Id,
            "Estudar Testes Automatizados",
            PrioridadeTarefa.Alta
        ));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioTarefa.Verify(r => r.Editar(tarefaExistente.Id, It.IsAny<Tarefa>()), Times.Once);

        List<ListarTarefasDto> tarefasListadas = servicoTarefa.SelecionarTodos();

        Assert.HasCount(1, tarefasListadas);
        Assert.AreEqual("Estudar Testes Automatizados", tarefasListadas[0].Titulo);
        Assert.AreEqual(PrioridadeTarefa.Alta, tarefasListadas[0].Prioridade);
    }

    [TestMethod]
    public void ConcluirTarefa_RegistraDataConclusaoEStatus()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new();

        Tarefa tarefaPendente = new Tarefa("Estudar", PrioridadeTarefa.Alta);

        repositorioTarefa
            .Setup(r => r.SelecionarPorId(tarefaPendente.Id))
            .Returns(tarefaPendente);
        repositorioTarefa
            .Setup(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Tarefa>()))
            .Callback<Guid, Tarefa>((id, tarefaAtualizada) =>
            {
                tarefaAtualizada.Id = id;
                tarefaPendente.Atualizar(tarefaAtualizada);
            });

        ServicoTarefa servicoTarefa = new(repositorioTarefa.Object);

        Result resultado = servicoTarefa.AlterarConclusao(new AlterarConclusaoTarefaDto(
            tarefaPendente.Id,
            true
        ));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsTrue(tarefaPendente.Concluida);
        Assert.AreEqual(100, tarefaPendente.PercentualConcluido);
        Assert.AreEqual(DateTime.Today, tarefaPendente.DataConclusao);
        repositorioTarefa.Verify(r => r.Editar(tarefaPendente.Id, It.IsAny<Tarefa>()), Times.Once);
    }

    [TestMethod]
    public void ReabrirTarefa_AtualizaStatusEDataConclusao()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new();

        Tarefa tarefaConcluida = new Tarefa("Comprar pão", PrioridadeTarefa.Baixa);
        tarefaConcluida.Concluida = true;
        tarefaConcluida.PercentualConcluido = 100;
        tarefaConcluida.DataConclusao = DateTime.Today;

        repositorioTarefa
            .Setup(r => r.SelecionarPorId(tarefaConcluida.Id))
            .Returns(tarefaConcluida);
        repositorioTarefa
            .Setup(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Tarefa>()))
            .Callback<Guid, Tarefa>((id, tarefaAtualizada) =>
            {
                tarefaAtualizada.Id = id;
                tarefaConcluida.Atualizar(tarefaAtualizada);
            });

        ServicoTarefa servicoTarefa = new(repositorioTarefa.Object);

        Result resultado = servicoTarefa.AlterarConclusao(new AlterarConclusaoTarefaDto(
            tarefaConcluida.Id,
            false
        ));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsFalse(tarefaConcluida.Concluida);
        Assert.AreEqual(0, tarefaConcluida.PercentualConcluido);
        Assert.IsNull(tarefaConcluida.DataConclusao);
        repositorioTarefa.Verify(r => r.Editar(tarefaConcluida.Id, It.IsAny<Tarefa>()), Times.Once);
    }

    [TestMethod]
    public void SelecionarTodos_SemFiltro_RetornaTarefas()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new();

        Tarefa tarefaPendente = new Tarefa("Estudar", PrioridadeTarefa.Alta);
        Tarefa tarefaConcluida = new Tarefa("Comprar pão", PrioridadeTarefa.Baixa);
        tarefaConcluida.Concluida = true;
        tarefaConcluida.PercentualConcluido = 100;
        tarefaConcluida.DataConclusao = DateTime.Today;

        repositorioTarefa.Setup(r => r.SelecionarTodos()).Returns(new List<Tarefa> { tarefaPendente, tarefaConcluida });

        ServicoTarefa servicoTarefa = new(repositorioTarefa.Object);

        List<ListarTarefasDto> tarefasListadas = servicoTarefa.SelecionarTodos();

        Assert.HasCount(2, tarefasListadas);
        Assert.AreEqual("Estudar", tarefasListadas[0].Titulo);
        Assert.AreEqual("Comprar pão", tarefasListadas[1].Titulo);
        Assert.IsFalse(tarefasListadas[0].Concluida);
        Assert.IsTrue(tarefasListadas[1].Concluida);
    }

    [TestMethod]
    public void SelecionarTodos_ComFiltroPendentes_RetornaTarefasPendentes()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new();

        Tarefa tarefaPendente = new Tarefa("Estudar", PrioridadeTarefa.Alta);
        Tarefa tarefaConcluida = new Tarefa("Comprar pão", PrioridadeTarefa.Baixa);
        tarefaConcluida.Concluida = true;
        tarefaConcluida.PercentualConcluido = 100;
        tarefaConcluida.DataConclusao = DateTime.Today;

        repositorioTarefa.Setup(r => r.SelecionarTodos()).Returns(new List<Tarefa> { tarefaPendente, tarefaConcluida });

        ServicoTarefa servicoTarefa = new(repositorioTarefa.Object);

        List<ListarTarefasDto> tarefasListadas = servicoTarefa.SelecionarTodos("Pendentes");

        Assert.HasCount(1, tarefasListadas);
        Assert.AreEqual("Estudar", tarefasListadas[0].Titulo);
        Assert.IsFalse(tarefasListadas[0].Concluida);
    }

    [TestMethod]
    public void SelecionarTodos_ComFiltroConcluidas_RetornaTarefasConcluidas()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new();

        Tarefa tarefaPendente = new Tarefa("Estudar", PrioridadeTarefa.Alta);
        Tarefa tarefaConcluida = new Tarefa("Comprar pão", PrioridadeTarefa.Baixa);
        tarefaConcluida.Concluida = true;
        tarefaConcluida.PercentualConcluido = 100;
        tarefaConcluida.DataConclusao = DateTime.Today;

        repositorioTarefa.Setup(r => r.SelecionarTodos()).Returns(new List<Tarefa> { tarefaPendente, tarefaConcluida });

        ServicoTarefa servicoTarefa = new(repositorioTarefa.Object);

        List<ListarTarefasDto> tarefasListadas = servicoTarefa.SelecionarTodos("Concluidas");

        Assert.HasCount(1, tarefasListadas);
        Assert.AreEqual("Comprar pão", tarefasListadas[0].Titulo);
        Assert.IsTrue(tarefasListadas[0].Concluida);
    }

    [TestMethod]
    public void SelecionarTodos_PorPrioridade_RetornaTarefasAgrupadas()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new();

        Tarefa tarefaBaixa = new Tarefa("Organizar armário", PrioridadeTarefa.Baixa);
        Tarefa tarefaNormal = new Tarefa("Estudar", PrioridadeTarefa.Normal);
        Tarefa tarefaAlta = new Tarefa("Responder e-mails", PrioridadeTarefa.Alta);

        repositorioTarefa.Setup(r => r.SelecionarTodos()).Returns(new List<Tarefa> { tarefaBaixa, tarefaNormal, tarefaAlta });

        ServicoTarefa servicoTarefa = new(repositorioTarefa.Object);

        List<ListarTarefasDto> tarefasListadas = servicoTarefa.SelecionarTodos();
        var tarefasAgrupadas = tarefasListadas
            .GroupBy(t => t.Prioridade)
            .ToDictionary(g => g.Key, g => g.Select(t => t.Titulo).ToList());

        Assert.IsTrue(tarefasAgrupadas.ContainsKey(PrioridadeTarefa.Baixa));
        Assert.IsTrue(tarefasAgrupadas.ContainsKey(PrioridadeTarefa.Normal));
        Assert.IsTrue(tarefasAgrupadas.ContainsKey(PrioridadeTarefa.Alta));
        Assert.AreEqual("Organizar armário", tarefasAgrupadas[PrioridadeTarefa.Baixa][0]);
        Assert.AreEqual("Estudar", tarefasAgrupadas[PrioridadeTarefa.Normal][0]);
        Assert.AreEqual("Responder e-mails", tarefasAgrupadas[PrioridadeTarefa.Alta][0]);
    }

    [TestMethod]
    public void SelecionarPorId_ComTarefaEItens_RetornaDadosDaTarefaEItens()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new();

        Tarefa tarefaComItens = new Tarefa("Planejar viagem", PrioridadeTarefa.Normal);
        tarefaComItens.AdicionarItem(new ItemTarefa("Reservar hotel"));
        tarefaComItens.AdicionarItem(new ItemTarefa("Comprar passagem"));

        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefaComItens.Id)).Returns(tarefaComItens);

        ServicoTarefa servicoTarefa = new(repositorioTarefa.Object);

        Result<DetalhesTarefaDto> resultado = servicoTarefa.SelecionarPorId(tarefaComItens.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(resultado.Value);
        Assert.AreEqual(tarefaComItens.Id, resultado.Value.Id);
        Assert.AreEqual("Planejar viagem", resultado.Value.Titulo);
        Assert.AreEqual(PrioridadeTarefa.Normal, resultado.Value.Prioridade);
        Assert.HasCount(2, resultado.Value.Itens);
        Assert.AreEqual("Reservar hotel", resultado.Value.Itens[0].Titulo);
        Assert.AreEqual("Comprar passagem", resultado.Value.Itens[1].Titulo);
    }

    [TestMethod]
    public void Excluir_ComTarefaEItensVinculados_RemoveTarefaEItens()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new();

        Tarefa tarefaComItens = new Tarefa("Limpar casa", PrioridadeTarefa.Baixa);
        tarefaComItens.AdicionarItem(new ItemTarefa("Varrer quarto"));

        List<Tarefa> tarefas = new() { tarefaComItens };

        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefaComItens.Id)).Returns(tarefaComItens);
        repositorioTarefa.Setup(r => r.Excluir(It.IsAny<Guid>()))
            .Callback<Guid>(id =>
            {
                Tarefa? tarefa = tarefas.FirstOrDefault(t => t.Id == id);
                if (tarefa != null)
                {
                    tarefas.Remove(tarefa);
                    tarefa.Itens.Clear();
                }
            })
            .Returns<Guid>(_ => true);

        ServicoTarefa servicoTarefa = new(repositorioTarefa.Object);

        Result resultado = servicoTarefa.Excluir(tarefaComItens.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.HasCount(0, tarefas);
        Assert.HasCount(0, tarefaComItens.Itens);
        repositorioTarefa.Verify(r => r.Excluir(tarefaComItens.Id), Times.Once);
    }
}
