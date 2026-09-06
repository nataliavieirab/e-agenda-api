using eAgenda.Dominio.Compartilhado;
using eAgenda.Dominio.Modulos.ModuloCategoria;

namespace eAgenda.Testes.Unidade.Modulos.ModuloCategoria;

[TestClass]
public sealed class CategoriaTests
{

    [TestMethod]
    public void Validar_DeveCadastrar_ComDadosValidos()
    {
        Categoria categoria = new("Mercado");

        IReadOnlyList<ErroValidacao> erros = categoria.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_ComTituloVazio_DeveRetornarErro()
    {
        Categoria categoria = new(string.Empty);

        IReadOnlyList<ErroValidacao> erros = categoria.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(nameof(Categoria.Titulo), erros.First().Campo);
        Assert.AreEqual(
            "O campo \"Título\" deve ser preenchido.",
            erros.First().Mensagem
        );
    }

    [TestMethod]
    public void Validar_ComTituloCurto_DeveRetornarErro()
    {
        Categoria categoria = new(new string('A', 1));

        IReadOnlyList<ErroValidacao> erros = categoria.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(nameof(Categoria.Titulo), erros.First().Campo);
        Assert.AreEqual(
            "O campo \"Título\" deve conter no mínimo 2 caracteres.",
            erros.First().Mensagem
        );
    }


    [TestMethod]
    public void Validar_ComTituloTamanhoLimite()
    {
        Categoria categoria = new(new string('A', 2));

        IReadOnlyList<ErroValidacao> erros = categoria.Validar();

        Assert.HasCount(0, erros);
    }


    [TestMethod]
    public void Validar_ComTituloLongo_DeveRetornarErro()
    {
        Categoria categoria = new(new string('A', 101));

        IReadOnlyList<ErroValidacao> erros = categoria.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(nameof(Categoria.Titulo), erros.First().Campo);
        Assert.AreEqual(
            "O campo \"Título\" deve conter no máximo 100 caracteres.",
            erros.First().Mensagem
        );
    }

    [TestMethod]
    public void Validar_ComTituloTamanhoMaximo()
    {
        Categoria categoria = new(new string('A', 100));

        IReadOnlyList<ErroValidacao> erros = categoria.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Atualizar_ComDadosValidos()
    {
        Categoria categoria = new("Mercado");

        Categoria categoriaAtualizada = new("Petshop");

        categoria.Atualizar(categoriaAtualizada);

        Assert.AreEqual("Petshop", categoria.Titulo);
    }

}
