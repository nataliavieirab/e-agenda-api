using Microsoft.Playwright;

namespace eAgenda.Testes.E2E.Modulos.ModuloItemTarefa;

public class ItemTarefaListarPage(IPage page)
{
    public ILocator BadgePercentual => page.Locator(".badge.text-bg-primary");

    public ILocator ItemPorTitulo(string titulo) =>
        page.Locator(".list-group-item").Filter(new() { HasText = titulo });

    public async Task AlternarConclusaoItemAsync(string tituloItem)
    {
        var itemRow = ItemPorTitulo(tituloItem);
        await itemRow.GetByRole(AriaRole.Button, new() { Name = "Concluir" })
            .Or(itemRow.GetByRole(AriaRole.Button, new() { Name = "Reabrir" })).ClickAsync();
    }
}