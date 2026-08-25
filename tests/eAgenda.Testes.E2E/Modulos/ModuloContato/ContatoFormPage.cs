using Microsoft.Playwright;

namespace eAgenda.Testes.E2E.Modulos.ModuloContato;

public sealed class ContatoFormPage(
    IPage page,
    string urlBase
)
{
    public string UrlCadastrar => $"{urlBase}/Contato/Cadastrar";
    public string UrlEditar => $"{urlBase}/Contato/Editar";

    public ILocator Nome => page.GetByLabel("Nome");
    public ILocator Email => page.GetByLabel("E-mail");
    public ILocator Telefone => page.GetByLabel("Telefone");
    public ILocator Cargo => page.GetByLabel("Cargo");
    public ILocator Empresa => page.GetByLabel("Empresa");

    public async Task IrParaCadastroAsync()
    {
        await page.GotoAsync(UrlCadastrar);
    }

    public async Task IrParaEdicaoAsync()
    {
        await page.GotoAsync(UrlEditar);
    }

    public async Task PreencherAsync(string nome, string email, string telefone, string cargo, string empresa)
    {
        await Nome.FillAsync(nome);
        await Email.FillAsync(email);
        await Telefone.FillAsync(telefone);
        await Cargo.FillAsync(cargo);
        await Empresa.FillAsync(empresa);
    }

    public async Task ConfirmarAsync()
    {
        await page.GetByRole(
            AriaRole.Button,
            new() { Name = "Confirmar", Exact = true }
        ).ClickAsync();
    }
}
