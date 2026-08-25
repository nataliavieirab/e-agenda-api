using System.ComponentModel.DataAnnotations;
using eAgenda.Dominio.Modulos.ModuloCompromisso;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace eAgenda.WebApp.Modulos.ModuloCompromisso;

public record ListarCompromissosViewModel(
    Guid Id,
    string Assunto,
    DateTime DataOcorrencia,
    TimeSpan HoraInicio,
    TimeSpan HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    Guid? ContatoId,
    string? ContatoNome
);

public record CadastrarCompromissoViewModel(
    [Required(ErrorMessage = "O campo \"Assunto\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo \"Assunto\" deve conter entre 2 e 100 caracteres.")]
    string Assunto,

    [Required(ErrorMessage = "O campo \"Data de Ocorrência\" deve ser preenchido.")]
    [DataType(DataType.Date)]
    DateTime DataOcorrencia,

    [Required(ErrorMessage = "O campo \"Hora de Início\" deve ser preenchido.")]
    [DataType(DataType.Time)]
    TimeSpan HoraInicio,

    [Required(ErrorMessage = "O campo \"Hora de Término\" deve ser preenchido.")]
    [DataType(DataType.Time)]
    TimeSpan HoraTermino,

    [Required(ErrorMessage = "O campo \"Tipo de Compromisso\" deve ser preenchido.")]
    TipoCompromisso Tipo,

    [StringLength(255, ErrorMessage = "O campo \"Local\" deve conter no máximo 255 caracteres.")]
    string? Local,

    [StringLength(500, ErrorMessage = "O campo \"Link\" deve conter no máximo 500 caracteres.")]
    string? Link,

    Guid? ContatoId,

    [ValidateNever]
    List<OpcaoContatoViewModel> Contatos
);

public record EditarCompromissoViewModel(
    Guid Id,

    [Required(ErrorMessage = "O campo \"Assunto\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo \"Assunto\" deve conter entre 2 e 100 caracteres.")]
    string Assunto,

    [Required(ErrorMessage = "O campo \"Data de Ocorrência\" deve ser preenchido.")]
    [DataType(DataType.Date)]
    DateTime DataOcorrencia,

    [Required(ErrorMessage = "O campo \"Hora de Início\" deve ser preenchido.")]
    [DataType(DataType.Time)]
    TimeSpan HoraInicio,

    [Required(ErrorMessage = "O campo \"Hora de Término\" deve ser preenchido.")]
    [DataType(DataType.Time)]
    TimeSpan HoraTermino,

    [Required(ErrorMessage = "O campo \"Tipo de Compromisso\" deve ser preenchido.")]
    TipoCompromisso Tipo,

    [StringLength(255, ErrorMessage = "O campo \"Local\" deve conter no máximo 255 caracteres.")]
    string? Local,

    [StringLength(500, ErrorMessage = "O campo \"Link\" deve conter no máximo 500 caracteres.")]
    string? Link,

    Guid? ContatoId,

    [ValidateNever]
    List<OpcaoContatoViewModel> Contatos
);

public record ExcluirCompromissoViewModel(
    Guid Id,
    string Assunto,
    DateTime DataOcorrencia,
    TimeSpan HoraInicio,
    TimeSpan HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    Guid? ContatoId,
    string? ContatoNome
);

public record OpcaoContatoViewModel(Guid Id, string Nome);
