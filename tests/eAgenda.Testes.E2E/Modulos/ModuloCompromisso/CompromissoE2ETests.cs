using System;
using eAgenda.Testes.E2E.Compartilhado;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace eAgenda.Testes.E2E.Modulos.ModuloCompromisso;

[TestClass]
public class CompromissoE2ETests : E2ETestsBase
{
    [TestMethod]
    public async Task DeveCadastrar_CompromissoPresencial_ComDadosValidos()
    {
        CompromissoFormPage formPage = new(Page, UrlBase);
        await formPage.IrParaCadastroAsync();

        await formPage.PreencherAsync(
            assunto: "Reunião Presencial",
            data: "2026-08-15",
            horaInicio: "14:00",
            horaTermino: "15:00",
            tipo: "Presencial",
            local: "Sala 101"
        );

        await formPage.ConfirmarAsync();

        CompromissoListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await Expect(listarPage.NomeDoCompromisso("Reunião Presencial")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Sala 101", new() { Exact = true })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task DeveCadastrar_CompromissoRemoto_ComDadosValidos()
    {
        CompromissoFormPage formPage = new(Page, UrlBase);
        await formPage.IrParaCadastroAsync();

        await formPage.PreencherAsync(
            assunto: "Reunião Mensal",
            data: "2026-08-15",
            horaInicio: "14:00",
            horaTermino: "15:00",
            tipo: "Remoto",
            link: "http://www.meet.google.com"
        );

        await formPage.ConfirmarAsync();

        CompromissoListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await Expect(listarPage.NomeDoCompromisso("Reunião Mensal")).ToBeVisibleAsync();
        await Expect(Page.GetByText("http://www.meet.google.com", new() { Exact = true })).ToBeVisibleAsync();
    }


    [TestMethod]
    public async Task DeveCadastrar_CompromissoVinculadoContato()
    {
        await Page.GotoAsync($"{UrlBase}/Contato/Cadastrar");
        await Page.GetByLabel("Nome").FillAsync("João");
        await Page.GetByLabel("E-mail").FillAsync("joao@email.com");
        await Page.GetByLabel("Telefone").FillAsync("(51) 99999-9999");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirmar" }).ClickAsync();

        CompromissoFormPage formPage = new(Page, UrlBase);
        await formPage.IrParaCadastroAsync();

        await formPage.PreencherAsync(
            assunto: "Reunião Mensal",
            data: "2026-08-15",
            horaInicio: "14:00",
            horaTermino: "15:00",
            tipo: "Remoto",
            link: "http://www.meet.google.com",
            contato: "João"
        );

        await formPage.ConfirmarAsync();

        CompromissoListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await Expect(listarPage.NomeDoCompromisso("Reunião Mensal")).ToBeVisibleAsync();
        await Expect(Page.GetByText("http://www.meet.google.com", new() { Exact = true })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task DeveExibirMensagensObrigatoriedade_SemAssunto()
    {
        CompromissoFormPage formPage = new(Page, UrlBase);
        await formPage.IrParaCadastroAsync();

        await formPage.ConfirmarAsync();
        await Expect(Page.GetByText("O campo \"Assunto\" deve ser preenchido.", new() { Exact = true }))
            .ToBeVisibleAsync();

    }

    [TestMethod]
    public async Task DeveExibirMensagensObrigatoriedade_SemHoraInicio()
    {
        CompromissoFormPage formPage = new(Page, UrlBase);
        await formPage.IrParaCadastroAsync();

        await formPage.PreencherAsync(
            assunto: "Reunião Mensal",
            data: "2026-08-15",
            horaInicio: "00:00",
            horaTermino: "15:00",
            tipo: "Remoto",
            link: "http://www.meet.google.com"
        );

        await formPage.ConfirmarAsync();

        await Expect(Page.GetByText("O campo \"Hora de Início\" deve ser preenchido.", new() { Exact = true }))
            .ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task DeveExibirMensagensObrigatoriedade_SemHoraTermino()
    {
        CompromissoFormPage formPage = new(Page, UrlBase);
        await formPage.IrParaCadastroAsync();

        await formPage.PreencherAsync(
            assunto: "Reunião Mensal",
            data: "2026-08-15",
            horaInicio: "10:00",
            horaTermino: "00:00",
            tipo: "Remoto",
            link: "http://www.meet.google.com"
        );

        await formPage.ConfirmarAsync();

        await Expect(Page.GetByText("O campo \"Hora de Término\" deve ser preenchido.", new() { Exact = true }))
            .ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task DeveExibirMensagem_LocalObrigatorio_CompromissoPresencial()
    {
        CompromissoFormPage formPage = new(Page, UrlBase);
        await formPage.IrParaCadastroAsync();

        await formPage.PreencherAsync(
            assunto: "Reunião Presencial",
            data: "2026-08-15",
            horaInicio: "09:00",
            horaTermino: "10:00",
            tipo: "Presencial"
        );

        await formPage.ConfirmarAsync();

        await Expect(Page.GetByText("O campo \"Local\" deve ser preenchido para compromissos presenciais.", new() { Exact = true }))
            .ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task DeveExibirMensagem_LinkObrigatorio_CompromissoRemoto()
    {
        CompromissoFormPage formPage = new(Page, UrlBase);
        await formPage.IrParaCadastroAsync();

        await formPage.PreencherAsync(
            assunto: "Reunião Remota",
            data: "2026-08-15",
            horaInicio: "14:00",
            horaTermino: "15:00",
            tipo: "Remoto"
        );

        await formPage.ConfirmarAsync();

        await Expect(Page.GetByText("O campo \"Link\" deve ser preenchido para compromissos remotos.", new() { Exact = true }))
            .ToBeVisibleAsync();
    }


    [TestMethod]
    public async Task DeveExibirMensagem_HorariosInvalidos()
    {
        CompromissoFormPage formPage = new(Page, UrlBase);
        await formPage.IrParaCadastroAsync();

        await formPage.PreencherAsync(
            assunto: "Reunião Inválida",
            data: "2026-08-15",
            horaInicio: "15:00",
            horaTermino: "14:00",
            tipo: "Remoto"
        );

        await formPage.ConfirmarAsync();

        await Expect(Page.GetByText("A hora de término deve ser posterior à hora de início.", new() { Exact = true }))
            .ToBeVisibleAsync();
    }


    [TestMethod]
    public async Task DeveExibirMensagem_ConflitoDeHorario()
    {
        CompromissoFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync(
            assunto: "Reunião Mensal",
            data: "2026-08-15",
            horaInicio: "09:00",
            horaTermino: "10:00",
            tipo: "Remoto",
            link: "http://www.meet.google.com"
        );
        await formPage.ConfirmarAsync();

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync(
            assunto: "Reunião Mensal 2",
            data: "2026-08-15",
            horaInicio: "09:30",
            horaTermino: "10:30",
            tipo: "Remoto",
            link: "http://www.meet.google.com"
        );
        await formPage.ConfirmarAsync();

        await Expect(Page.GetByText("Já existe um compromisso cadastrado neste intervalo de horário.", new() { Exact = true }))
            .ToBeVisibleAsync();
    }


    [TestMethod]
    public async Task DeveEditarCompromisso_ComDadosValidos()
    {
        CompromissoFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync(
            assunto: "Reunião Mensal",
            data: "2026-08-15",
            horaInicio: "09:00",
            horaTermino: "10:00",
            tipo: "Remoto",
            link: "http://www.meet.google.com"
        );
        await formPage.ConfirmarAsync();

        CompromissoListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();
        await listarPage.EditarAsync("Reunião Mensal");

        await formPage.Assunto.FillAsync("Reunião Editada");
        await formPage.ConfirmarAsync();

        await Expect(listarPage.NomeDoCompromisso("Reunião Editada")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task DeveListarTodosCompromissos()
    {
        CompromissoFormPage formPage = new(Page, UrlBase);
        CompromissoListarPage listarPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync(
            assunto: "Reunião Mensal",
            data: "2026-08-15",
            horaInicio: "09:00",
            horaTermino: "10:00",
            tipo: "Remoto",
            link: "http://www.meet.google.com"
        );
        await formPage.ConfirmarAsync();

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync(
            assunto: "Reunião Mensal Dezembro",
            data: "2026-08-15",
            horaInicio: "11:00",
            horaTermino: "12:00",
            tipo: "Remoto",
            link: "http://www.meet.google.com"
        );
        await formPage.ConfirmarAsync();

        await listarPage.IrParaAsync();
        await Expect(listarPage.NomeDoCompromisso("Reunião Mensal")).ToBeVisibleAsync();
        await Expect(listarPage.NomeDoCompromisso("Reunião Mensal Dezembro")).ToBeVisibleAsync();
    }


    [TestMethod]
    public async Task DeveExcluirCompromisso()
    {
        CompromissoFormPage formPage = new(Page, UrlBase);
        CompromissoListarPage listarPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync(
            assunto: "Reunião para Exclusão",
            data: "2026-08-15",
            horaInicio: "09:00",
            horaTermino: "10:00",
            tipo: "Remoto",
            link: "http://www.meet.google.com"
        );
        await formPage.ConfirmarAsync();

        await listarPage.IrParaAsync();
        await listarPage.ExcluirAsync("Reunião para Exclusão");

        CompromissoExcluirPage excluirPage = new(Page);
        await Expect(excluirPage.MensagemConfirmacao).ToBeVisibleAsync();
        await excluirPage.ConfirmarAsync();

        await listarPage.IrParaAsync();
        await Expect(listarPage.NomeDoCompromisso("Reunião para Exclusão"))
            .Not.ToBeVisibleAsync();
    }
}
