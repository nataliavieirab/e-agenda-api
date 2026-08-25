using eAgenda.Testes.E2E.Compartilhado;
using eAgenda.Testes.E2E.Modulos.ModuloCategoria;
using Microsoft.Playwright;

namespace eAgenda.Testes.E2E.Modulos.ModuloDespesa
{
    [TestClass]
    public class DespesaE2ETests : E2ETestsBase
    {
        [TestMethod]
        public async Task DeveCadastrar_DadosValidos_UmaCategoria()
        {
            CategoriaFormPage categoriaPage = new(Page, UrlBase);
            await categoriaPage.IrParaCadastroAsync();
            await categoriaPage.PreencherAsync("Educação");
            await categoriaPage.ConfirmarAsync();

            DespesaFormPage formPage = new(Page, UrlBase);
            await formPage.IrParaCadastroAsync();

            await formPage.PreencherAsync(
                descricao: "Almoço",
                dataOcorrencia: "2026-08-15",
                valor: "100.00",
                formaPagamento: "Débito",
                categorias: new[] { "Educação" }
            );

            await formPage.ConfirmarAsync();

            DespesaListarPage listarPage = new(Page, UrlBase);
            await listarPage.IrParaAsync();

            await Expect(listarPage.NomeDaDespesa("Almoço")).ToBeVisibleAsync();
            await Expect(Page.GetByText("Débito", new() { Exact = true })).ToBeVisibleAsync();
            await Expect(Page.GetByText("Educação", new() { Exact = true })).ToBeVisibleAsync();
        }

        [TestMethod]
        public async Task DeveCadastrar_DadosValidos_MultiplasCategorias()
        {
            CategoriaFormPage categoriaPage = new(Page, UrlBase);

            await categoriaPage.IrParaCadastroAsync();
            await categoriaPage.PreencherAsync("Educação");
            await categoriaPage.ConfirmarAsync();

            await categoriaPage.IrParaCadastroAsync();
            await categoriaPage.PreencherAsync("Alimentação");
            await categoriaPage.ConfirmarAsync();

            DespesaFormPage formPage = new(Page, UrlBase);
            await formPage.IrParaCadastroAsync();

            await formPage.PreencherAsync(
                descricao: "Supermercado",
                dataOcorrencia: "2026-08-15",
                valor: "250.00",
                formaPagamento: "Crédito",
                categorias: new[] { "Educação", "Alimentação" }
            );

            await formPage.ConfirmarAsync();

            DespesaListarPage listarPage = new(Page, UrlBase);
            await listarPage.IrParaAsync();

            await Expect(listarPage.NomeDaDespesa("Supermercado")).ToBeVisibleAsync();
            await Expect(Page.GetByText("Crédito", new() { Exact = true })).ToBeVisibleAsync();
            await Expect(Page.GetByText("Alimentação, Educação", new() { Exact = true })).ToBeVisibleAsync();
        }

        [TestMethod]
        public async Task DeveExibirMensagensObrigatoriedade_SemDescricao()
        {
            CategoriaFormPage categoriaPage = new(Page, UrlBase);

            await categoriaPage.IrParaCadastroAsync();
            await categoriaPage.PreencherAsync("Educação");
            await categoriaPage.ConfirmarAsync();

            DespesaFormPage formPage = new(Page, UrlBase);
            await formPage.IrParaCadastroAsync();

            await formPage.PreencherAsync(
                descricao: string.Empty,
                dataOcorrencia: "2026-08-15",
                valor: "250.00",
                formaPagamento: "Crédito",
                categorias: new[] { "Educação" }
            );

            await formPage.ConfirmarAsync();
            await Expect(Page.GetByText("O campo \"Descrição\" deve ser preenchido.", new() { Exact = true }))
                .ToBeVisibleAsync();

        }
        [TestMethod]
        public async Task DeveEditar_ComDadosValidos()
        {
            CategoriaFormPage categoriaPage = new(Page, UrlBase);

            await categoriaPage.IrParaCadastroAsync();
            await categoriaPage.PreencherAsync("Educação");
            await categoriaPage.ConfirmarAsync();

            DespesaFormPage formPage = new(Page, UrlBase);

            await formPage.IrParaCadastroAsync();
            await formPage.PreencherAsync(
                descricao: "Mercado",
                dataOcorrencia: "2026-08-15",
                valor: "250.00",
                formaPagamento: "Crédito",
                categorias: new[] { "Educação" }
            );
            await formPage.ConfirmarAsync();

            DespesaListarPage listarPage = new(Page, UrlBase);
            await listarPage.IrParaAsync();
            await listarPage.EditarAsync("Mercado");

            await formPage.Descricao.FillAsync("Farmácia");
            await formPage.ConfirmarAsync();

            await Expect(listarPage.NomeDaDespesa("Farmácia")).ToBeVisibleAsync();
        }

        [TestMethod]
        public async Task DeveListarTodasDespesa()
        {
            CategoriaFormPage categoriaPage = new(Page, UrlBase);

            await categoriaPage.IrParaCadastroAsync();
            await categoriaPage.PreencherAsync("Compras");
            await categoriaPage.ConfirmarAsync();

            DespesaFormPage formPage = new(Page, UrlBase);
            DespesaListarPage listarPage = new(Page, UrlBase);

            await formPage.IrParaCadastroAsync();
            await formPage.PreencherAsync(
                descricao: "Mercado",
                dataOcorrencia: "2026-08-15",
                valor: "250.00",
                formaPagamento: "Crédito",
                categorias: new[] { "Compras" }
            );
            await formPage.ConfirmarAsync();

            await formPage.IrParaCadastroAsync();
            await formPage.PreencherAsync(
                descricao: "Farmácia",
                dataOcorrencia: "2026-08-15",
                valor: "3000.00",
                formaPagamento: "Crédito",
                categorias: new[] { "Compras" }
            );
            await formPage.ConfirmarAsync();

            await listarPage.IrParaAsync();
            await Expect(listarPage.NomeDaDespesa("Mercado")).ToBeVisibleAsync();
            await Expect(listarPage.NomeDaDespesa("Farmácia")).ToBeVisibleAsync();
        }

        [TestMethod]
        public async Task DeveExcluirDespesa()
        {
            CategoriaFormPage categoriaPage = new(Page, UrlBase);

            await categoriaPage.IrParaCadastroAsync();
            await categoriaPage.PreencherAsync("Compras");
            await categoriaPage.ConfirmarAsync();

            DespesaFormPage formPage = new(Page, UrlBase);
            DespesaListarPage listarPage = new(Page, UrlBase);

            await formPage.IrParaCadastroAsync();
            await formPage.PreencherAsync(
                descricao: "Mercado",
                dataOcorrencia: "2026-08-15",
                valor: "250.00",
                formaPagamento: "Crédito",
                categorias: new[] { "Compras" }
            );
            await formPage.ConfirmarAsync();

            await listarPage.IrParaAsync();
            await listarPage.ExcluirAsync("Mercado");

            DespesaExcluirPage excluirPage = new(Page);
            await Expect(excluirPage.MensagemConfirmacao).ToBeVisibleAsync();
            await excluirPage.ConfirmarAsync();

            await listarPage.IrParaAsync();
            await Expect(listarPage.NomeDaDespesa("Mercado"))
                .Not.ToBeVisibleAsync();
        }

    }
}