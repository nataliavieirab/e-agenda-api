using Microsoft.Playwright;

namespace eAgenda.Testes.E2E.Modulos.ModuloCategoria;

public sealed class CategoriaExcluirPage(
    IPage page
)
{
    public ILocator MensagemConfirmacao => page.GetByText(
        "Deseja realmente excluir esta categoria?",
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
