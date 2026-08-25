using AutoMapper;
using FluentResults;
using eAgenda.WebApp.Compartilhado.Apresentacao.Extensions;
using eAgenda.Aplicacao.Modulos.ModuloDespesa;
using eAgenda.Dominio.Modulos.ModuloDespesa;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Modulos.ModuloDespesa;

public class DespesaController(ServicoDespesa servicoDespesa, IMapper mapeador) : Controller
{
    [HttpGet]
    public ActionResult Listar()
    {
        List<ListarDespesasDto> dtos = servicoDespesa.SelecionarTodos();

        List<ListarDespesasViewModel> listarVms = mapeador.Map<List<ListarDespesasViewModel>>(dtos);

        return View(listarVms);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarDespesaViewModel cadastrarVm = new CadastrarDespesaViewModel(
            string.Empty,
            DateTime.Today,
            0,
            FormaPagamento.AVista,
            [],
            SelecionarCategorias()
        );

        return View(cadastrarVm);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarDespesaViewModel cadastrarVm)
    {
        if (!ModelState.IsValid)
            return View(cadastrarVm with { Categorias = SelecionarCategorias() });

        CadastrarDespesaDto dto = mapeador.Map<CadastrarDespesaDto>(cadastrarVm);

        Result resultado = servicoDespesa.Cadastrar(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);

            return View(cadastrarVm with { Categorias = SelecionarCategorias() });
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Editar(Guid id)
    {
        Result<DetalhesDespesaDto> resultado = servicoDespesa.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        EditarDespesaViewModel editarVm =
            mapeador.Map<EditarDespesaViewModel>(resultado.Value) with { Categorias = SelecionarCategorias() };

        return View(editarVm);
    }

    [HttpPost]
    public ActionResult Editar(EditarDespesaViewModel editarVm)
    {
        if (!ModelState.IsValid)
            return View(editarVm with { Categorias = SelecionarCategorias() });

        EditarDespesaDto dto = mapeador.Map<EditarDespesaDto>(editarVm);

        Result resultado = servicoDespesa.Editar(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);

            return View(editarVm with { Categorias = SelecionarCategorias() });
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Excluir(Guid id)
    {
        Result<DetalhesDespesaDto> resultado = servicoDespesa.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        ExcluirDespesaViewModel excluirVm = mapeador.Map<ExcluirDespesaViewModel>(resultado.Value);

        return View(excluirVm);
    }

    [HttpPost]
    public ActionResult Excluir(ExcluirDespesaViewModel excluirVm)
    {
        Result resultado = servicoDespesa.Excluir(excluirVm.Id);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        return RedirectToAction(nameof(Listar));
    }

    private List<CategoriaDespesaViewModel> SelecionarCategorias()
    {
        List<CategoriaDespesaDto> dtos = servicoDespesa.SelecionarCategorias();

        return mapeador.Map<List<CategoriaDespesaViewModel>>(dtos);
    }
}
