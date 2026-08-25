using eAgenda.Testes.E2E.Compartilhado;
using eAgenda.Testes.E2E.Modulos.ModuloTarefa;
using eAgenda.Testes.E2E.Modulos.ModuloItemTarefa;
using Microsoft.Playwright;

namespace eAgenda.Testes.E2E.Modulos.ModuloItensTarefa;

[TestClass]
public class ItensTarefaE2ETests : E2ETestsBase
{
    [TestMethod]
    public async Task AdicionarItem_TarefaExistente_RegistraItem()
    {
        TarefaFormPage formPage = new(Page, UrlBase);
        TarefaListarPage listarPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Estudar", "Alta");
        await formPage.ConfirmarAsync();

        await listarPage.GerenciarItensAsync("Estudar");

        await formPage.AdicionarItemAsync("Ler capítulo 1");

        await Expect(Page.GetByText("Ler capítulo 1", new() { Exact = true })).ToBeVisibleAsync();

        await listarPage.IrParaAsync();
        await Expect(listarPage.TextoNaTarefa("Estudar", "1")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task ConcluirItem_AtualizaPercentual()
    {
        TarefaFormPage formPage = new(Page, UrlBase);
        TarefaListarPage listarPage = new(Page, UrlBase);
        ItemTarefaListarPage itemListarPage = new(Page);

        // Criar tarefa com 4 itens
        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Estudar", "Alta");
        await formPage.ConfirmarAsync();

        await listarPage.GerenciarItensAsync("Estudar");

        await formPage.AdicionarItemAsync("Ler capítulo 1");
        await formPage.AdicionarItemAsync("Ler capítulo 2");
        await formPage.AdicionarItemAsync("Ler capítulo 3");
        await formPage.AdicionarItemAsync("Ler capítulo 4");

        await itemListarPage.AlternarConclusaoItemAsync("Ler capítulo 1");

        await listarPage.IrParaAsync();

        await Expect(listarPage.TextoNaTarefa("Estudar", "25%")).ToBeVisibleAsync();
        await Expect(listarPage.Status("Estudar")).ToContainTextAsync("Pendente");
    }

    [TestMethod]
    public async Task ConcluirTodosItens_AtualizaStatusParaConcluida()
    {
        TarefaFormPage formPage = new(Page, UrlBase);
        TarefaListarPage listarPage = new(Page, UrlBase);
        ItemTarefaListarPage itemListarPage = new(Page);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Projeto Final", "Alta");
        await formPage.ConfirmarAsync();

        await listarPage.GerenciarItensAsync("Projeto Final");

        await formPage.AdicionarItemAsync("Análise de requisitos");
        await formPage.AdicionarItemAsync("Design");
        await formPage.AdicionarItemAsync("Implementação");
        await formPage.AdicionarItemAsync("Testes");

        await itemListarPage.AlternarConclusaoItemAsync("Análise de requisitos");
        await itemListarPage.AlternarConclusaoItemAsync("Design");
        await itemListarPage.AlternarConclusaoItemAsync("Implementação");
        await itemListarPage.AlternarConclusaoItemAsync("Testes");

        await listarPage.IrParaAsync();

        await Expect(listarPage.TextoNaTarefa("Projeto Final", "100%")).ToBeVisibleAsync();
        await Expect(listarPage.Status("Projeto Final")).ToContainTextAsync("Concluída");
    }

    [TestMethod]
    public async Task RemoverItem_RecalculaPercentual()
    {
        TarefaFormPage formPage = new(Page, UrlBase);
        TarefaListarPage listarPage = new(Page, UrlBase);
        ItemTarefaListarPage itemListarPage = new(Page);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Projeto Final", "Alta");
        await formPage.ConfirmarAsync();

        await listarPage.GerenciarItensAsync("Projeto Final");

        await formPage.AdicionarItemAsync("Análise de requisitos");
        await formPage.AdicionarItemAsync("Design");
        await formPage.AdicionarItemAsync("Implementação");
        await formPage.AdicionarItemAsync("Testes");

        await itemListarPage.AlternarConclusaoItemAsync("Análise de requisitos");

        ILocator itemParaRemover = itemListarPage.ItemPorTitulo("Testes");
        await itemParaRemover.GetByRole(AriaRole.Button, new() { Name = "Remover", Exact = true }).ClickAsync();

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Expect(itemListarPage.ItemPorTitulo("Testes")).Not.ToBeVisibleAsync();

        await Expect(itemListarPage.BadgePercentual).ToContainTextAsync("33");

        await Expect(itemListarPage.ItemPorTitulo("Análise de requisitos")).ToBeVisibleAsync();
        await Expect(itemListarPage.ItemPorTitulo("Design")).ToBeVisibleAsync();
        await Expect(itemListarPage.ItemPorTitulo("Implementação")).ToBeVisibleAsync();
    }
}