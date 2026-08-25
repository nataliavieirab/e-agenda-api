using AutoMapper;
using FluentResults;
using eAgenda.WebApp.Compartilhado.Apresentacao.Extensions;
using eAgenda.Aplicacao.Modulos.ModuloCompromisso;
using eAgenda.Dominio.Modulos.ModuloCompromisso;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Modulos.ModuloCompromisso;

public class CompromissoController(ServicoCompromisso servicoCompromisso, IMapper mapeador) : Controller
{
    [HttpGet]
    public ActionResult Listar()
    {
        List<ListarCompromissosDto> dtos = servicoCompromisso.SelecionarTodos();

        List<ListarCompromissosViewModel> listarVms = mapeador.Map<List<ListarCompromissosViewModel>>(dtos);

        return View(listarVms);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarCompromissoViewModel cadastrarVm = new CadastrarCompromissoViewModel(
            string.Empty,
            DateTime.Today,
            TimeSpan.Zero,
            TimeSpan.Zero,
            TipoCompromisso.Presencial,
            null,
            null,
            null,
            SelecionarContatos()
        );

        return View(cadastrarVm);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarCompromissoViewModel cadastrarVm)
    {
        if (!ModelState.IsValid)
            return View(cadastrarVm with { Contatos = SelecionarContatos() });

        CadastrarCompromissoDto dto = mapeador.Map<CadastrarCompromissoDto>(cadastrarVm);

        Result resultado = servicoCompromisso.Cadastrar(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);

            return View(cadastrarVm with { Contatos = SelecionarContatos() });
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Editar(Guid id)
    {
        Result<DetalhesCompromissoDto> resultado = servicoCompromisso.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        EditarCompromissoViewModel editarVm =
            mapeador.Map<EditarCompromissoViewModel>(resultado.Value) with { Contatos = SelecionarContatos() };

        return View(editarVm);
    }

    [HttpPost]
    public ActionResult Editar(EditarCompromissoViewModel editarVm)
    {
        if (!ModelState.IsValid)
            return View(editarVm with { Contatos = SelecionarContatos() });

        EditarCompromissoDto dto = mapeador.Map<EditarCompromissoDto>(editarVm);

        Result resultado = servicoCompromisso.Editar(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);

            return View(editarVm with { Contatos = SelecionarContatos() });
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Excluir(Guid id)
    {
        Result<DetalhesCompromissoDto> resultado = servicoCompromisso.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        ExcluirCompromissoViewModel excluirVm = mapeador.Map<ExcluirCompromissoViewModel>(resultado.Value);

        return View(excluirVm);
    }

    [HttpPost]
    public ActionResult Excluir(ExcluirCompromissoViewModel excluirVm)
    {
        Result resultado = servicoCompromisso.Excluir(excluirVm.Id);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        return RedirectToAction(nameof(Listar));
    }

    private List<OpcaoContatoViewModel> SelecionarContatos()
    {
        List<OpcaoContatoDto> dtos = servicoCompromisso.SelecionarContatos();

        return mapeador.Map<List<OpcaoContatoViewModel>>(dtos);
    }
}
