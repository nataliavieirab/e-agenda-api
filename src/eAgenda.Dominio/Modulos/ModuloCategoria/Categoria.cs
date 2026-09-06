using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.Modulos.ModuloDespesa;

namespace eAgenda.Dominio.Modulos.ModuloCategoria;

public class Categoria : EntidadeBase<Categoria>
{
    public string Titulo { get; set; }
    public List<Despesa> Despesas { get; set; } = new List<Despesa>();

    public Categoria()
    {
    }

    public Categoria(string titulo) : this()
    {
        Titulo = titulo;
    }

    public override IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (string.IsNullOrWhiteSpace(Titulo))
            erros.Add(new(nameof(Titulo), "O campo \"Título\" deve ser preenchido."));

        else if (Titulo.Length < 2)
            erros.Add(new(nameof(Titulo), "O campo \"Título\" deve conter no mínimo 2 caracteres."));

        else if (Titulo.Length > 100)
            erros.Add(new(nameof(Titulo), "O campo \"Título\" deve conter no máximo 100 caracteres."));

        return erros;
    }

    public override void Atualizar(Categoria entidadeAtualizada)
    {
        Titulo = entidadeAtualizada.Titulo;
    }
}
