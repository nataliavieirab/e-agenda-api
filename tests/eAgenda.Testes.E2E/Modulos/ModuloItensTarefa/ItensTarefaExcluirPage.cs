using Microsoft.Playwright;

namespace eAgenda.Testes.E2E.Modulos.ModuloItensTarefa;

public class ItensTarefaExcluirPage(IPage page)
{
    private readonly IPage _page = page;
    public ILocator BotaoRemover => _page.GetByRole(AriaRole.Button, new() { Name = "Remover" });

    public async Task ConfirmarAsync() => await BotaoRemover.ClickAsync();
}
