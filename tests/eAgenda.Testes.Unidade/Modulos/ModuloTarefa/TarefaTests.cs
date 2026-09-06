using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.Modulos.ModuloTarefa;
namespace eAgenda.Testes.Unidade.Modulos.ModuloTarefa;

[TestClass]
public sealed class TarefaTests
{
    [TestMethod]
    public void Validar_DeveCadastrar_ComDadosValidos()
    {
        Tarefa tarefa = new("Faxina", PrioridadeTarefa.Alta);

        IReadOnlyList<ErroValidacao> erros = tarefa.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_RecemCriada_ComDadosValidos()
    {
        Tarefa tarefa = new("Faxina", PrioridadeTarefa.Alta);

        IReadOnlyList<ErroValidacao> erros = tarefa.Validar();

        Assert.HasCount(0, erros);
        Assert.IsFalse(tarefa.Concluida);
        Assert.AreEqual(0, tarefa.PercentualConcluido);
        Assert.AreEqual(DateTime.Today, tarefa.DataCriacao);
        Assert.IsNull(tarefa.DataConclusao);
    }

    [TestMethod]
    public void Validar_DeveCadastrar_ComItens()
    {
        Tarefa tarefa = new("Faxina", PrioridadeTarefa.Alta);

        ItemTarefa itemTarefa = new("Limpar o banheiro");
        ItemTarefa itemTarefa2 = new("Arrumar a cama");

        tarefa.AdicionarItem(itemTarefa);
        tarefa.AdicionarItem(itemTarefa2);

        IReadOnlyList<ErroValidacao> erros = tarefa.Validar();

        Assert.HasCount(0, erros);
        Assert.HasCount(2, tarefa.Itens);
        Assert.AreEqual(0, tarefa.PercentualConcluido);
    }

    [TestMethod]
    public void Validar_ComCamposObrigatoriosEmBranco_DeveRetornarErro()
    {
        Tarefa tarefa = new(string.Empty, (PrioridadeTarefa)999);

        IReadOnlyList<ErroValidacao> erros = tarefa.Validar();

        Assert.HasCount(2, erros);
        CollectionAssert.AreEquivalent(
            new[]
            {
                nameof(Tarefa.Titulo),
                nameof(Tarefa.Prioridade)
            },
            erros.Select(e => e.Campo).ToArray()
        );
        CollectionAssert.AreEquivalent(
            new[]
            {
                "O campo \"Título\" deve ser preenchido.",
                "O campo \"Prioridade\" deve ser preenchido."
            },
            erros.Select(e => e.Mensagem).ToArray()
        );
    }

    [TestMethod]
    public void Validar_ComTituloCurto_DeveRetornarErro()
    {
        Tarefa tarefa = new(new string('A', 1), PrioridadeTarefa.Alta);

        IReadOnlyList<ErroValidacao> erros = tarefa.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(nameof(Tarefa.Titulo), erros.First().Campo);
        Assert.AreEqual(
            "O campo \"Título\" deve conter no mínimo 2 caracteres.",
            erros.First().Mensagem
        );
    }

    [TestMethod]
    public void Validar_ComTituloTamanhoLimite()
    {
        Tarefa tarefa = new(new string('A', 2), PrioridadeTarefa.Alta);

        IReadOnlyList<ErroValidacao> erros = tarefa.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_ComTituloLongo_DeveRetornarErro()
    {
        Tarefa tarefa = new(new string('A', 101), PrioridadeTarefa.Alta);

        IReadOnlyList<ErroValidacao> erros = tarefa.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(nameof(Tarefa.Titulo), erros.First().Campo);
        Assert.AreEqual(
            "O campo \"Título\" deve conter no máximo 100 caracteres.",
            erros.First().Mensagem
        );
    }

    [TestMethod]
    public void Validar_ComTituloTamanhoMaximo()
    {
        Tarefa tarefa = new(new string('A', 100), PrioridadeTarefa.Alta);

        IReadOnlyList<ErroValidacao> erros = tarefa.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_ComPrioridadeInvalida_DeveRetornarErro()
    {
        Tarefa tarefa = new("Faxina", (PrioridadeTarefa)999);

        IReadOnlyList<ErroValidacao> erros = tarefa.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(nameof(Tarefa.Prioridade), erros.First().Campo);
        Assert.AreEqual(
            "O campo \"Prioridade\" deve ter valores válidos.",
            erros.First().Mensagem
        );
    }

    [TestMethod]
    public void Atualizar_DeveAtualizar_ComDadosValidos()
    {
        Tarefa tarefa = new("Faxina", PrioridadeTarefa.Alta);
        tarefa.AdicionarItem(new ItemTarefa("Limpar a casa"));

        Tarefa tarefaAtualizada = new("Limpeza", PrioridadeTarefa.Baixa);
        tarefaAtualizada.AdicionarItem(new ItemTarefa("Varrer o quintal"));
        tarefaAtualizada.DataConclusao = DateTime.Today.AddDays(-1);
        tarefaAtualizada.Concluida = true;
        tarefaAtualizada.PercentualConcluido = 100;

        tarefa.Atualizar(tarefaAtualizada);

        Assert.AreEqual("Limpeza", tarefa.Titulo);
        Assert.AreEqual(PrioridadeTarefa.Baixa, tarefa.Prioridade);
        Assert.AreEqual(tarefaAtualizada.DataCriacao, tarefa.DataCriacao);
        Assert.AreEqual(tarefaAtualizada.DataConclusao, tarefa.DataConclusao);
        Assert.AreEqual(tarefaAtualizada.Concluida, tarefa.Concluida);
        Assert.AreEqual(tarefaAtualizada.PercentualConcluido, tarefa.PercentualConcluido);
        Assert.AreSame(tarefaAtualizada.Itens, tarefa.Itens);
        Assert.HasCount(1, tarefa.Itens);
        Assert.AreEqual("Varrer o quintal", tarefa.Itens[0].Titulo);
    }

    [TestMethod]
    public void AlterarConclusaoManual_DeveConcluirTarefa_ComDadosValidos()
    {
        Tarefa tarefa = new("Faxina", PrioridadeTarefa.Alta);

        bool resultado = tarefa.AlterarConclusaoManual(true);

        Assert.IsTrue(resultado);
        Assert.IsTrue(tarefa.Concluida);
        Assert.AreEqual(100, tarefa.PercentualConcluido);
        Assert.AreEqual(DateTime.Today, tarefa.DataConclusao);
    }

    [TestMethod]
    public void AlterarConclusaoManual_DeveReabrirTarefa_ComDadosValidos()
    {
        Tarefa tarefa = new("Faxina", PrioridadeTarefa.Alta);
        tarefa.AlterarConclusaoManual(true);

        bool resultado = tarefa.AlterarConclusaoManual(false);

        Assert.IsTrue(resultado);
        Assert.IsFalse(tarefa.Concluida);
        Assert.AreEqual(0, tarefa.PercentualConcluido);
        Assert.IsNull(tarefa.DataConclusao);
    }
}
