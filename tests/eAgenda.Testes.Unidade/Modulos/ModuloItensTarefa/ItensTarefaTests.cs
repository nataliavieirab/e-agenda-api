using eAgenda.Dominio.Modulos.ModuloTarefa;

namespace eAgenda.Testes.Unidade.Modulos.ModuloItensTarefa;

[TestClass]
public sealed class ItensTarefaTests
{
    [TestMethod]
    public void Validar_AdicionarItem_TarefaExistente()
    {
        Tarefa tarefa = new("Faxina", PrioridadeTarefa.Alta);

        ItemTarefa itemTarefa = new("Arrumar a cama");

        tarefa.AdicionarItem(itemTarefa);

        List<string> erros = tarefa.Validar();

        Assert.HasCount(0, erros);
        Assert.HasCount(1, tarefa.Itens);
        Assert.AreEqual(0, tarefa.PercentualConcluido);
    }

    [TestMethod]
    public void Validar_AdicionarItem_SemTitulo()
    {
        Tarefa tarefa = new("Faxina", PrioridadeTarefa.Alta);

        ItemTarefa itemTarefa = new(string.Empty);

        tarefa.AdicionarItem(itemTarefa);

        List<string> erros = tarefa.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Título\" deve conter entre 2 e 100 caracteres.", erros.First());
        Assert.HasCount(1, tarefa.Itens);
        Assert.AreEqual(0, tarefa.PercentualConcluido);
    }

    [TestMethod]
    public void Validar_AdicionarItem_TituloAbaixoMinimo()
    {
        Tarefa tarefa = new("Faxina", PrioridadeTarefa.Alta);

        ItemTarefa itemTarefa = new(new string('A', 1));

        tarefa.AdicionarItem(itemTarefa);

        List<string> erros = tarefa.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Título\" deve conter entre 2 e 100 caracteres.", erros.First());
        Assert.HasCount(1, tarefa.Itens);
        Assert.AreEqual(0, tarefa.PercentualConcluido);
    }

    [TestMethod]
    public void Validar_AdicionarItem_TituloAcimaMaximo()
    {
        Tarefa tarefa = new("Faxina", PrioridadeTarefa.Alta);

        ItemTarefa itemTarefa = new(new string('A', 101));

        tarefa.AdicionarItem(itemTarefa);

        List<string> erros = tarefa.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Título\" deve conter entre 2 e 100 caracteres.", erros.First());
        Assert.HasCount(1, tarefa.Itens);
        Assert.AreEqual(0, tarefa.PercentualConcluido);
    }

    [TestMethod]
    public void ConcluirItem_DeveAtualizarPercentualPara25_TarefaPermanecePendente()
    {
        Tarefa tarefa = new("Faxina", PrioridadeTarefa.Alta);

        ItemTarefa item1 = new("Arrumar a cama");
        ItemTarefa item2 = new("Lavar a louça");
        ItemTarefa item3 = new("Varrer a sala");
        ItemTarefa item4 = new("Limpar o banheiro");

        tarefa.AdicionarItem(item1);
        tarefa.AdicionarItem(item2);
        tarefa.AdicionarItem(item3);
        tarefa.AdicionarItem(item4);

        tarefa.AlterarConclusaoItem(item1.Id, true);

        Assert.AreEqual(25, tarefa.PercentualConcluido);
        Assert.IsFalse(tarefa.Concluida);
        Assert.IsNull(tarefa.DataConclusao);
    }

    [TestMethod]
    public void ConcluirTodosOsItens_DeveAtualizarPercentualPara100_TarefaConcluida()
    {
        Tarefa tarefa = new("Faxina", PrioridadeTarefa.Alta);

        ItemTarefa item1 = new("Arrumar a cama") { Concluido = true };
        ItemTarefa item2 = new("Lavar a louça") { Concluido = true };
        ItemTarefa item3 = new("Varrer a sala") { Concluido = true };
        ItemTarefa item4 = new("Limpar o banheiro");

        tarefa.AdicionarItem(item1);
        tarefa.AdicionarItem(item2);
        tarefa.AdicionarItem(item3);
        tarefa.AdicionarItem(item4);

        tarefa.AlterarConclusaoItem(item4.Id, true);

        Assert.AreEqual(100, tarefa.PercentualConcluido);
        Assert.IsTrue(tarefa.Concluida);
        Assert.AreEqual(DateTime.Today, tarefa.DataConclusao);
    }

    [TestMethod]
    public void ReabrirItem_DeveReduzirPercentualPara75_TarefaVoltaPendente()
    {
        Tarefa tarefa = new("Faxina", PrioridadeTarefa.Alta);

        ItemTarefa item1 = new("Arrumar a cama") { Concluido = true };
        ItemTarefa item2 = new("Lavar a louça") { Concluido = true };
        ItemTarefa item3 = new("Varrer a sala") { Concluido = true };
        ItemTarefa item4 = new("Limpar o banheiro") { Concluido = true };

        tarefa.AdicionarItem(item1);
        tarefa.AdicionarItem(item2);
        tarefa.AdicionarItem(item3);
        tarefa.AdicionarItem(item4);

        tarefa.AlterarConclusaoItem(item4.Id, false);

        Assert.AreEqual(75, tarefa.PercentualConcluido);
        Assert.IsFalse(tarefa.Concluida);
        Assert.IsNull(tarefa.DataConclusao);
    }

    [TestMethod]
    public void RemoverItem_DeveRecalcularPercentualPara33_TarefaPermanecePendente()
    {
        Tarefa tarefa = new("Faxina", PrioridadeTarefa.Alta);

        ItemTarefa item1 = new("Arrumar a cama") { Concluido = true };
        ItemTarefa item2 = new("Lavar a louça");
        ItemTarefa item3 = new("Varrer a sala");
        ItemTarefa item4 = new("Limpar o banheiro");

        tarefa.AdicionarItem(item1);
        tarefa.AdicionarItem(item2);
        tarefa.AdicionarItem(item3);
        tarefa.AdicionarItem(item4);

        Assert.AreEqual(25, tarefa.PercentualConcluido);

        tarefa.RemoverItem(item2.Id);

        Assert.HasCount(3, tarefa.Itens);
        Assert.AreEqual(33, tarefa.PercentualConcluido);
        Assert.IsFalse(tarefa.Concluida);
        Assert.IsNull(tarefa.DataConclusao);
    }

    [TestMethod]
    public void RemoverUltimoItem_DeveZerarPercentual_TarefaSemItens()
    {
        Tarefa tarefa = new("Faxina", PrioridadeTarefa.Alta);

        ItemTarefa item = new("Arrumar a cama");
        tarefa.AdicionarItem(item);

        Assert.HasCount(1, tarefa.Itens);
        Assert.AreEqual(0, tarefa.PercentualConcluido);
        // Act: remover o único item
        tarefa.RemoverItem(item.Id);

        // Assert
        Assert.IsEmpty(tarefa.Itens);
        Assert.AreEqual(0, tarefa.PercentualConcluido);
        Assert.IsFalse(tarefa.Concluida);
        Assert.IsNull(tarefa.DataConclusao);
    }

    [TestMethod]
    public void AtualizarTituloItem_DeveAtualizarNaListaDeItens()
    {
        Tarefa tarefa = new("Faxina", PrioridadeTarefa.Alta);
        ItemTarefa item = new("Arrumar a cama");
        tarefa.AdicionarItem(item);

        ItemTarefa itemAtualizado = new("Arrumar o quarto");
        item.Atualizar(itemAtualizado);

        Assert.HasCount(1, tarefa.Itens);
        Assert.AreEqual("Arrumar o quarto", tarefa.Itens[0].Titulo);
        Assert.IsFalse(tarefa.Itens[0].Concluido);
    }

}