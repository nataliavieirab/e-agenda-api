using eAgenda.Dominio.Modulos.ModuloContato;
namespace eAgenda.Testes.Unidade.Modulos.ModuloContato;

[TestClass]
public sealed class ContatoTests
{
    [TestMethod]
    public void Validar_ComNomeVazio_DeveRetornarErro()
    {
        Contato contato = new Contato(string.Empty, "nbv@gmail.com", "(48) 99970-6544", "Desenvolvedora", "Academia do Programador");

        List<string> erros = contato.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Nome\" deve ser preenchido.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComNomeCurto_DeveRetornarErro()
    {
        Contato contato = new Contato(new string('A', 1), "nbv@gmail.com", "(48) 99970-6544", null, null);

        List<string> erros = contato.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Nome\" deve conter no mínimo 2 caracteres.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_NomeComTamanhoLimite()
    {
        Contato contato = new Contato(new string('A', 2), "nbv@gmail.com", "(48) 99970-6544", null, null);

        List<string> erros = contato.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_ComNomeLongo_DeveRetornarErro()
    {
        Contato contato = new Contato(new string('A', 101), "nbv@gmail.com", "(48) 99970-6544", string.Empty, string.Empty);

        List<string> erros = contato.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Nome\" deve conter no máximo 100 caracteres.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_NomeComTamanhoMaximo()
    {
        Contato contato = new Contato(new string('A', 100), "nbv@gmail.com", "(48) 99970-6544", null, null);

        List<string> erros = contato.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_EmailComFormatoInvalido_DeveRetornarErro()
    {
        Contato contato = new Contato("Natalia Vieira", new string('A', 10), "(48) 99970-6544", "Desenvolvedora", "Academia do Programador");

        List<string> erros = contato.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"E-mail\" deve conter um endereço de e-mail válido.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_EmailSemDominio_DeveRetornarErro()
    {
        Contato contato = new Contato("Natalia Vieira", "nbv@", "(48) 99970-6544", "Desenvolvedora", "Academia do Programador");

        List<string> erros = contato.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"E-mail\" deve conter um endereço de e-mail válido.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_TelefoneComFormatoInvalido_DeveRetornarErro()
    {
        Contato contato = new Contato("Natalia Vieira", "nbv@gmail.com", "48999706544", "Desenvolvedora", "Academia do Programador");

        List<string> erros = contato.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Telefone\" deve estar no formato (XX) XXXX-XXXX ou (XX) XXXXX-XXXX.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComTelefoneFixo()
    {
        Contato contato = new Contato("Natalia Vieira", "nbv@gmail.com", "(48) 3222-9900", "Desenvolvedora", "Academia do Programador");

        List<string> erros = contato.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_ComTelefoneCelular()
    {
        Contato contato = new Contato("Natalia Vieira", "nbv@gmail.com", "(48) 99970-6544", "Desenvolvedora", "Academia do Programador");

        List<string> erros = contato.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_CargoComNomeLongo_DeveRetornarErro()
    {
        Contato contato = new Contato("Natalia Vieira", "nbv@gmail.com", "(48) 99970-6544", new string('A', 101), null);

        List<string> erros = contato.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Cargo\" deve conter no máximo 100 caracteres.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_EmpresaComNomeLongo_DeveRetornarErro()
    {
        Contato contato = new Contato("Natalia Vieira", "nbv@gmail.com", "(48) 99970-6544", null, new string('A', 101));

        List<string> erros = contato.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Empresa\" deve conter no máximo 100 caracteres.",
            erros.First()
        );
    }

    [TestMethod]
    public void Atualizar_DeveAtualizar_DadosValidos()
    {
        Contato contato = new Contato(
            "Natalia Vieira",
            "nbv@gmail.com",
            "(48) 99970-6544",
            "Desenvolvedora",
            "Academia do Programador"
        );


        Contato contatoAtualizada = new Contato(
            "Natalia Bortoli Vieira",
            "nataliabv@gmail.com",
            "(48) 99988-7788",
            "Programadora",
            "Amazon"
        );

        contato.Atualizar(contatoAtualizada);

        Assert.AreEqual("Natalia Bortoli Vieira", contato.Nome);
        Assert.AreEqual("nataliabv@gmail.com", contato.Email);
        Assert.AreEqual("(48) 99988-7788", contato.Telefone);
        Assert.AreEqual("Programadora", contato.Cargo);
        Assert.AreEqual("Amazon", contato.Empresa);
    }
}
