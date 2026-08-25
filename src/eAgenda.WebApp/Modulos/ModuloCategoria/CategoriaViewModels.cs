using System.ComponentModel.DataAnnotations;

namespace eAgenda.WebApp.Modulos.ModuloCategoria;

public record ListarCategoriasViewModel(
    Guid Id,
    string Titulo
);

public record CadastrarCategoriaViewModel(
    [Required(ErrorMessage = "O campo \"Título\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo \"Título\" deve conter entre 2 e 100 caracteres.")]
    string Titulo
);

public record EditarCategoriaViewModel(
    Guid Id,

    [Required(ErrorMessage = "O campo \"Título\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O campo \"Título\" deve conter entre 2 e 100 caracteres.")]
    string Titulo
);

public record ExcluirCategoriaViewModel(
    Guid Id,
    string Titulo
);
