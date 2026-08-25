using Microsoft.Playwright;

namespace eAgenda.Testes.E2E.Modulos.ModuloTarefa;

public sealed class TarefaFormPage(
    IPage page,
    string urlBase
)
{
    public string UrlCadastrar => $"{urlBase}/Tarefa/Cadastrar";
    public string UrlEditar => $"{urlBase}/Tarefa/Editar";

    public ILocator Titulo => page.GetByLabel("Título");
    public ILocator Prioridade => page.GetByLabel("Prioridade");
    public ILocator NovoItem => page.GetByLabel("Novo Item");

    public ILocator NomeDoItem(string titulo) => page.GetByText(
        titulo,
        new() { Exact = true }
    );

    public async Task IrParaCadastroAsync()
    {
        await page.GotoAsync(UrlCadastrar);
    }

    public async Task IrParaEdicaoAsync()
    {
        await page.GotoAsync(UrlEditar);
    }

    public async Task PreencherAsync(string titulo, string? prioridade = null)
    {
        await Titulo.FillAsync(titulo);

        if (prioridade is not null)
            await Prioridade.SelectOptionAsync(new SelectOptionValue { Label = prioridade });
    }

    public async Task LimparPrioridadeAsync()
    {
        await Prioridade.EvaluateAsync("""
            el => {
                const option = document.createElement('option');
                option.value = '';
                option.textContent = '';
                el.insertBefore(option, el.firstChild);
                el.value = '';
            }
            """);
    }

    public async Task AdicionarItemAsync(string titulo)
    {
        await NovoItem.FillAsync(titulo);
        await page.GetByRole(
            AriaRole.Button,
            new() { Name = "Adicionar", Exact = true }
        ).ClickAsync();
    }

    public async Task ConfirmarAsync()
    {
        await page.GetByRole(
            AriaRole.Button,
            new() { Name = "Confirmar", Exact = true }
        ).ClickAsync();
    }
}
