using System.ComponentModel.DataAnnotations;
using eAgenda.Dominio.Modulos.ModuloTarefa;

namespace eAgenda.WebApp.Modulos.ModuloTarefa;

public record ListagemTarefasViewModel(
    string Filtro,
    bool AgruparPorPrioridade,
    List<ListarTarefasViewModel> Tarefas
);

public record ListarTarefasViewModel(
    Guid Id,
    string Titulo,
    PrioridadeTarefa Prioridade,
    DateTime DataCriacao,
    DateTime? DataConclusao,
    bool Concluida,
    int PercentualConcluido,
    List<ItemTarefaViewModel> Itens
);

public record CadastrarTarefaViewModel(
    [Required(ErrorMessage = "O campo \"Título\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo \"Título\" deve conter entre 2 e 100 caracteres.")]
    string Titulo,

    [Required(ErrorMessage = "O campo \"Prioridade\" deve ser preenchido.")]
    PrioridadeTarefa Prioridade
);

public record EditarTarefaViewModel(
    Guid Id,

    [Required(ErrorMessage = "O campo \"Título\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo \"Título\" deve conter entre 2 e 100 caracteres.")]
    string Titulo,

    [Required(ErrorMessage = "O campo \"Prioridade\" deve ser preenchido.")]
    PrioridadeTarefa Prioridade
);

public record ExcluirTarefaViewModel(
    Guid Id,
    string Titulo,
    PrioridadeTarefa Prioridade,
    DateTime DataCriacao,
    DateTime? DataConclusao,
    bool Concluida,
    int PercentualConcluido,
    List<ItemTarefaViewModel> Itens
);

public record GerenciarItensTarefaViewModel(
    Guid Id,
    string Titulo,
    PrioridadeTarefa Prioridade,
    DateTime DataCriacao,
    DateTime? DataConclusao,
    bool Concluida,
    int PercentualConcluido,
    List<ItemTarefaViewModel> Itens,
    string NovoItemTitulo
);

public record AdicionarItemTarefaViewModel(
    Guid TarefaId,

    [Required(ErrorMessage = "O campo \"Título\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo \"Título\" deve conter entre 2 e 100 caracteres.")]
    string Titulo
);

public record ItemTarefaViewModel(
    Guid Id,
    string Titulo,
    bool Concluido
);
