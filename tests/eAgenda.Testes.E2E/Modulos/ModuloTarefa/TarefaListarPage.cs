using Microsoft.Playwright;

namespace eAgenda.Testes.E2E.Modulos.ModuloTarefa;

public sealed class TarefaListarPage(
    IPage page,
    string urlBase
)
{
    public string Url => $"{urlBase}/Tarefa/Listar";

    public ILocator Titulo => page.GetByRole(
        AriaRole.Heading,
        new() { Name = "Listagem de Tarefas" }
    );

    public ILocator CadastrarNovo => page.GetByRole(
        AriaRole.Link,
        new() { Name = "Cadastrar Nova" }
    );

    public ILocator EstadoVazio => page.GetByText(
        "Nenhuma tarefa cadastrada.",
        new() { Exact = true }
    );

    public ILocator MensagemErro => page.Locator(".alert-danger");

    public ILocator NomeDaTarefa(string nome) => page.GetByRole(
        AriaRole.Heading,
        new() { Name = nome, Exact = true }
    );

    public ILocator GrupoPrioridade(string prioridade) => page.GetByRole(
        AriaRole.Heading,
        new() { Name = prioridade, Exact = true }
    );

    public ILocator TextoNaTarefa(string nome, string texto) => CardPorNome(nome).GetByText(
        texto,
        new() { Exact = true }
    );

    public ILocator DataConclusao(string nome) => CardPorNome(nome).Locator("dd").Nth(1);

    public ILocator Status(string nome) => CardPorNome(nome).Locator("dd").Nth(2);

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

    public async Task GerenciarItensAsync(string nome)
    {
        await CardPorNome(nome).GetByRole(
            AriaRole.Link,
            new() { Name = "Itens", Exact = true }
        ).ClickAsync();
    }

    public async Task ConcluirAsync(string nome)
    {
        await CardPorNome(nome).GetByRole(
            AriaRole.Button,
            new() { Name = "Concluir", Exact = true }
        ).ClickAsync();
    }

    public async Task FiltrarTodasAsync()
    {
        await page.GetByRole(
            AriaRole.Link,
            new() { Name = "Todas", Exact = true }
        ).ClickAsync();
    }

    public async Task FiltrarPendentesAsync()
    {
        await page.GetByRole(
            AriaRole.Link,
            new() { Name = "Pendentes", Exact = true }
        ).ClickAsync();
    }

    public async Task FiltrarConcluidasAsync()
    {
        await page.GetByRole(
            AriaRole.Link,
            new() { Name = "Concluídas", Exact = true }
        ).ClickAsync();
    }

    public async Task AgruparPorPrioridadeAsync()
    {
        await page.GetByRole(
            AriaRole.Link,
            new() { Name = "Prioridade", Exact = true }
        ).ClickAsync();
    }

    private ILocator CardPorNome(string nome)
    {
        ILocator nomeTarefa = NomeDaTarefa(nome);

        return page.Locator(".card").Filter(new() { Has = nomeTarefa });
    }
}
