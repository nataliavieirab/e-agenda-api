using System.Text.RegularExpressions;
using eAgenda.Testes.E2E.Compartilhado;
using Microsoft.Playwright;
namespace eAgenda.Testes.E2E.Modulos.ModuloContato;

[TestClass]
public sealed class ContatoE2ETests : E2ETestsBase
{
    [TestMethod]
    public async Task Cadastrar_ComDadosValido_RegistraContato()
    {
        await Page.GotoAsync($"{UrlBase}/Contato/Listar");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Cadastrar Novo" })
            .ClickAsync();

        await Page.GetByLabel("Nome").FillAsync("Teste");
        await Page.GetByLabel("E-mail").FillAsync("teste@email.com");
        await Page.GetByLabel("Telefone").FillAsync("(00) 00000-0000");
        await Page.GetByLabel("Cargo").FillAsync("Cargo X");
        await Page.GetByLabel("Empresa").FillAsync("Empresa X");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirmar" })
            .ClickAsync();

        Assert.AreEqual(
            "/Contato/Listar",
            new Uri(Page.Url).AbsolutePath
        );

        await Expect(Page.GetByText("Teste", new() { Exact = true }))
            .ToBeVisibleAsync();

        await Expect(Page.GetByText("Nenhum contato cadastrado.", new() { Exact = true }))
            .Not.ToBeVisibleAsync();

    }

