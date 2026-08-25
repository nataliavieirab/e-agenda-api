using Microsoft.Playwright;

namespace eAgenda.Testes.E2E.Modulos.ModuloDespesa;

public sealed class DespesaFormPage(
    IPage page,
    string urlBase
)
{
    public string UrlCadastrar => $"{urlBase}/Despesa/Cadastrar";
    public string UrlEditar => $"{urlBase}/Despesa/Editar";

    // Campos do formulário
    public ILocator Descricao => page.GetByLabel("Descrição");
    public ILocator DataOcorrencia => page.GetByLabel("Data de Ocorrência");
    public ILocator Valor => page.GetByLabel("Valor");
    public ILocator FormaDePagamento => page.GetByLabel("Forma de Pagamento");
    public ILocator Categorias => page.GetByLabel("Categorias");

    // Navegação
    public async Task IrParaCadastroAsync()
    {
        await page.GotoAsync(UrlCadastrar);
    }

    public async Task IrParaEdicaoAsync()
    {
        await page.GotoAsync(UrlEditar);
    }

    // Preenchimento
    public async Task PreencherAsync(
        string descricao,
        string dataOcorrencia,
        string valor,
        string formaPagamento,
        string[] categorias
    )
    {
        await Descricao.FillAsync(descricao);
        await DataOcorrencia.FillAsync(dataOcorrencia);
        await Valor.FillAsync(valor);

        // Seleciona forma de pagamento (enum exibido como texto no select)
        await FormaDePagamento.SelectOptionAsync(new SelectOptionValue { Label = formaPagamento });

        // Seleciona múltiplas categorias de uma vez
        await Categorias.SelectOptionAsync(
            categorias.Select(c => new SelectOptionValue { Label = c }).ToArray()
        );
    }

    // Confirmação
    public async Task ConfirmarAsync()
    {
        await page.GetByRole(
            AriaRole.Button,
            new() { Name = "Confirmar", Exact = true }
        ).ClickAsync();
    }
}
