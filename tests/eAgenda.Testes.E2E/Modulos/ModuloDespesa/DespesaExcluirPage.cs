using Microsoft.Playwright;

namespace eAgenda.Testes.E2E.Modulos.ModuloDespesa;

public class DespesaExcluirPage(IPage page)
{
    private readonly IPage _page = page;
    public ILocator MensagemConfirmacao => _page.GetByText("Deseja realmente excluir esta despesa?");
    public ILocator BotaoConfirmar => _page.GetByRole(AriaRole.Button, new() { Name = "Confirmar" });

    public async Task ConfirmarAsync() => await BotaoConfirmar.ClickAsync();
}

