using System.Text.RegularExpressions;
using eAgenda.Testes.E2E.Compartilhado;
using Microsoft.Playwright;

namespace eAgenda.Testes.E2E.Modulos.ModuloTarefa;

[TestClass]
public sealed class TarefaE2ETests : E2ETestsBase
{
    [TestMethod]
    public async Task Cadastrar_ComDadosValido_RegistraTarefa()
    {
        TarefaFormPage formPage = new(Page, UrlBase);
        TarefaListarPage listarPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();

        await Page.GetByLabel("Título").FillAsync("Faxina");
        await Page.GetByLabel("Prioridade").SelectOptionAsync(new SelectOptionValue { Label = "Alta" });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirmar", Exact = true })
            .ClickAsync();

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Faxina", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByText("Alta", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Nenhuma tarefa cadastrada.", new() { Exact = true }))
            .Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Cadastrar_ComItensVinculados_RegistraTarefa()
    {
        TarefaFormPage formPage = new(Page, UrlBase);
        TarefaListarPage listarPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();

        await Page.GetByLabel("Título").FillAsync("Faxina");
        await Page.GetByLabel("Prioridade").SelectOptionAsync(new SelectOptionValue { Label = "Alta" });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirmar", Exact = true })
            .ClickAsync();

        await listarPage.GerenciarItensAsync("Faxina");

        await Page.GetByLabel("Novo Item").FillAsync("Limpar o banheiro");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar", Exact = true })
            .ClickAsync();

        await Page.GetByLabel("Novo Item").FillAsync("Arrumar a cama");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar", Exact = true })
            .ClickAsync();

        await Expect(Page.GetByText("Limpar o banheiro", new() { Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByText("Arrumar a cama", new() { Exact = true }))
            .ToBeVisibleAsync();

        await listarPage.IrParaAsync();

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Faxina", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(listarPage.TextoNaTarefa("Faxina", "2")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Nenhuma tarefa cadastrada.", new() { Exact = true }))
            .Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Cadastrar_SemTitulo_ImpedeCadastro_RetornaMensagemDeErro()
    {
        TarefaFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirmar", Exact = true })
            .ClickAsync();

        await Expect(Page.GetByText("O campo \"Título\" deve ser preenchido.", new() { Exact = true }))
            .ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Editar_ComDadosValidos_AtualizaTarefa()
    {
        await CadastrarTarefaAsync("Faxina", "Alta");

        TarefaListarPage listarPage = new(Page, UrlBase);

        await listarPage.EditarAsync("Faxina");

        await Page.GetByLabel("Título").FillAsync("Limpeza");
        await Page.GetByLabel("Prioridade").SelectOptionAsync(new SelectOptionValue { Label = "Baixa" });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirmar", Exact = true })
            .ClickAsync();

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Limpeza", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByText("Baixa", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Faxina", Exact = true }))
            .Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Editar_SemTitulo_ImpedeAtualizacao_RetornaMensagemDeErro()
    {
        await CadastrarTarefaAsync("Faxina", "Alta");

        TarefaListarPage listarPage = new(Page, UrlBase);

        await listarPage.EditarAsync("Faxina");

        await Page.GetByLabel("Título").FillAsync(string.Empty);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirmar", Exact = true })
            .ClickAsync();

        await Expect(Page.GetByText("O campo \"Título\" deve ser preenchido.", new() { Exact = true }))
            .ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Concluir_RegistraDataConclusaoEStatus()
    {
        await CadastrarTarefaAsync("Estudar", "Alta");

        TarefaListarPage listarPage = new(Page, UrlBase);

        await listarPage.ConcluirAsync("Estudar");

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(listarPage.Status("Estudar")).ToHaveTextAsync("Concluída");
        await Expect(listarPage.DataConclusao("Estudar"))
            .ToHaveTextAsync(DateTime.Today.ToString("dd/MM/yyyy"));
        await Expect(Page.GetByText("100%", new() { Exact = true })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Excluir_SemItensVinculados_DeletaTarefa()
    {
        await CadastrarTarefaAsync("Faxina", "Alta");

        TarefaListarPage listarPage = new(Page, UrlBase);

        await listarPage.ExcluirAsync("Faxina");

        await Expect(Page).ToHaveURLAsync(
            new Regex($"{Regex.Escape(UrlBase)}/Tarefa/Excluir/.*")
        );

        await Expect(Page.GetByText("Deseja realmente excluir esta tarefa?", new() { Exact = true }))
            .ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirmar", Exact = true })
            .ClickAsync();

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Faxina", Exact = true }))
            .Not.ToBeVisibleAsync();
        await Expect(Page.GetByText("Nenhuma tarefa cadastrada.", new() { Exact = true }))
            .ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Excluir_ComItensVinculados_DeletaTarefaEItens()
    {
        await CadastrarTarefaComItensAsync("Limpar casa", "Baixa", "Varrer quarto");

        TarefaListarPage listarPage = new(Page, UrlBase);

        await listarPage.IrParaAsync();
        await listarPage.ExcluirAsync("Limpar casa");

        await Expect(Page).ToHaveURLAsync(
            new Regex($"{Regex.Escape(UrlBase)}/Tarefa/Excluir/.*")
        );

        await Expect(Page.GetByText("Deseja realmente excluir esta tarefa?", new() { Exact = true }))
            .ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirmar", Exact = true })
            .ClickAsync();

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Limpar casa", Exact = true }))
            .Not.ToBeVisibleAsync();
        await Expect(Page.GetByText("Nenhuma tarefa cadastrada.", new() { Exact = true }))
            .ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Listar_ExibeTodasAsTarefas()
    {
        await CadastrarTarefaAsync("Estudar", "Alta");
        await CadastrarTarefaAsync("Comprar pão", "Baixa");

        TarefaListarPage listarPage = new(Page, UrlBase);

        await listarPage.IrParaAsync();

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Estudar", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Comprar pão", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByText("Nenhuma tarefa cadastrada.", new() { Exact = true }))
            .Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Listar_AgrupadasPorPrioridade_ExibeTarefasAgrupadasPorPrioridade()
    {
        await CadastrarTarefaAsync("Organizar armário", "Baixa");
        await CadastrarTarefaAsync("Estudar", "Normal");
        await CadastrarTarefaAsync("Responder e-mails", "Alta");

        TarefaListarPage listarPage = new(Page, UrlBase);

        await listarPage.IrParaAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Prioridade", Exact = true })
            .ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Alta", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Normal", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Baixa", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Responder e-mails", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Estudar", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Organizar armário", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByText("Nenhuma tarefa cadastrada.", new() { Exact = true }))
            .Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Listar_AgrupadasPorConclusao_ExibeTarefasAgrupadasPorConclusao()
    {
        await CadastrarTarefaAsync("Estudar", "Alta");
        await CadastrarTarefaAsync("Comprar pão", "Baixa");

        TarefaListarPage listarPage = new(Page, UrlBase);

        await listarPage.ConcluirAsync("Comprar pão");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Pendentes", Exact = true })
            .ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Estudar", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Comprar pão", Exact = true }))
            .Not.ToBeVisibleAsync();
        await Expect(listarPage.Status("Estudar")).ToHaveTextAsync("Pendente");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Concluídas", Exact = true })
            .ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Comprar pão", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Estudar", Exact = true }))
            .Not.ToBeVisibleAsync();
        await Expect(listarPage.Status("Comprar pão")).ToHaveTextAsync("Concluída");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Todas", Exact = true })
            .ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Estudar", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Comprar pão", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.GetByText("Nenhuma tarefa cadastrada.", new() { Exact = true }))
            .Not.ToBeVisibleAsync();
    }

    private async Task CadastrarTarefaAsync(string titulo, string prioridade)
    {
        TarefaFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();

        await Page.GetByLabel("Título").FillAsync(titulo);
        await Page.GetByLabel("Prioridade").SelectOptionAsync(new SelectOptionValue { Label = prioridade });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirmar", Exact = true })
            .ClickAsync();

        await Expect(Page).ToHaveURLAsync($"{UrlBase}/Tarefa/Listar");
    }

    private async Task CadastrarTarefaComItensAsync(string titulo, string prioridade, params string[] itens)
    {
        await CadastrarTarefaAsync(titulo, prioridade);

        TarefaListarPage listarPage = new(Page, UrlBase);

        await listarPage.GerenciarItensAsync(titulo);

        foreach (string item in itens)
        {
            await Page.GetByLabel("Novo Item").FillAsync(item);
            await Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar", Exact = true })
                .ClickAsync();
        }

        await listarPage.IrParaAsync();
        await Expect(Page).ToHaveURLAsync(listarPage.Url);
    }
}
