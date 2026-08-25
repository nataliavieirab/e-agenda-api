using System.ComponentModel.DataAnnotations;
using eAgenda.Dominio.Modulos.ModuloDespesa;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace eAgenda.WebApp.Modulos.ModuloDespesa;

public record ListarDespesasViewModel(
    Guid Id,
    string Descricao,
    DateTime DataOcorrencia,
    decimal Valor,
    FormaPagamento FormaPagamento,
    List<CategoriaDespesaViewModel> Categorias
);

public record CadastrarDespesaViewModel(
    [Required(ErrorMessage = "O campo \"Descrição\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo \"Descrição\" deve conter entre 2 e 100 caracteres.")]
    string Descricao,

    [DataType(DataType.Date)]
    DateTime? DataOcorrencia,

    [Range(0.01, double.MaxValue, ErrorMessage = "O campo \"Valor\" deve ser maior que zero.")]
    decimal Valor,

    [Required(ErrorMessage = "O campo \"Forma de Pagamento\" deve ser preenchido.")]
    FormaPagamento FormaPagamento,

    List<Guid> CategoriaIds,

    [ValidateNever]
    List<CategoriaDespesaViewModel> Categorias
);

public record EditarDespesaViewModel(
    Guid Id,

    [Required(ErrorMessage = "O campo \"Descrição\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo \"Descrição\" deve conter entre 2 e 100 caracteres.")]
    string Descricao,

    [DataType(DataType.Date)]
    DateTime? DataOcorrencia,

    [Range(0.01, double.MaxValue, ErrorMessage = "O campo \"Valor\" deve ser maior que zero.")]
    decimal Valor,

    [Required(ErrorMessage = "O campo \"Forma de Pagamento\" deve ser preenchido.")]
    FormaPagamento FormaPagamento,

    List<Guid> CategoriaIds,

    [ValidateNever]
    List<CategoriaDespesaViewModel> Categorias
);

public record ExcluirDespesaViewModel(
    Guid Id,
    string Descricao,
    DateTime DataOcorrencia,
    decimal Valor,
    FormaPagamento FormaPagamento,
    List<CategoriaDespesaViewModel> Categorias
);

public record CategoriaDespesaViewModel(Guid Id, string Titulo);
