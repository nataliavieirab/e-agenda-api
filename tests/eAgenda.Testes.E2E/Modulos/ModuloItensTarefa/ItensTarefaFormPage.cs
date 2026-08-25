using Microsoft.Playwright;

namespace GeradorDeProvas.Testes.E2E.Modulos.ModuloTarefa;

public sealed class ItemTarefaFormPage(
    IPage page,
    string urlBase
)
{
    public string UrlCadastrar => $"{urlBase}/Tarefa/AdicionarItem";
    public string UrlEditar => $"{urlBase}/Tarefa/EditarItem";

    public ILocator Titulo => page.GetByLabel("Título");

    public async Task IrParaCadastroAsync()
    {
        await page.GotoAsync(UrlCadastrar);
    }

    public async Task IrParaEdicaoAsync()
    {
        await page.GotoAsync(UrlEditar);
    }

    public async Task PreencherAsync(string titulo)
    {
        await Titulo.FillAsync(titulo);
    }

    public async Task ConfirmarAsync()
    {
        await page.GetByRole(
            AriaRole.Button,
            new() { Name = "Adicinar", Exact = true }
        ).ClickAsync();
    }
}
