using AutoMapper;
using FluentResults;
using eAgenda.WebApp.Compartilhado.Apresentacao.Extensions;
using eAgenda.Aplicacao.Modulos.ModuloTarefa;
using eAgenda.Dominio.Modulos.ModuloTarefa;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Modulos.ModuloTarefa;

public class TarefaController(ServicoTarefa servicoTarefa, IMapper mapeador) : Controller
{
    [HttpGet]
    public ActionResult Listar(string filtro = "Todas", bool agruparPorPrioridade = false)
    {
        List<ListarTarefasDto> dtos = servicoTarefa.SelecionarTodos(filtro);
        List<ListarTarefasViewModel> listarVms = mapeador.Map<List<ListarTarefasViewModel>>(dtos);

        ListagemTarefasViewModel listagemVm = new ListagemTarefasViewModel(
            filtro,
            agruparPorPrioridade,
            listarVms
        );

        return View(listagemVm);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarTarefaViewModel cadastrarVm = new CadastrarTarefaViewModel(
            string.Empty,
            PrioridadeTarefa.Normal
        );

        return View(cadastrarVm);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarTarefaViewModel cadastrarVm)
    {
        if (!ModelState.IsValid)
            return View(cadastrarVm);

        CadastrarTarefaDto dto = mapeador.Map<CadastrarTarefaDto>(cadastrarVm);
        Result resultado = servicoTarefa.Cadastrar(dto);

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
        Result<DetalhesTarefaDto> resultado = servicoTarefa.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        EditarTarefaViewModel editarVm = mapeador.Map<EditarTarefaViewModel>(resultado.Value);

        return View(editarVm);
    }

    [HttpPost]
    public ActionResult Editar(EditarTarefaViewModel editarVm)
    {
        if (!ModelState.IsValid)
            return View(editarVm);

        EditarTarefaDto dto = mapeador.Map<EditarTarefaDto>(editarVm);
        Result resultado = servicoTarefa.Editar(dto);

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
        Result<DetalhesTarefaDto> resultado = servicoTarefa.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        ExcluirTarefaViewModel excluirVm = mapeador.Map<ExcluirTarefaViewModel>(resultado.Value);

        return View(excluirVm);
    }

    [HttpPost]
    public ActionResult Excluir(ExcluirTarefaViewModel excluirVm)
    {
        Result resultado = servicoTarefa.Excluir(excluirVm.Id);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult GerenciarItens(Guid id)
    {
        Result<DetalhesTarefaDto> resultado = servicoTarefa.SelecionarPorId(id);

        if (resultado.IsFailed)
        {
            TempData.AddErrorMessage(resultado);

            return RedirectToAction(nameof(Listar));
        }

        GerenciarItensTarefaViewModel gerenciarVm =
            mapeador.Map<GerenciarItensTarefaViewModel>(resultado.Value);

        return View(gerenciarVm);
    }

    [HttpPost]
    public ActionResult AdicionarItem(AdicionarItemTarefaViewModel adicionarVm)
    {
        if (!ModelState.IsValid)
        {
            TempData["MensagemErro"] = "O campo \"Título\" deve conter entre 2 e 100 caracteres.";

            return RedirectToAction(nameof(GerenciarItens), new { id = adicionarVm.TarefaId });
        }

        AdicionarItemTarefaDto dto = mapeador.Map<AdicionarItemTarefaDto>(adicionarVm);
        Result resultado = servicoTarefa.AdicionarItem(dto);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        return RedirectToAction(nameof(GerenciarItens), new { id = adicionarVm.TarefaId });
    }

    [HttpPost]
    public ActionResult AlterarConclusaoItem(Guid tarefaId, Guid itemId, bool concluido)
    {
        AlterarConclusaoItemTarefaDto dto = new AlterarConclusaoItemTarefaDto(
            tarefaId,
            itemId,
            concluido
        );

        Result resultado = servicoTarefa.AlterarConclusaoItem(dto);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        return RedirectToAction(nameof(GerenciarItens), new { id = tarefaId });
    }

    [HttpPost]
    public ActionResult AlterarConclusao(Guid tarefaId, bool concluida, bool retornarParaItens = false)
    {
        AlterarConclusaoTarefaDto dto = new AlterarConclusaoTarefaDto(tarefaId, concluida);
        Result resultado = servicoTarefa.AlterarConclusao(dto);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        if (retornarParaItens)
            return RedirectToAction(nameof(GerenciarItens), new { id = tarefaId });

        return RedirectToAction(nameof(Listar));
    }

    [HttpPost]
    public ActionResult RemoverItem(Guid tarefaId, Guid itemId)
    {
        RemoverItemTarefaDto dto = new RemoverItemTarefaDto(tarefaId, itemId);
        Result resultado = servicoTarefa.RemoverItem(dto);

        if (resultado.IsFailed)
            TempData.AddErrorMessage(resultado);

        return RedirectToAction(nameof(GerenciarItens), new { id = tarefaId });
    }
}
