using eAgenda.Aplicacao.Modulos.ModuloTarefa;
using eAgenda.Dominio.Modulos.ModuloTarefa;
using FluentResults;
using Moq;

namespace eAgenda.Testes.Unidade.Modulos.ModuloItensTarefa;

[TestClass]
public sealed class ServicoItensTarefaTests
{
    [TestMethod]
    public void AdicionarItem_TarefaExistente_ItemAdicionado()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new Mock<IRepositorioTarefa>();

        Tarefa tarefaExistente = new Tarefa("Finalizar relatório", PrioridadeTarefa.Alta);

        List<Tarefa> tarefas = new() { tarefaExistente };

        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefaExistente.Id))
            .Returns(tarefaExistente);

        repositorioTarefa.Setup(r => r.SelecionarTodos())
            .Returns(() => tarefas);

        repositorioTarefa
            .Setup(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Tarefa>()))
            .Callback<Guid, Tarefa>((id, tarefaAtualizada) =>
            {
                int index = tarefas.FindIndex(t => t.Id == id);
                if (index >= 0)
                    tarefas[index].Atualizar(tarefaAtualizada);
            })
            .Returns<Guid, Tarefa>((id, tarefaAtualizada) => tarefas.Any(t => t.Id == id));

        ServicoTarefa servicoTarefa = new ServicoTarefa(repositorioTarefa.Object);

        AdicionarItemTarefaDto dto = new AdicionarItemTarefaDto(tarefaExistente.Id, "Revisar conclusão");
        Result resultado = servicoTarefa.AdicionarItem(dto);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.HasCount(1, tarefaExistente.Itens);
        Assert.AreEqual("Revisar conclusão", tarefaExistente.Itens[0].Titulo);
        Assert.IsFalse(tarefaExistente.Itens[0].Concluido);

        List<ListarTarefasDto> tarefasListadas = servicoTarefa.SelecionarTodos();
        Assert.HasCount(1, tarefasListadas);
        Assert.HasCount(1, tarefasListadas[0].Itens);
        Assert.AreEqual("Revisar conclusão", tarefasListadas[0].Itens[0].Titulo);
    }

    [TestMethod]
    public void AdicionarItem_SemTitulo_DeveRetornarErro()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new Mock<IRepositorioTarefa>();

        Tarefa tarefaExistente = new Tarefa("Finalizar relatório", PrioridadeTarefa.Alta);
        List<Tarefa> tarefas = new() { tarefaExistente };

        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefaExistente.Id))
            .Returns(tarefaExistente);

        repositorioTarefa.Setup(r => r.SelecionarTodos())
            .Returns(() => tarefas);

        ServicoTarefa servicoTarefa = new ServicoTarefa(repositorioTarefa.Object);

        AdicionarItemTarefaDto dto = new AdicionarItemTarefaDto(tarefaExistente.Id, "");
        Result resultado = servicoTarefa.AdicionarItem(dto);

        Assert.IsFalse(resultado.IsSuccess);
        Assert.AreEqual("O campo \"Título\" deve conter entre 2 e 100 caracteres.", resultado.Errors[0].Message);
        Assert.IsEmpty(tarefaExistente.Itens);

        repositorioTarefa.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Tarefa>()), Times.Never);
    }

    [TestMethod]
    public void AdicionarItem_SemVinculoComTarefa_DeveRetornarErro()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new Mock<IRepositorioTarefa>();

        repositorioTarefa.Setup(r => r.SelecionarPorId(It.IsAny<Guid>()))
            .Returns((Tarefa?)null);

        ServicoTarefa servicoTarefa = new ServicoTarefa(repositorioTarefa.Object);

        AdicionarItemTarefaDto dto = new AdicionarItemTarefaDto(Guid.NewGuid(), "Item sem tarefa");
        Result resultado = servicoTarefa.AdicionarItem(dto);

        Assert.IsFalse(resultado.IsSuccess);
        Assert.AreEqual("Tarefa não encontrada.", resultado.Errors[0].Message);

        repositorioTarefa.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Tarefa>()), Times.Never);
    }

    [TestMethod]
    public void ConcluirItem_TarefaComQuatroItensPendentes_AtualizaPercentualPara25()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new Mock<IRepositorioTarefa>();

        Tarefa tarefa = new Tarefa("Estudar para prova", PrioridadeTarefa.Alta);

        ItemTarefa item1 = new ItemTarefa("Ler capítulo 1");
        ItemTarefa item2 = new ItemTarefa("Ler capítulo 2");
        ItemTarefa item3 = new ItemTarefa("Fazer exercícios");
        ItemTarefa item4 = new ItemTarefa("Revisar resumo");

        tarefa.AdicionarItem(item1);
        tarefa.AdicionarItem(item2);
        tarefa.AdicionarItem(item3);
        tarefa.AdicionarItem(item4);

        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefa.Id)).Returns(tarefa);

        ServicoTarefa servicoTarefa = new ServicoTarefa(repositorioTarefa.Object);

        AlterarConclusaoItemTarefaDto dto = new AlterarConclusaoItemTarefaDto(tarefa.Id, item1.Id, true);
        Result resultado = servicoTarefa.AlterarConclusaoItem(dto);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(25, tarefa.PercentualConcluido);
        Assert.IsFalse(tarefa.Concluida);
    }

    [TestMethod]
    public void ConcluirTodosItens_TarefaComQuatroItens_AtualizaPercentualPara100()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new Mock<IRepositorioTarefa>();

        Tarefa tarefa = new Tarefa("Estudar para prova", PrioridadeTarefa.Alta);

        ItemTarefa item1 = new ItemTarefa("Ler capítulo 1") { Concluido = true };
        ItemTarefa item2 = new ItemTarefa("Ler capítulo 2") { Concluido = true };
        ItemTarefa item3 = new ItemTarefa("Fazer exercícios") { Concluido = true };
        ItemTarefa item4 = new ItemTarefa("Revisar resumo");

        tarefa.AdicionarItem(item1);
        tarefa.AdicionarItem(item2);
        tarefa.AdicionarItem(item3);
        tarefa.AdicionarItem(item4);

        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefa.Id)).Returns(tarefa);

        ServicoTarefa servicoTarefa = new ServicoTarefa(repositorioTarefa.Object);

        AlterarConclusaoItemTarefaDto dto = new AlterarConclusaoItemTarefaDto(tarefa.Id, item4.Id, true);
        Result resultado = servicoTarefa.AlterarConclusaoItem(dto);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(100, tarefa.PercentualConcluido);
        Assert.IsTrue(tarefa.Concluida);
    }

    [TestMethod]
    public void ReabrirItem_TarefaComQuatroItensConcluidos_AtualizaParaPendente()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new Mock<IRepositorioTarefa>();

        Tarefa tarefa = new Tarefa("Estudar para prova", PrioridadeTarefa.Alta);

        ItemTarefa item1 = new ItemTarefa("Ler capítulo 1") { Concluido = true };
        ItemTarefa item2 = new ItemTarefa("Ler capítulo 2") { Concluido = true };
        ItemTarefa item3 = new ItemTarefa("Fazer exercícios") { Concluido = true };
        ItemTarefa item4 = new ItemTarefa("Revisar resumo") { Concluido = true };

        tarefa.AdicionarItem(item1);
        tarefa.AdicionarItem(item2);
        tarefa.AdicionarItem(item3);
        tarefa.AdicionarItem(item4);

        Assert.AreEqual(100, tarefa.PercentualConcluido);
        Assert.IsTrue(tarefa.Concluida);

        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefa.Id)).Returns(tarefa);

        ServicoTarefa servicoTarefa = new ServicoTarefa(repositorioTarefa.Object);

        AlterarConclusaoItemTarefaDto dto = new AlterarConclusaoItemTarefaDto(tarefa.Id, item4.Id, false);
        Result resultado = servicoTarefa.AlterarConclusaoItem(dto);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(75, tarefa.PercentualConcluido);
        Assert.IsFalse(tarefa.Concluida);
    }

    [TestMethod]
    public void RemoverItem_TarefaComQuatroItensUmConcluido_RecalculaPercentualPara33()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new Mock<IRepositorioTarefa>();

        Tarefa tarefa = new Tarefa("Estudar para prova", PrioridadeTarefa.Alta);

        ItemTarefa item1 = new ItemTarefa("Ler capítulo 1") { Concluido = true };
        ItemTarefa item2 = new ItemTarefa("Ler capítulo 2");
        ItemTarefa item3 = new ItemTarefa("Fazer exercícios");
        ItemTarefa item4 = new ItemTarefa("Revisar resumo");

        tarefa.AdicionarItem(item1);
        tarefa.AdicionarItem(item2);
        tarefa.AdicionarItem(item3);
        tarefa.AdicionarItem(item4);

        Assert.AreEqual(25, tarefa.PercentualConcluido);

        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefa.Id)).Returns(tarefa);

        ServicoTarefa servicoTarefa = new ServicoTarefa(repositorioTarefa.Object);

        RemoverItemTarefaDto dto = new RemoverItemTarefaDto(tarefa.Id, item2.Id);
        Result resultado = servicoTarefa.RemoverItem(dto);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(33, tarefa.PercentualConcluido);
        Assert.IsFalse(tarefa.Concluida);
    }

    [TestMethod]
    public void RemoverUltimoItem_TarefaComUmItem_DeveFicarSemItensEPercentualZero()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new Mock<IRepositorioTarefa>();

        Tarefa tarefa = new Tarefa("Estudar para prova", PrioridadeTarefa.Alta);

        ItemTarefa item = new ItemTarefa("Ler capítulo 1");
        tarefa.AdicionarItem(item);

        Assert.AreEqual(0, tarefa.PercentualConcluido);

        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefa.Id)).Returns(tarefa);

        ServicoTarefa servicoTarefa = new ServicoTarefa(repositorioTarefa.Object);

        RemoverItemTarefaDto dto = new RemoverItemTarefaDto(tarefa.Id, item.Id);
        Result resultado = servicoTarefa.RemoverItem(dto);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsEmpty(tarefa.Itens);
        Assert.AreEqual(0, tarefa.PercentualConcluido);
        Assert.IsFalse(tarefa.Concluida);
    }

    [TestMethod]
    public void EditarTituloItem_TarefaComItemCadastrado_AlteracaoRefletidaNaLista()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new Mock<IRepositorioTarefa>();

        Tarefa tarefa = new Tarefa("Estudar para prova", PrioridadeTarefa.Alta);
        ItemTarefa item = new ItemTarefa("Ler capítulo 1");
        tarefa.AdicionarItem(item);

        List<Tarefa> tarefas = new() { tarefa };

        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefa.Id)).Returns(tarefa);
        repositorioTarefa.Setup(r => r.SelecionarTodos()).Returns(() => tarefas);

        repositorioTarefa
            .Setup(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Tarefa>()))
            .Callback<Guid, Tarefa>((id, tarefaAtualizada) =>
            {
                int index = tarefas.FindIndex(t => t.Id == id);
                if (index >= 0)
                    tarefas[index].Atualizar(tarefaAtualizada);
            })
            .Returns<Guid, Tarefa>((id, tarefaAtualizada) => tarefas.Any(t => t.Id == id));

        ServicoTarefa servicoTarefa = new ServicoTarefa(repositorioTarefa.Object);

        item.Atualizar(new ItemTarefa("Ler capítulo 1 revisado"));
        repositorioTarefa.Object.Editar(tarefa.Id, tarefa);

        Assert.AreEqual("Ler capítulo 1 revisado", tarefa.Itens[0].Titulo);

        List<ListarTarefasDto> tarefasListadas = servicoTarefa.SelecionarTodos();
        Assert.AreEqual("Ler capítulo 1 revisado", tarefasListadas[0].Itens[0].Titulo);
    }

    [TestMethod]
    public void ListarItens_TarefaComDoisItens_ExibeTodosComStatus()
    {
        Mock<IRepositorioTarefa> repositorioTarefa = new Mock<IRepositorioTarefa>();

        Tarefa tarefa = new Tarefa("Estudar para prova", PrioridadeTarefa.Alta);

        ItemTarefa item1 = new ItemTarefa("Ler capítulo 1") { Concluido = true };
        ItemTarefa item2 = new ItemTarefa("Fazer exercícios") { Concluido = false };

        tarefa.AdicionarItem(item1);
        tarefa.AdicionarItem(item2);

        repositorioTarefa.Setup(r => r.SelecionarPorId(tarefa.Id)).Returns(tarefa);

        ServicoTarefa servicoTarefa = new ServicoTarefa(repositorioTarefa.Object);

        Result<DetalhesTarefaDto> resultado = servicoTarefa.SelecionarPorId(tarefa.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.HasCount(2, resultado.Value.Itens);

        Assert.AreEqual("Ler capítulo 1", resultado.Value.Itens[0].Titulo);
        Assert.IsTrue(resultado.Value.Itens[0].Concluido);

        Assert.AreEqual("Fazer exercícios", resultado.Value.Itens[1].Titulo);
        Assert.IsFalse(resultado.Value.Itens[1].Concluido);
    }
}
