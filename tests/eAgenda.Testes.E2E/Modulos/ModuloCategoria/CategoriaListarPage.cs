using Microsoft.Playwright;

namespace eAgenda.Testes.E2E.Modulos.ModuloCategoria;

public sealed class CategoriaListarPage(
    IPage page,
    string urlBase
)
{
    public string Url => $"{urlBase}/Categoria/Listar";

    public ILocator Titulo => page.GetByRole(
        AriaRole.Heading,
        new() { Name = "Listagem de Categorias" }
    );

    public ILocator CadastrarNovo => page.GetByRole(
            AriaRole.Link,
            new() { Name = "Cadastrar Novo" }
        );

    public ILocator EstadoVazio => page.GetByText(
        "Nenhuma categoria cadastrada.",
        new() { Exact = true }
    );

    public ILocator MensagemErro => page.Locator(".alert-danger");

    public ILocator NomeDaCategoria(string nome) => page.GetByRole(
        AriaRole.Heading,
        new() { Name = nome, Exact = true }
    );

    public async Task IrParaAsync()
    {
        await page.GotoAsync(Url);
    }

    public async Task EditarAsync(string nome)
    {
        await CardPorNome(nome).GetByRole(
            AriaRole.Link,
            new() { Name = "Editar", Exact = true }
        ).ClickAsync();
    }

    public async Task ExcluirAsync(string nome)
    {
        await CardPorNome(nome).GetByRole(
            AriaRole.Link,
            new() { Name = "Excluir", Exact = true }
        ).ClickAsync();
    }

    private ILocator CardPorNome(string nome)
    {
        ILocator nomeCategoria = NomeDaCategoria(nome);

        return page.Locator(".card").Filter(new() { Has = nomeCategoria });
    }
}
