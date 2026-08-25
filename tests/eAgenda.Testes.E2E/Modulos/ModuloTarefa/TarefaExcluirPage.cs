using Microsoft.Playwright;

namespace eAgenda.Testes.E2E.Modulos.ModuloTarefa;

public sealed class ContatoExcluirPage(
    IPage page
)
{
    public ILocator MensagemConfirmacao => page.GetByText(
        "Deseja realmente excluir esta tarefa?",
        new() { Exact = true }
    );

    public async Task ConfirmarAsync()
    {
        await page.GetByRole(
            AriaRole.Button,
            new() { Name = "Confirmar", Exact = true }
        ).ClickAsync();
    }
}
