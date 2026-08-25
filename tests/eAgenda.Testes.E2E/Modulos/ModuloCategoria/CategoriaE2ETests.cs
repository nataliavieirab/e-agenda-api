using System.Text.RegularExpressions;
using eAgenda.Testes.E2E.Compartilhado;
using Microsoft.Playwright;

namespace eAgenda.Testes.E2E.Modulos.ModuloCategoria;

[TestClass]
public sealed class CategoriaE2ETests : E2ETestsBase
{
    [TestMethod]
    public async Task Cadastrar_ComDadosValido_RegistraCategoria()
    {
        CategoriaFormPage formPage = new(Page, UrlBase);
        CategoriaListarPage listarPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Mercado");
        await formPage.ConfirmarAsync();

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(listarPage.NomeDaCategoria("Mercado")).ToBeVisibleAsync();
        await Expect(listarPage.EstadoVazio).Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Cadastrar_SemTitulo_ImpedeCadastro_RetornaMensagemDeErro()
    {
        CategoriaFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();

        await formPage.ConfirmarAsync();

        await Expect(Page.GetByText("O campo \"Título\" deve ser preenchido.", new() { Exact = true }))
            .ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Cadastrar_ComTituloDuplicado_ImpedeCadastro_RetornaMensagemDeErro()
    {
        CategoriaFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync(titulo: "Mercado");
        await formPage.ConfirmarAsync();

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync(titulo: "Mercado");
        await formPage.ConfirmarAsync();

        await Expect(Page.GetByText("Já existe uma categoria com este título.", new() { Exact = true }))
            .ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Editar_ComDadosValidos_AtualizaCategoria()
    {
        await CadastrarCategoriaAsync("Mercado");

        CategoriaFormPage formPage = new(Page, UrlBase);
        CategoriaListarPage listarPage = new(Page, UrlBase);

        await listarPage.EditarAsync("Mercado");

        await formPage.PreencherAsync("Supermercado");
        await formPage.ConfirmarAsync();

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(listarPage.NomeDaCategoria("Supermercado")).ToBeVisibleAsync();
        await Expect(listarPage.NomeDaCategoria("Mercado")).Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Listar_DespesasDaCategoria_ExibeDespesas()
    {
        await CadastrarCategoriaAsync("Limpeza");
        await CadastrarDespesaAsync("Detergente", "Limpeza");
        await CadastrarDespesaAsync("Esponja", "Limpeza");

        await Page.GotoAsync($"{UrlBase}/Despesa/Listar");

        await Expect(Page).ToHaveURLAsync($"{UrlBase}/Despesa/Listar");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Detergente", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Esponja", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByText("Limpeza", new() { Exact = true })).ToHaveCountAsync(2);
        await Expect(Page.GetByText("Nenhuma despesa cadastrada.", new() { Exact = true }))
            .Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Excluir_SemDespesasVinculadas_DeletaCategoria()
    {
        await CadastrarCategoriaAsync("Limpeza");

        CategoriaListarPage listarPage = new(Page, UrlBase);
        CategoriaExcluirPage excluirPage = new(Page);

        await listarPage.ExcluirAsync("Limpeza");

        await Expect(Page).ToHaveURLAsync(
            new Regex($"{Regex.Escape(UrlBase)}/Categoria/Excluir/.*")
        );

        await Expect(excluirPage.MensagemConfirmacao).ToBeVisibleAsync();

        await excluirPage.ConfirmarAsync();

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(listarPage.NomeDaCategoria("Limpeza")).Not.ToBeVisibleAsync();
        await Expect(listarPage.EstadoVazio).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Excluir_ComDespesasVinculadas_ImpedeDelecao_RetornaMensagemDeErro()
    {
        await CadastrarCategoriaAsync("Limpeza");
        await CadastrarDespesaAsync("Detergente", "Limpeza");

        CategoriaListarPage listarPage = new(Page, UrlBase);
        CategoriaExcluirPage excluirPage = new(Page);

        await listarPage.IrParaAsync();
        await listarPage.ExcluirAsync("Limpeza");

        await Expect(Page).ToHaveURLAsync(
            new Regex($"{Regex.Escape(UrlBase)}/Categoria/Excluir/.*")
        );

        await Expect(excluirPage.MensagemConfirmacao).ToBeVisibleAsync();

        await excluirPage.ConfirmarAsync();

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(listarPage.MensagemErro).ToContainTextAsync(
            "Não é possível excluir esta categoria, pois ela possui despesas vinculadas."
        );
        await Expect(listarPage.NomeDaCategoria("Limpeza")).ToBeVisibleAsync();
        await Expect(listarPage.EstadoVazio).Not.ToBeVisibleAsync();
    }

    private async Task CadastrarCategoriaAsync(string titulo)
    {
        CategoriaFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync(titulo);
        await formPage.ConfirmarAsync();

        CategoriaListarPage listarPage = new(Page, UrlBase);

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
    }

    private async Task CadastrarDespesaAsync(string descricao, string tituloCategoria)
    {
        await Page.GotoAsync($"{UrlBase}/Despesa/Cadastrar");

        await Page.GetByLabel("Descrição").FillAsync(descricao);
        await Page.GetByLabel("Data de Ocorrência").FillAsync(DateTime.Today.ToString("yyyy-MM-dd"));
        await Page.GetByLabel("Valor").FillAsync("50");
        await Page.GetByLabel("Forma de Pagamento").SelectOptionAsync("AVista");
        await Page.GetByLabel("Categorias").SelectOptionAsync(new SelectOptionValue { Label = tituloCategoria });

        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirmar", Exact = true })
            .ClickAsync();

        await Expect(Page).ToHaveURLAsync($"{UrlBase}/Despesa/Listar");
    }
}
