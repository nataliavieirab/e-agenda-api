using Microsoft.Playwright;

namespace eAgenda.Testes.E2E.Modulos.ModuloDespesa;

public sealed class DespesaListarPage(
    IPage page,
    string urlBase
)
{
    public string Url => $"{urlBase}/Despesa/Listar";

    public ILocator Titulo => page.GetByRole(
        AriaRole.Heading,
        new() { Name = "Listagem de Despesas" }
    );

    public ILocator CadastrarNovo => page.GetByRole(
        AriaRole.Link,
        new() { Name = "Cadastrar Nova" }
    );

    public ILocator EstadoVazio => page.GetByText(
        "Nenhuma despesa cadastrada.",
        new() { Exact = true }
    );

    public ILocator NomeDaDespesa(string assunto) => page.GetByRole(
        AriaRole.Heading,
        new() { Name = assunto, Exact = true }
    );

    public async Task IrParaAsync()
    {
        await page.GotoAsync(Url);
    }

    public async Task EditarAsync(string assunto)
    {
        await CardPorAssunto(assunto).GetByRole(
            AriaRole.Link,
            new() { Name = "Editar", Exact = true }
        ).ClickAsync();
    }

    public async Task ExcluirAsync(string assunto)
    {
        await CardPorAssunto(assunto).GetByRole(
            AriaRole.Link,
            new() { Name = "Excluir", Exact = true }
        ).ClickAsync();
    }

    private ILocator CardPorAssunto(string assunto)
    {
        ILocator nomeDespesa = NomeDaDespesa(assunto);

        return page.Locator(".card").Filter(new() { Has = nomeDespesa });
    }
}
