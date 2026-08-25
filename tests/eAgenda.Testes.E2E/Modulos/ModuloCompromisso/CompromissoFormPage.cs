using Microsoft.Playwright;

namespace eAgenda.Testes.E2E.Modulos.ModuloCompromisso;

public sealed class CompromissoFormPage(
    IPage page,
    string urlBase
)
{
    public string UrlCadastrar => $"{urlBase}/Compromisso/Cadastrar";
    public string UrlEditar => $"{urlBase}/Compromisso/Editar";

    // Campos do formulário
    public ILocator Assunto => page.GetByLabel("Assunto");
    public ILocator Link => page.GetByLabel("Link");
    public ILocator DataOcorrencia => page.GetByLabel("Data de Ocorrência");
    public ILocator HoraInicio => page.GetByLabel("Hora de Início");
    public ILocator HoraTermino => page.GetByLabel("Hora de Término");
    public ILocator TipoCompromisso => page.GetByLabel("Tipo de Compromisso");
    public ILocator Local => page.GetByLabel("Local");
    public ILocator Contato => page.GetByLabel("Contato");

    // Navegação
    public async Task IrParaCadastroAsync()
    {
        await page.GotoAsync(UrlCadastrar);
    }

    public async Task IrParaEdicaoAsync()
    {
        await page.GotoAsync(UrlEditar);
    }

    // Preenchimento
    public async Task PreencherAsync(
        string assunto,
        string data,
        string horaInicio,
        string horaTermino,
        string tipo,
        string? link = null,
        string? local = null,
        string? contato = null
    )
    {
        await Assunto.FillAsync(assunto);
        await DataOcorrencia.FillAsync(data);
        await HoraInicio.FillAsync(horaInicio);
        await HoraTermino.FillAsync(horaTermino);
        await TipoCompromisso.SelectOptionAsync(new SelectOptionValue { Label = tipo });

        if (!string.IsNullOrEmpty(link))
            await Link.FillAsync(link);

        if (!string.IsNullOrEmpty(local))
            await Local.FillAsync(local);

        if (!string.IsNullOrEmpty(contato))
            await Contato.SelectOptionAsync(new SelectOptionValue { Label = contato });
    }

    // Confirmação
    public async Task ConfirmarAsync()
    {
        await page.GetByRole(
            AriaRole.Button,
            new() { Name = "Confirmar", Exact = true }
        ).ClickAsync();
    }
}
