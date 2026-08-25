using eAgenda.Aplicacao.Modulos.ModuloContato;
using eAgenda.Dominio.Modulos.ModuloCompromisso;
using eAgenda.Dominio.Modulos.ModuloContato;
using FluentResults;
using Moq;
namespace eAgenda.Testes.Unidade.Modulos.ModuloContato;

[TestClass]
public sealed class ServicoContatoTestes
{
    [TestMethod]
    public void Cadastrar_ComTodosCampos_PersisteContato()
    {
        Mock<IRepositorioContato> repositorioContato = new();
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();

        repositorioContato.Setup(r => r.SelecionarTodos()).Returns([]);

        Contato? contatoCadastrado = null;

        repositorioContato
            .Setup(r => r.Cadastrar(It.IsAny<Contato>()))
            .Callback<Contato>(
                contato => contatoCadastrado = contato
            );

        ServicoContato servicoContato = new ServicoContato(
            repositorioContato.Object,
            repositorioCompromisso.Object
        );

        Result resultado = servicoContato.Cadastrar(new CadastrarContatoDto(
            "Natalia Bortoli Vieira",
            "nataliabv@gmail.com",
            "(48) 99988-7788",
            "Programadora",
            "Amazon"
        ));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(contatoCadastrado);
        Assert.AreEqual("Natalia Bortoli Vieira", contatoCadastrado.Nome);
        Assert.AreEqual("nataliabv@gmail.com", contatoCadastrado.Email);
        Assert.AreEqual("(48) 99988-7788", contatoCadastrado.Telefone);
        Assert.AreEqual("Programadora", contatoCadastrado.Cargo);
        Assert.AreEqual("Amazon", contatoCadastrado.Empresa);

        repositorioContato.Verify(r => r.Cadastrar(It.IsAny<Contato>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_ApenasCamposObrigatorios_PersisteContato()
    {
        Mock<IRepositorioContato> repositorioContato = new();
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();

        repositorioContato.Setup(r => r.SelecionarTodos()).Returns([]);

        Contato? contatoCadastrado = null;

        repositorioContato
            .Setup(r => r.Cadastrar(It.IsAny<Contato>()))
            .Callback<Contato>(
                contato => contatoCadastrado = contato
            );

        ServicoContato servicoContato = new ServicoContato(
            repositorioContato.Object,
            repositorioCompromisso.Object
        );

        Result resultado = servicoContato.Cadastrar(new CadastrarContatoDto(
            "Natalia Bortoli Vieira",
            "nataliabv@gmail.com",
            "(48) 99988-7788",
            null,
            null
        ));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(contatoCadastrado);
        Assert.AreEqual("Natalia Bortoli Vieira", contatoCadastrado.Nome);
        Assert.AreEqual("nataliabv@gmail.com", contatoCadastrado.Email);
        Assert.AreEqual("(48) 99988-7788", contatoCadastrado.Telefone);
        Assert.IsNull(contatoCadastrado.Cargo);
        Assert.IsNull(contatoCadastrado.Empresa);

        repositorioContato.Verify(r => r.Cadastrar(It.IsAny<Contato>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_ComNomeVazio_RetornaErro()
    {
        Mock<IRepositorioContato> repositorioContato = new();
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();

        repositorioContato.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoContato servicoContato = new(
            repositorioContato.Object,
            repositorioCompromisso.Object
        );

        Result resultado = servicoContato.Cadastrar(new CadastrarContatoDto(
            string.Empty,
            "nataliabv@gmail.com",
            "(48) 99988-7788",
            null,
            null
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("O campo \"Nome\" deve ser preenchido.", resultado.Errors.First().Message);

        repositorioContato.Verify(r => r.Cadastrar(It.IsAny<Contato>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_ComEmailVazio_RetornaErro()
    {
        Mock<IRepositorioContato> repositorioContato = new();
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();

        repositorioContato.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoContato servicoContato = new(
            repositorioContato.Object,
            repositorioCompromisso.Object
        );

        Result resultado = servicoContato.Cadastrar(new CadastrarContatoDto(
            "Natalia Bortoli Vieira",
            string.Empty,
            "(48) 99988-7788",
            null,
            null
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("O campo \"E-mail\" deve ser preenchido.", resultado.Errors.First().Message);

        repositorioContato.Verify(r => r.Cadastrar(It.IsAny<Contato>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_ComTelefoneVazio_RetornaErro()
    {
        Mock<IRepositorioContato> repositorioContato = new();
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();

        repositorioContato.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoContato servicoContato = new(
            repositorioContato.Object,
            repositorioCompromisso.Object
        );

        Result resultado = servicoContato.Cadastrar(new CadastrarContatoDto(
            "Natalia Bortoli Vieira",
            "nataliabv@gmail.com",
            string.Empty,
            null,
            null
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("O campo \"Telefone\" deve ser preenchido.", resultado.Errors.First().Message);

        repositorioContato.Verify(r => r.Cadastrar(It.IsAny<Contato>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_EmailDuplicado_RetornaFalha()
    {
        Mock<IRepositorioContato> repositorioContato = new();
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();

        repositorioContato.Setup(r => r.SelecionarTodos())
        .Returns([new Contato(
            "Natalia Bortoli Vieira",
            "nataliabortolivieira@gmail.com",
            "(49) 99988-7766",
            null,
            null
        )]);

        ServicoContato servicoContato = new(
            repositorioContato.Object,
            repositorioCompromisso.Object
        );

        Result resultado = servicoContato.Cadastrar(new CadastrarContatoDto(
            "Natalia Vieira",
            "nataliabortolivieira@gmail.com",
            string.Empty,
            null,
            null
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Email", resultado.Errors.Single().Metadata["Campo"]);
        Assert.Contains("Já existe", resultado.Errors.Single().Message);

        repositorioContato.Verify(r => r.Cadastrar(It.IsAny<Contato>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_TelefoneDuplicado_RetornaFalha()
    {
        Mock<IRepositorioContato> repositorioContato = new();
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();

        repositorioContato.Setup(r => r.SelecionarTodos())
        .Returns([new Contato(
            "Natalia Bortoli Vieira",
            "nataliabortolivieira@gmail.com",
            "(49) 99988-7766",
            null,
            null
        )]);

        ServicoContato servicoContato = new(
            repositorioContato.Object,
            repositorioCompromisso.Object
        );

        Result resultado = servicoContato.Cadastrar(new CadastrarContatoDto(
            "Natalia Vieira",
            "nataliabvieira@gmail.com",
            "(49) 99988-7766",
            null,
            null
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Telefone", resultado.Errors.Single().Metadata["Campo"]);
        Assert.Contains("Já existe", resultado.Errors.Single().Message);

        repositorioContato.Verify(r => r.Cadastrar(It.IsAny<Contato>()), Times.Never);
    }

    [TestMethod]
    public void Editar_ComDadosValidos_PersisteContato()
    {
        Mock<IRepositorioContato> repositorioContato = new();
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();

        Contato contatoExistente = new Contato(
            "Natalia Vieira",
            "nbv@gmail.com",
            "(48) 99970-6544",
            "Desenvolvedora",
            "Academia do Programador"
        );

        List<Contato> contatos = new() { contatoExistente };

        repositorioContato.Setup(r => r.SelecionarTodos()).Returns(() => contatos);
        repositorioContato
            .Setup(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Contato>()))
            .Callback<Guid, Contato>((id, contatoAtualizado) =>
            {
                contatoAtualizado.Id = id;
                int index = contatos.FindIndex(c => c.Id == id);
                if (index >= 0)
                    contatos[index].Atualizar(contatoAtualizado);
            })
            .Returns<Guid, Contato>((id, contatoAtualizado) => contatos.Any(c => c.Id == id));

        ServicoContato servicoContato = new ServicoContato(
            repositorioContato.Object,
            repositorioCompromisso.Object
        );

        Result resultado = servicoContato.Editar(new EditarContatoDto(
            contatoExistente.Id,
            "Natalia Bortoli Vieira",
            "nataliabv@gmail.com",
            "(48) 99988-7788",
            "Programadora",
            "Amazon"
        ));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioContato.Verify(r => r.Editar(contatoExistente.Id, It.IsAny<Contato>()), Times.Once);

        List<ListarContatosDto> contatosListados = servicoContato.SelecionarTodos();

        Assert.HasCount(1, contatosListados);
        Assert.AreEqual("Natalia Bortoli Vieira", contatosListados[0].Nome);
        Assert.AreEqual("nataliabv@gmail.com", contatosListados[0].Email);
        Assert.AreEqual("(48) 99988-7788", contatosListados[0].Telefone);
        Assert.AreEqual("Programadora", contatosListados[0].Cargo);
        Assert.AreEqual("Amazon", contatosListados[0].Empresa);
    }

    [TestMethod]
    public void Editar_ComEmailDuplicado_RetornaFalha()
    {
        Mock<IRepositorioContato> repositorioContato = new();
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();

        Contato contatoExistente = new Contato(
            "Natalia Vieira",
            "nbv@gmail.com",
            "(48) 99970-6544",
            "Desenvolvedora",
            "Academia do Programador"
        );

        Contato outroContato = new Contato(
            "Maria Silva",
            "nataliabv@gmail.com",
            "(48) 11111-2222",
            "Analista",
            "Empresa"
        );

        List<Contato> contatos = new() { contatoExistente, outroContato };

        repositorioContato.Setup(r => r.SelecionarTodos()).Returns(() => contatos);

        ServicoContato servicoContato = new ServicoContato(
            repositorioContato.Object,
            repositorioCompromisso.Object
        );

        Result resultado = servicoContato.Editar(new EditarContatoDto(
            contatoExistente.Id,
            "Natalia Bortoli Vieira",
            "nataliabv@gmail.com",
            "(48) 99988-7788",
            "Programadora",
            "Amazon"
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Email", resultado.Errors.Single().Metadata["Campo"]);
        Assert.Contains("Já existe", resultado.Errors.Single().Message);
        repositorioContato.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Contato>()), Times.Never);
    }

    [TestMethod]
    public void Editar_ComTelefoneDuplicado_RetornaFalha()
    {
        Mock<IRepositorioContato> repositorioContato = new();
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();

        Contato contatoExistente = new Contato(
            "Natalia Vieira",
            "nbv@gmail.com",
            "(48) 99970-6544",
            "Desenvolvedora",
            "Academia do Programador"
        );

        Contato outroContato = new Contato(
            "Maria Silva",
            "nataliabv@gmail.com",
            "(48) 11111-2222",
            "Analista",
            "Empresa"
        );

        List<Contato> contatos = new() { contatoExistente, outroContato };

        repositorioContato.Setup(r => r.SelecionarTodos()).Returns(() => contatos);

        ServicoContato servicoContato = new ServicoContato(
            repositorioContato.Object,
            repositorioCompromisso.Object
        );

        Result resultado = servicoContato.Editar(new EditarContatoDto(
            contatoExistente.Id,
            "Natalia Bortoli Vieira",
            "nbv@gmail.com",
            "(48) 11111-2222",
            "Programadora",
            "Amazon"
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Telefone", resultado.Errors.Single().Metadata["Campo"]);
        Assert.Contains("Já existe", resultado.Errors.Single().Message);
        repositorioContato.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Contato>()), Times.Never);
    }

    [TestMethod]
    public void Editar_MantendoEmailTelefone_PersisteContato()
    {
        Mock<IRepositorioContato> repositorioContato = new();
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();

        Contato contatoExistente = new Contato(
            "Natalia Vieira",
            "nbv@gmail.com",
            "(48) 99970-6544",
            "Desenvolvedora",
            "Academia do Programador"
        );

        List<Contato> contatos = new() { contatoExistente };

        repositorioContato.Setup(r => r.SelecionarTodos()).Returns(() => contatos);
        repositorioContato
            .Setup(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Contato>()))
            .Callback<Guid, Contato>((id, contatoAtualizado) =>
            {
                contatoAtualizado.Id = id;
                int index = contatos.FindIndex(c => c.Id == id);
                if (index >= 0)
                    contatos[index].Atualizar(contatoAtualizado);
            })
            .Returns<Guid, Contato>((id, contatoAtualizado) => contatos.Any(c => c.Id == id));

        ServicoContato servicoContato = new ServicoContato(
            repositorioContato.Object,
            repositorioCompromisso.Object
        );

        Result resultado = servicoContato.Editar(new EditarContatoDto(
            contatoExistente.Id,
            "Natalia Bortoli Vieira",
            "nbv@gmail.com",
            "(48) 99970-6544",
            "Programadora",
            "Amazon"
        ));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioContato.Verify(r => r.Editar(contatoExistente.Id, It.IsAny<Contato>()), Times.Once);

        List<ListarContatosDto> contatosListados = servicoContato.SelecionarTodos();

        Assert.HasCount(1, contatosListados);
        Assert.AreEqual("Natalia Bortoli Vieira", contatosListados[0].Nome);
        Assert.AreEqual("nbv@gmail.com", contatosListados[0].Email);
        Assert.AreEqual("(48) 99970-6544", contatosListados[0].Telefone);
        Assert.AreEqual("Programadora", contatosListados[0].Cargo);
        Assert.AreEqual("Amazon", contatosListados[0].Empresa);
    }

    [TestMethod]
    public void SelecionarPorId_RetornaContato()
    {
        Mock<IRepositorioContato> repositorioContato = new();
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();

        Contato contatoExistente = new Contato(
            "Natalia Vieira",
            "nbv@gmail.com",
            "(48) 99970-6544",
            "Desenvolvedora",
            "Academia do Programador"
        );

        repositorioContato
            .Setup(r => r.SelecionarPorId(contatoExistente.Id))
            .Returns(contatoExistente);

        ServicoContato servicoContato = new ServicoContato(
            repositorioContato.Object,
            repositorioCompromisso.Object
        );

        Result<DetalhesContatoDto> resultado = servicoContato.SelecionarPorId(contatoExistente.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(resultado.Value);
        Assert.AreEqual(contatoExistente.Id, resultado.Value.Id);
        Assert.AreEqual("Natalia Vieira", resultado.Value.Nome);
        Assert.AreEqual("nbv@gmail.com", resultado.Value.Email);
        Assert.AreEqual("(48) 99970-6544", resultado.Value.Telefone);
        Assert.AreEqual("Desenvolvedora", resultado.Value.Cargo);
        Assert.AreEqual("Academia do Programador", resultado.Value.Empresa);
    }

    [TestMethod]
    public void SelecionarTodos_RetornaContatosCadastrados()
    {
        Mock<IRepositorioContato> repositorioContato = new();
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();

        List<Contato> contatos = new()
        {
            new Contato(
                "Natalia Vieira",
                "nbv@gmail.com",
                "(48) 99970-6544",
                "Desenvolvedora",
                "Academia do Programador"
            ),
            new Contato(
                "Maria Silva",
                "maria@gmail.com",
                "(48) 98888-7777",
                "Analista",
                "Empresa"
            )
        };

        repositorioContato.Setup(r => r.SelecionarTodos()).Returns(() => contatos);

        ServicoContato servicoContato = new ServicoContato(
            repositorioContato.Object,
            repositorioCompromisso.Object
        );

        List<ListarContatosDto> contatosListados = servicoContato.SelecionarTodos();

        Assert.HasCount(2, contatosListados);
        Assert.AreEqual("Natalia Vieira", contatosListados[0].Nome);
        Assert.AreEqual("nbv@gmail.com", contatosListados[0].Email);
        Assert.AreEqual("(48) 99970-6544", contatosListados[0].Telefone);
        Assert.AreEqual("Desenvolvedora", contatosListados[0].Cargo);
        Assert.AreEqual("Academia do Programador", contatosListados[0].Empresa);

        Assert.AreEqual("Maria Silva", contatosListados[1].Nome);
        Assert.AreEqual("maria@gmail.com", contatosListados[1].Email);
        Assert.AreEqual("(48) 98888-7777", contatosListados[1].Telefone);
        Assert.AreEqual("Analista", contatosListados[1].Cargo);
        Assert.AreEqual("Empresa", contatosListados[1].Empresa);
    }

    [TestMethod]
    public void Excluir_SemCompromissosVinculados_ExcluiContato()
    {
        Mock<IRepositorioContato> repositorioContato = new();
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();

        Contato contato = new Contato(
            "Natalia Vieira",
            "nbv@gmail.com",
            "(48) 99970-6544",
            "Desenvolvedora",
            "Academia do Programador"
        );

        repositorioContato
            .Setup(r => r.SelecionarPorId(contato.Id))
            .Returns(contato);
        repositorioCompromisso
            .Setup(r => r.SelecionarTodos())
            .Returns(new List<Compromisso>());

        ServicoContato servicoContato = new ServicoContato(
            repositorioContato.Object,
            repositorioCompromisso.Object
        );

        Result resultado = servicoContato.Excluir(contato.Id);

        Assert.IsTrue(resultado.IsSuccess);
        repositorioContato.Verify(r => r.Excluir(contato.Id), Times.Once);
    }

    [TestMethod]
    public void Excluir_ComCompromissosVinculados_RetornaFalha()
    {
        Mock<IRepositorioContato> repositorioContato = new();
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();

        Contato contato = new Contato(
            "Natalia Vieira",
            "nbv@gmail.com",
            "(48) 99970-6544",
            "Desenvolvedora",
            "Academia do Programador"
        );

        Compromisso compromissoVinculado = new Compromisso(
            "Reunião com cliente",
            DateTime.Today,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(11),
            TipoCompromisso.Presencial,
            "Sala 1",
            null,
            contato
        );

        repositorioContato
            .Setup(r => r.SelecionarPorId(contato.Id))
            .Returns(contato);
        repositorioCompromisso
            .Setup(r => r.SelecionarTodos())
            .Returns(new List<Compromisso> { compromissoVinculado });

        ServicoContato servicoContato = new ServicoContato(
            repositorioContato.Object,
            repositorioCompromisso.Object
        );

        Result resultado = servicoContato.Excluir(contato.Id);

        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("compromissos vinculados", resultado.Errors.Single().Message);
        repositorioContato.Verify(r => r.Excluir(contato.Id), Times.Never);
    }

}