    [TestMethod]
    public async Task Cadastrar_ComEmailDuplicado_ImpedeCadastro_RetornaMensagemDeErro()
    {
        await Page.GotoAsync($"{UrlBase}/Contato/Listar");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Cadastrar Novo" })
            .ClickAsync();

        await Page.GetByLabel("Nome").FillAsync("Contato1");
        await Page.GetByLabel("E-mail").FillAsync("email.contato1@email.com");
        await Page.GetByLabel("Telefone").FillAsync("(00) 00000-0000");
        await Page.GetByLabel("Cargo").FillAsync("Cargo X");
        await Page.GetByLabel("Empresa").FillAsync("Empresa X");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirmar" })
            .ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Cadastrar Novo" })
          .ClickAsync();

        await Page.GetByLabel("Nome").FillAsync("Contato2");
        await Page.GetByLabel("E-mail").FillAsync("email.contato1@email.com");
        await Page.GetByLabel("Telefone").FillAsync("(11) 11111-1111");
        await Page.GetByLabel("Cargo").FillAsync("Cargo Y");
        await Page.GetByLabel("Empresa").FillAsync("Empresa Y");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirmar" })
            .ClickAsync();


        Assert.AreEqual(
            "/Contato/Cadastrar",
            new Uri(Page.Url).AbsolutePath
        );

        await Expect(Page.GetByText("Já existe um contato com este email.", new() { Exact = true }))
            .ToBeVisibleAsync();

        await Expect(Page.GetByText("Nenhum contato cadastrado.", new() { Exact = true }))
            .Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Cadastrar_ComTelefoneDuplicado_ImpedeCadastro_RetornaMensagemDeErro()
    {
        await Page.GotoAsync($"{UrlBase}/Contato/Listar");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Cadastrar Novo" })
            .ClickAsync();

        await Page.GetByLabel("Nome").FillAsync("Contato1");
        await Page.GetByLabel("E-mail").FillAsync("email.contato1@email.com");
        await Page.GetByLabel("Telefone").FillAsync("(00) 00000-0000");
        await Page.GetByLabel("Cargo").FillAsync("Cargo X");
        await Page.GetByLabel("Empresa").FillAsync("Empresa X");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirmar" })
            .ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Cadastrar Novo" })
          .ClickAsync();

        await Page.GetByLabel("Nome").FillAsync("Contato2");
        await Page.GetByLabel("E-mail").FillAsync("email.contato2@email.com");
        await Page.GetByLabel("Telefone").FillAsync("(00) 00000-0000");
        await Page.GetByLabel("Cargo").FillAsync("Cargo Y");
        await Page.GetByLabel("Empresa").FillAsync("Empresa Y");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirmar" })
            .ClickAsync();


        Assert.AreEqual(
            "/Contato/Cadastrar",
            new Uri(Page.Url).AbsolutePath
        );

        await Expect(Page.GetByText("Já existe um contato com este telefone.", new() { Exact = true }))
            .ToBeVisibleAsync();

        await Expect(Page.GetByText("Nenhum contato cadastrado.", new() { Exact = true }))
            .Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Editar_ComDadosValidos_AtualizaContato()
    {
        await CadastarContatoAsync("Contato", "contato@email.com", "(00) 00000-0000", "Cargo X", "Empresa Y");

        ContatoFormPage formPage = new(Page, UrlBase);
        ContatoListarPage listarPage = new(Page, UrlBase);

        await listarPage.EditarAsync("Contato");

        await formPage.PreencherAsync("ContatoEditado", "contato@email.com", "(00) 00000-0000", "Cargo X", "Empresa Y");
        await formPage.ConfirmarAsync();

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(listarPage.NomeDoContato("ContatoEditado")).ToBeVisibleAsync();
        await Expect(listarPage.NomeDoContato("Contato")).Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Listar_ExibeTodosOsContatos()
    {
        await CadastarContatoAsync(
            "Contato1",
            "contato1@email.com",
            "(00) 00000-0000",
            "Cargo X",
            "Empresa Y"
        );

        await CadastarContatoAsync(
            "Contato2",
            "contato2@email.com",
            "(11) 11111-1111",
            "Cargo Y",
            "Empresa X"
        );

        ContatoListarPage listarPage = new(Page, UrlBase);

        await listarPage.IrParaAsync();

        await Expect(Page).ToHaveURLAsync(listarPage.Url);

        await Expect(listarPage.NomeDoContato("Contato1"))
            .ToBeVisibleAsync();

        await Expect(listarPage.NomeDoContato("Contato2"))
            .ToBeVisibleAsync();

        await Expect(listarPage.EstadoVazio)
            .Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Excluir_SemCompromissoVinculado_DeletaContato()
    {
        await CadastarContatoAsync("Contato", "contato@email.com", "(00) 00000-0000", "Cargo X", "Empresa Y");

        ContatoListarPage listarPage = new(Page, UrlBase);
        ContatoExcluirPage excluirPage = new(Page);

        await listarPage.ExcluirAsync("Contato");

        await Expect(Page).ToHaveURLAsync(
            new Regex($"{Regex.Escape(UrlBase)}/Contato/Excluir/.*")
        );

        await Expect(excluirPage.MensagemConfirmacao).ToBeVisibleAsync();

        await excluirPage.ConfirmarAsync();

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(listarPage.NomeDoContato("Contato")).Not.ToBeVisibleAsync();
        await Expect(listarPage.EstadoVazio).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Excluir_ComCompromissoVinculado_ImpedeDelecao_RetornaMensagemDeErro()
    {
        await CadastarContatoAsync("Contato", "contato@email.com", "(00) 00000-0000", "Cargo X", "Empresa Y");

        await CadastarCompromissoAsync("Contato");

        ContatoListarPage listarPage = new(Page, UrlBase);
        ContatoExcluirPage excluirPage = new(Page);

        await listarPage.IrParaAsync();
        await listarPage.ExcluirAsync("Contato");

        await Expect(Page).ToHaveURLAsync(
            new Regex($"{Regex.Escape(UrlBase)}/Contato/Excluir/.*")
        );

        await Expect(excluirPage.MensagemConfirmacao).ToBeVisibleAsync();

        await excluirPage.ConfirmarAsync();

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
        await Expect(listarPage.MensagemErro).ToContainTextAsync(
            "Não é possível excluir este contato, pois ele possui compromissos vinculados."
        );
        await Expect(listarPage.NomeDoContato("Contato")).ToBeVisibleAsync();
        await Expect(listarPage.EstadoVazio).Not.ToBeVisibleAsync();
    }

    private async Task CadastarContatoAsync(string nome, string email, string telefone, string cargo, string empresa)
    {
        ContatoFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();

        await formPage.PreencherAsync(nome, email, telefone, cargo, empresa);

        await formPage.ConfirmarAsync();

        ContatoListarPage listarPage = new(Page, UrlBase);

        await Expect(Page).ToHaveURLAsync(listarPage.Url);
    }

    private async Task CadastarCompromissoAsync(string nomeContato)
    {
        await Page.GotoAsync($"{UrlBase}/Compromisso/Cadastrar");

        await Page.GetByLabel("Assunto").FillAsync("Reunião");
        await Page.GetByLabel("Data de Ocorrência").FillAsync(DateTime.Today.ToString("yyyy-MM-dd"));
        await Page.GetByLabel("Hora de Início").FillAsync("10:00");
        await Page.GetByLabel("Hora de Término").FillAsync("11:00");
        await Page.GetByLabel("Tipo de Compromisso").SelectOptionAsync("Presencial");
        await Page.GetByLabel("Contato").SelectOptionAsync(new SelectOptionValue { Label = nomeContato });
        await Page.GetByLabel("Local").FillAsync("Sala 1");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirmar", Exact = true })
            .ClickAsync();

        await Expect(Page).ToHaveURLAsync($"{UrlBase}/Compromisso/Listar");
    }
}
