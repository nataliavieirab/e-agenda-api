using AutoMapper;
using FluentResults;
using eAgenda.WebApp.Compartilhado.Apresentacao.Extensions;
using eAgenda.Aplicacao.Modulos.ModuloContato;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Modulos.ModuloContato;

public class ContatoController(ServicoContato servicoContato, IMapper mapeador) : Controller
{
    [HttpGet]
    public ActionResult Listar()
    {
        List<ListarContatosDto> dtos = servicoContato.SelecionarTodos();

        List<ListarContatosViewModel> listarVms = mapeador.Map<List<ListarContatosViewModel>>(dtos);

        return View(listarVms);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarContatoViewModel cadastrarVm = new CadastrarContatoViewModel(
            string.Empty,
            string.Empty,
            string.Empty,
            null,
            null
        );

        return View(cadastrarVm);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarContatoViewModel cadastrarVm)
    {
        if (!ModelState.IsValid)
            return View(cadastrarVm);

        CadastrarContatoDto dto = mapeador.Map<CadastrarContatoDto>(cadastrarVm);

        Result resultado = servicoContato.Cadastrar(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);

            return View(cadastrarVm);
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Editar(Guid id)
    {
        Result<DetalhesContatoDto> resultado = servicoContato.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        EditarContatoViewModel editarVm = mapeador.Map<EditarContatoViewModel>(resultado.Value);

        return View(editarVm);
    }

    [HttpPost]
    public ActionResult Editar(EditarContatoViewModel editarVm)
    {
        if (!ModelState.IsValid)
            return View(editarVm);

        EditarContatoDto dto = mapeador.Map<EditarContatoDto>(editarVm);

        Result resultado = servicoContato.Editar(dto);

        if (resultado.IsFailed)
        {
            ModelState.AddModelError(resultado);

            return View(editarVm);
        }

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Excluir(Guid id)
    {
        Result<DetalhesContatoDto> resultado = servicoContato.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        ExcluirContatoViewModel excluirVm = mapeador.Map<ExcluirContatoViewModel>(resultado.Value);

        return View(excluirVm);
    }

    [HttpPost]
    public ActionResult Excluir(ExcluirContatoViewModel excluirVm)
    {
        Result resultado = servicoContato.Excluir(excluirVm.Id);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        return RedirectToAction(nameof(Listar));
    }
}
