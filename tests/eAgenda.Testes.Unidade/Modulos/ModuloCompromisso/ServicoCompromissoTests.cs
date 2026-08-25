using eAgenda.Aplicacao.Modulos.ModuloCompromisso;
using eAgenda.Dominio.Modulos.ModuloCompromisso;
using eAgenda.Dominio.Modulos.ModuloContato;
using FluentResults;
using Moq;

namespace eAgenda.Testes.Unidade.Modulos.ModuloCompromisso;

[TestClass]
public sealed class ServicoCompromissoTests
{
    [TestMethod]
    public void Cadastrar_DadosValidos_PersisteCompromissoPresencial()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        Mock<IRepositorioContato> repositorioContato = new Mock<IRepositorioContato>();

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns([]);

        Compromisso? compromissoCadastrado = null;

        repositorioCompromisso
            .Setup(r => r.Cadastrar(It.IsAny<Compromisso>()))
            .Callback<Compromisso>(
                compromisso => compromissoCadastrado = compromisso
            );

        Contato contato = new Contato("João Silva", "carlos@email.com.br", "99999-9999", "Empresa X", "Gerente");

        repositorioContato
            .Setup(r => r.SelecionarPorId(contato.Id))
            .Returns(contato);

        ServicoCompromisso servicoCompromisso = new ServicoCompromisso(
            repositorioCompromisso.Object,
            repositorioContato.Object
        );

        // Act
        Result resultado = servicoCompromisso.Cadastrar(new CadastrarCompromissoDto(
            "Almoço de negócios",
            new DateTime(2026, 8, 8),
            new TimeSpan(12, 0, 0),
            new TimeSpan(13, 0, 0),
            TipoCompromisso.Presencial,
            "Restaurante Central",
            null,
            contato.Id
        ));

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(compromissoCadastrado);
        Assert.AreEqual("Almoço de negócios", compromissoCadastrado.Assunto);
        Assert.AreEqual(new DateTime(2026, 8, 8), compromissoCadastrado.DataOcorrencia);
        Assert.AreEqual(new TimeSpan(12, 0, 0), compromissoCadastrado.HoraInicio);
        Assert.AreEqual(new TimeSpan(13, 0, 0), compromissoCadastrado.HoraTermino);
        Assert.AreEqual(TipoCompromisso.Presencial, compromissoCadastrado.Tipo);
        Assert.AreEqual("Restaurante Central", compromissoCadastrado.Local);
        Assert.IsNull(compromissoCadastrado.Link);
        Assert.AreEqual(contato, compromissoCadastrado.Contato);

        repositorioCompromisso.Verify(r => r.Cadastrar(It.IsAny<Compromisso>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_DadosValidos_PersisteCompromissoRemoto()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        Mock<IRepositorioContato> repositorioContato = new Mock<IRepositorioContato>();

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns([]);

        Compromisso? compromissoCadastrado = null;

        repositorioCompromisso
            .Setup(r => r.Cadastrar(It.IsAny<Compromisso>()))
            .Callback<Compromisso>(
                compromisso => compromissoCadastrado = compromisso
            );

        Contato contato = new Contato("João Silva", "carlos@email.com.br", "99999-9999", "Empresa X", "Gerente");

        repositorioContato
            .Setup(r => r.SelecionarPorId(contato.Id))
            .Returns(contato);

        ServicoCompromisso servicoCompromisso = new ServicoCompromisso(
            repositorioCompromisso.Object,
            repositorioContato.Object
        );

        // Act
        Result resultado = servicoCompromisso.Cadastrar(new CadastrarCompromissoDto(
            "Almoço de negócios",
            new DateTime(2026, 8, 8),
            new TimeSpan(12, 0, 0),
            new TimeSpan(13, 0, 0),
            TipoCompromisso.Remoto,
            null,
            "www.reuniao.com.br",
            contato.Id
        ));

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(compromissoCadastrado);
        Assert.AreEqual("Almoço de negócios", compromissoCadastrado.Assunto);
        Assert.AreEqual(new DateTime(2026, 8, 8), compromissoCadastrado.DataOcorrencia);
        Assert.AreEqual(new TimeSpan(12, 0, 0), compromissoCadastrado.HoraInicio);
        Assert.AreEqual(new TimeSpan(13, 0, 0), compromissoCadastrado.HoraTermino);
        Assert.AreEqual(TipoCompromisso.Remoto, compromissoCadastrado.Tipo);
        Assert.IsNull(compromissoCadastrado.Local);
        Assert.AreEqual("www.reuniao.com.br", compromissoCadastrado.Link);
        Assert.AreEqual(contato, compromissoCadastrado.Contato);

        repositorioCompromisso.Verify(r => r.Cadastrar(It.IsAny<Compromisso>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_SemContato_PersisteCompromisso()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        Mock<IRepositorioContato> repositorioContato = new Mock<IRepositorioContato>();

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns([]);

        Compromisso? compromissoCadastrado = null;

        repositorioCompromisso
            .Setup(r => r.Cadastrar(It.IsAny<Compromisso>()))
            .Callback<Compromisso>(
                compromisso => compromissoCadastrado = compromisso
            );

        ServicoCompromisso servicoCompromisso = new ServicoCompromisso(
            repositorioCompromisso.Object,
            repositorioContato.Object
        );

        // Act
        Result resultado = servicoCompromisso.Cadastrar(new CadastrarCompromissoDto(
            "Almoço de negócios",
            new DateTime(2026, 8, 8),
            new TimeSpan(12, 0, 0),
            new TimeSpan(13, 0, 0),
            TipoCompromisso.Remoto,
            null,
            "www.reuniao.com.br",
            null
        ));

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(compromissoCadastrado);
        Assert.AreEqual("Almoço de negócios", compromissoCadastrado.Assunto);
        Assert.AreEqual(new DateTime(2026, 8, 8), compromissoCadastrado.DataOcorrencia);
        Assert.AreEqual(new TimeSpan(12, 0, 0), compromissoCadastrado.HoraInicio);
        Assert.AreEqual(new TimeSpan(13, 0, 0), compromissoCadastrado.HoraTermino);
        Assert.AreEqual(TipoCompromisso.Remoto, compromissoCadastrado.Tipo);
        Assert.IsNull(compromissoCadastrado.Local);
        Assert.AreEqual("www.reuniao.com.br", compromissoCadastrado.Link);
        Assert.IsNull(compromissoCadastrado.Contato);

        repositorioCompromisso.Verify(r => r.Cadastrar(It.IsAny<Compromisso>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_ComContato_PersisteCompromisso()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        Mock<IRepositorioContato> repositorioContato = new Mock<IRepositorioContato>();

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns([]);

        Compromisso? compromissoCadastrado = null;

        repositorioCompromisso
            .Setup(r => r.Cadastrar(It.IsAny<Compromisso>()))
            .Callback<Compromisso>(
                compromisso => compromissoCadastrado = compromisso
            );

        Contato contato = new Contato("João Silva", "carlos@email.com.br", "99999-9999", "Empresa X", "Gerente");

        repositorioContato
            .Setup(r => r.SelecionarPorId(contato.Id))
            .Returns(contato);

        ServicoCompromisso servicoCompromisso = new ServicoCompromisso(
            repositorioCompromisso.Object,
            repositorioContato.Object
        );

        // Act
        Result resultado = servicoCompromisso.Cadastrar(new CadastrarCompromissoDto(
            "Almoço de negócios",
            new DateTime(2026, 8, 8),
            new TimeSpan(12, 0, 0),
            new TimeSpan(13, 0, 0),
            TipoCompromisso.Presencial,
            "Restaurante Central",
            null,
            contato.Id
        ));

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(compromissoCadastrado);
        Assert.AreEqual("Almoço de negócios", compromissoCadastrado.Assunto);
        Assert.AreEqual(new DateTime(2026, 8, 8), compromissoCadastrado.DataOcorrencia);
        Assert.AreEqual(new TimeSpan(12, 0, 0), compromissoCadastrado.HoraInicio);
        Assert.AreEqual(new TimeSpan(13, 0, 0), compromissoCadastrado.HoraTermino);
        Assert.AreEqual(TipoCompromisso.Presencial, compromissoCadastrado.Tipo);
        Assert.AreEqual("Restaurante Central", compromissoCadastrado.Local);
        Assert.IsNull(compromissoCadastrado.Link);
        Assert.AreEqual(contato, compromissoCadastrado.Contato);

        repositorioCompromisso.Verify(r => r.Cadastrar(It.IsAny<Compromisso>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_ComAssuntoVazio_RetornaErro()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();
        Mock<IRepositorioContato> repositorioContato = new();

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoCompromisso servicoCompromisso = new(
            repositorioCompromisso.Object,
            repositorioContato.Object
        );

        Result resultado = servicoCompromisso.Cadastrar(new CadastrarCompromissoDto(
            string.Empty,
            new DateTime(2026, 8, 8),
            new TimeSpan(12, 0, 0),
            new TimeSpan(13, 0, 0),
            TipoCompromisso.Remoto,
            null,
            "www.reuniao.com.br",
            null
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("O campo \"Assunto\" deve conter entre 2 e 100 caracteres.", resultado.Errors.First().Message);

        repositorioContato.Verify(r => r.Cadastrar(It.IsAny<Contato>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_ComDataOcorrenciaVazia_RetornaErro()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new();
        Mock<IRepositorioContato> repositorioContato = new();

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoCompromisso servicoCompromisso = new(
            repositorioCompromisso.Object,
            repositorioContato.Object
        );

        Result resultado = servicoCompromisso.Cadastrar(new CadastrarCompromissoDto(
            "Reunião",
            default,
            new TimeSpan(12, 0, 0),
            new TimeSpan(13, 0, 0),
            TipoCompromisso.Remoto,
            null,
            "www.reuniao.com.br",
            null
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("O campo \"Data de Ocorrência\" deve ser preenchido.", resultado.Errors.First().Message);

        repositorioContato.Verify(r => r.Cadastrar(It.IsAny<Contato>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_ComHoraTerminoVazia_RetornaErro()
    {
        Compromisso compromisso = new Compromisso(
            "Reunião",
            new DateTime(2026, 8, 8),
            new TimeSpan(12, 0, 0),
            default,
            TipoCompromisso.Presencial,
            "Sala 101",
            null,
            null
        );

        List<string> erros = compromisso.Validar();

        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Hora de Término\" deve ser preenchido.", erros[0]);
    }

    [TestMethod]
    public void Cadastrar_ComTipoInvalido_RetornaErro()
    {
        Compromisso compromisso = new Compromisso(
            "Reunião",
            new DateTime(2026, 8, 8),
            new TimeSpan(12, 0, 0),
            new TimeSpan(13, 0, 0),
            (TipoCompromisso)999,
            "Sala 101",
            null,
            null
        );

        List<string> erros = compromisso.Validar();

        Assert.AreEqual("O campo \"Tipo de Compromisso\" deve ser preenchido.", erros.Single());
    }

    [TestMethod]
    public void Cadastrar_PresencialSemLocal_RetornaErro()
    {
        Compromisso compromisso = new Compromisso(
            "Reunião",
            new DateTime(2026, 8, 8),
            new TimeSpan(12, 0, 0),
            new TimeSpan(13, 0, 0),
            TipoCompromisso.Presencial,
            null,
            null,
            null
        );

        List<string> erros = compromisso.Validar();

        Assert.AreEqual("O campo \"Local\" deve ser preenchido para compromissos presenciais.", erros.Single());
    }

    [TestMethod]
    public void Cadastrar_RemotoSemLink_RetornaErro()
    {
        Compromisso compromisso = new Compromisso(
            "Reunião",
            new DateTime(2026, 8, 8),
            new TimeSpan(12, 0, 0),
            new TimeSpan(13, 0, 0),
            TipoCompromisso.Remoto,
            null,
            null,
            null
        );

        List<string> erros = compromisso.Validar();

        Assert.AreEqual("O campo \"Link\" deve ser preenchido para compromissos remotos.", erros.Single());
    }


    [TestMethod]
    public void Cadastrar_CompromissoImediatamenteAposOutro_PersisteComSucesso()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        Mock<IRepositorioContato> repositorioContato = new Mock<IRepositorioContato>();

        Compromisso compromissoExistente = new Compromisso(
            "Reunião de equipe",
            new DateTime(2026, 8, 10),
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 101",
            null,
            null
        );

        repositorioCompromisso.Setup(r => r.SelecionarTodos())
            .Returns([compromissoExistente]);

        Compromisso? compromissoCadastrado = null;

        repositorioCompromisso
            .Setup(r => r.Cadastrar(It.IsAny<Compromisso>()))
            .Callback<Compromisso>(c => compromissoCadastrado = c);

        ServicoCompromisso servicoCompromisso = new ServicoCompromisso(
            repositorioCompromisso.Object,
            repositorioContato.Object
        );

        Result resultado = servicoCompromisso.Cadastrar(new CadastrarCompromissoDto(
            "Treinamento",
            new DateTime(2026, 8, 10),
            new TimeSpan(10, 0, 0),
            new TimeSpan(11, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 102",
            null,
            null
        ));

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(compromissoCadastrado);
        Assert.AreEqual("Treinamento", compromissoCadastrado.Assunto);
        Assert.AreEqual(new DateTime(2026, 8, 10), compromissoCadastrado.DataOcorrencia);
        Assert.AreEqual(new TimeSpan(10, 0, 0), compromissoCadastrado.HoraInicio);
        Assert.AreEqual(new TimeSpan(11, 0, 0), compromissoCadastrado.HoraTermino);
        Assert.AreEqual(TipoCompromisso.Presencial, compromissoCadastrado.Tipo);
        Assert.AreEqual("Sala 102", compromissoCadastrado.Local);

        repositorioCompromisso.Verify(r => r.Cadastrar(It.IsAny<Compromisso>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_CompromissoMesmoHorarioEmDataDiferente_PersisteComSucesso()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        Mock<IRepositorioContato> repositorioContato = new Mock<IRepositorioContato>();

        Compromisso compromissoExistente = new Compromisso(
            "Reunião de equipe",
            new DateTime(2026, 8, 10),
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 101",
            null,
            null
        );

        repositorioCompromisso.Setup(r => r.SelecionarTodos())
            .Returns([compromissoExistente]);

        Compromisso? compromissoCadastrado = null;

        repositorioCompromisso
            .Setup(r => r.Cadastrar(It.IsAny<Compromisso>()))
            .Callback<Compromisso>(c => compromissoCadastrado = c);

        ServicoCompromisso servicoCompromisso = new ServicoCompromisso(
            repositorioCompromisso.Object,
            repositorioContato.Object
        );

        Result resultado = servicoCompromisso.Cadastrar(new CadastrarCompromissoDto(
            "Treinamento",
            new DateTime(2026, 8, 11),
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 102",
            null,
            null
        ));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(compromissoCadastrado);
        Assert.AreEqual("Treinamento", compromissoCadastrado.Assunto);
        Assert.AreEqual(new DateTime(2026, 8, 11), compromissoCadastrado.DataOcorrencia);
        Assert.AreEqual(new TimeSpan(9, 0, 0), compromissoCadastrado.HoraInicio);
        Assert.AreEqual(new TimeSpan(10, 0, 0), compromissoCadastrado.HoraTermino);
        Assert.AreEqual(TipoCompromisso.Presencial, compromissoCadastrado.Tipo);
        Assert.AreEqual("Sala 102", compromissoCadastrado.Local);

        repositorioCompromisso.Verify(r => r.Cadastrar(It.IsAny<Compromisso>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_ComSobreposicaoParcial_RetornaErro()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        Mock<IRepositorioContato> repositorioContato = new Mock<IRepositorioContato>();

        Compromisso compromissoExistente = new Compromisso(
            "Reunião",
            new DateTime(2026, 8, 10),
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 101",
            null,
            null
        );

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns([compromissoExistente]);

        ServicoCompromisso servicoCompromisso = new(repositorioCompromisso.Object, repositorioContato.Object);

        Result resultado = servicoCompromisso.Cadastrar(new CadastrarCompromissoDto(
            "Treinamento",
            new DateTime(2026, 8, 10),
            new TimeSpan(9, 30, 0),
            new TimeSpan(10, 30, 0),
            TipoCompromisso.Presencial,
            "Sala 102",
            null,
            null
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Já existe um compromisso cadastrado neste intervalo de horário.", resultado.Errors.First().Message);
    }

    [TestMethod]
    public void Cadastrar_ComCompromissoContido_RetornaErro()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        Mock<IRepositorioContato> repositorioContato = new Mock<IRepositorioContato>();

        Compromisso compromissoExistente = new Compromisso(
            "Reunião longa",
            new DateTime(2026, 8, 10),
            new TimeSpan(9, 0, 0),
            new TimeSpan(12, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 101",
            null,
            null
        );

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns([compromissoExistente]);

        ServicoCompromisso servicoCompromisso = new(repositorioCompromisso.Object, repositorioContato.Object);

        Result resultado = servicoCompromisso.Cadastrar(new CadastrarCompromissoDto(
            "Treinamento",
            new DateTime(2026, 8, 10),
            new TimeSpan(10, 0, 0),
            new TimeSpan(11, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 102",
            null,
            null
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Já existe um compromisso cadastrado neste intervalo de horário.", resultado.Errors.First().Message);
    }

    [TestMethod]
    public void Cadastrar_ComCompromissoEnglobandoOutro_RetornaErro()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        Mock<IRepositorioContato> repositorioContato = new Mock<IRepositorioContato>();

        Compromisso compromissoExistente = new Compromisso(
            "Reunião curta",
            new DateTime(2026, 8, 10),
            new TimeSpan(10, 0, 0),
            new TimeSpan(11, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 101",
            null,
            null
        );

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns([compromissoExistente]);

        ServicoCompromisso servicoCompromisso = new(repositorioCompromisso.Object, repositorioContato.Object);

        Result resultado = servicoCompromisso.Cadastrar(new CadastrarCompromissoDto(
            "Treinamento",
            new DateTime(2026, 8, 10),
            new TimeSpan(9, 0, 0),
            new TimeSpan(12, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 102",
            null,
            null
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Já existe um compromisso cadastrado neste intervalo de horário.", resultado.Errors.First().Message);
    }

    [TestMethod]
    public void Editar_CompromissoComDadosValidos_AlteraERefleteNaListagem()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        Mock<IRepositorioContato> repositorioContato = new Mock<IRepositorioContato>();

        Compromisso compromissoExistente = new Compromisso(
            "Reunião de equipe",
            new DateTime(2026, 8, 10),
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 101",
            null,
            null
        );

        List<Compromisso> compromissos = new() { compromissoExistente };

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns(() => compromissos);

        repositorioCompromisso
            .Setup(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Compromisso>()))
            .Callback<Guid, Compromisso>((id, compromissoAtualizado) =>
            {
                compromissoAtualizado.Id = id;
                int index = compromissos.FindIndex(c => c.Id == id);
                if (index >= 0)
                    compromissos[index].Atualizar(compromissoAtualizado);
            })
            .Returns<Guid, Compromisso>((id, compromissoAtualizado) => compromissos.Any(c => c.Id == id));

        ServicoCompromisso servicoCompromisso = new ServicoCompromisso(
            repositorioCompromisso.Object,
            repositorioContato.Object
        );

        Result resultado = servicoCompromisso.Editar(new EditarCompromissoDto(
            compromissoExistente.Id,
            "Reunião de planejamento",
            new DateTime(2026, 8, 11),
            new TimeSpan(14, 0, 0),
            new TimeSpan(15, 30, 0),
            TipoCompromisso.Remoto,
            null,
            "https://meet.google.com/reuniao",
            null
        ));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioCompromisso.Verify(r => r.Editar(compromissoExistente.Id, It.IsAny<Compromisso>()), Times.Once);

        List<ListarCompromissosDto> compromissosListados = servicoCompromisso.SelecionarTodos();

        Assert.HasCount(1, compromissosListados);
        Assert.AreEqual("Reunião de planejamento", compromissosListados[0].Assunto);
        Assert.AreEqual(new DateTime(2026, 8, 11), compromissosListados[0].DataOcorrencia);
        Assert.AreEqual(new TimeSpan(14, 0, 0), compromissosListados[0].HoraInicio);
        Assert.AreEqual(new TimeSpan(15, 30, 0), compromissosListados[0].HoraTermino);
        Assert.AreEqual(TipoCompromisso.Remoto, compromissosListados[0].Tipo);
        Assert.AreEqual("https://meet.google.com/reuniao", compromissosListados[0].Link);
    }

    [TestMethod]
    public void Editar_CompromissoGerandoConflito_NaoSalvaEExibeErro()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        Mock<IRepositorioContato> repositorioContato = new Mock<IRepositorioContato>();

        Compromisso compromissoA = new Compromisso("Reunião A", new DateTime(2026, 8, 10),
            new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0), TipoCompromisso.Presencial, "Sala 101", null, null);

        Compromisso compromissoB = new Compromisso("Reunião B", new DateTime(2026, 8, 10),
            new TimeSpan(14, 0, 0), new TimeSpan(15, 0, 0), TipoCompromisso.Remoto, null, "https://meet.google.com/b", null);

        List<Compromisso> compromissos = new() { compromissoA, compromissoB };

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns(() => compromissos);

        ServicoCompromisso servicoCompromisso = new ServicoCompromisso(
            repositorioCompromisso.Object,
            repositorioContato.Object
        );

        Result resultado = servicoCompromisso.Editar(new EditarCompromissoDto(
            compromissoA.Id,
            "Reunião A Conflitante",
            new DateTime(2026, 8, 10),
            new TimeSpan(14, 0, 0),
            new TimeSpan(15, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 102",
            null,
            null
        ));

        Assert.IsFalse(resultado.IsSuccess);
        Assert.AreEqual("Já existe um compromisso cadastrado neste intervalo de horário.", resultado.Errors[0].Message);

        repositorioCompromisso.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Compromisso>()), Times.Never);

        List<ListarCompromissosDto> compromissosListados = servicoCompromisso.SelecionarTodos();
        Assert.AreEqual("Reunião A", compromissosListados[0].Assunto);
    }

    [TestMethod]
    public void Editar_CompromissoMantendoMesmoHorario_SalvaSemConflito()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        Mock<IRepositorioContato> repositorioContato = new Mock<IRepositorioContato>();

        Compromisso compromissoExistente = new Compromisso(
            "Reunião de equipe",
            new DateTime(2026, 8, 10),
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 101",
            null,
            null
        );

        List<Compromisso> compromissos = new() { compromissoExistente };

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns(() => compromissos);

        repositorioCompromisso
            .Setup(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Compromisso>()))
            .Callback<Guid, Compromisso>((id, compromissoAtualizado) =>
            {
                compromissoAtualizado.Id = id;
                int index = compromissos.FindIndex(c => c.Id == id);
                if (index >= 0)
                    compromissos[index].Atualizar(compromissoAtualizado);
            })
            .Returns<Guid, Compromisso>((id, compromissoAtualizado) => compromissos.Any(c => c.Id == id));

        ServicoCompromisso servicoCompromisso = new ServicoCompromisso(
            repositorioCompromisso.Object,
            repositorioContato.Object
        );

        Result resultado = servicoCompromisso.Editar(new EditarCompromissoDto(
            compromissoExistente.Id,
            "Reunião atualizada",
            new DateTime(2026, 8, 10),
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 202",
            null,
            null
        ));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioCompromisso.Verify(r => r.Editar(compromissoExistente.Id, It.IsAny<Compromisso>()), Times.Once);

        List<ListarCompromissosDto> compromissosListados = servicoCompromisso.SelecionarTodos();

        Assert.AreEqual("Reunião atualizada", compromissosListados[0].Assunto);
        Assert.AreEqual("Sala 202", compromissosListados[0].Local);
        Assert.AreEqual(new TimeSpan(9, 0, 0), compromissosListados[0].HoraInicio);
        Assert.AreEqual(new TimeSpan(10, 0, 0), compromissosListados[0].HoraTermino);
    }

    [TestMethod]
    public void Editar_CompromissoPresencialParaRemoto_AtualizaCamposObrigatorios()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        Mock<IRepositorioContato> repositorioContato = new Mock<IRepositorioContato>();

        Compromisso compromissoExistente = new Compromisso(
            "Reunião de equipe",
            new DateTime(2026, 8, 10),
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 101",
            null,
            null
        );

        List<Compromisso> compromissos = new() { compromissoExistente };

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns(() => compromissos);

        repositorioCompromisso
            .Setup(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Compromisso>()))
            .Callback<Guid, Compromisso>((id, compromissoAtualizado) =>
            {
                compromissoAtualizado.Id = id;
                int index = compromissos.FindIndex(c => c.Id == id);
                if (index >= 0)
                    compromissos[index].Atualizar(compromissoAtualizado);
            })
            .Returns<Guid, Compromisso>((id, compromissoAtualizado) => compromissos.Any(c => c.Id == id));

        ServicoCompromisso servicoCompromisso = new ServicoCompromisso(
            repositorioCompromisso.Object,
            repositorioContato.Object
        );

        Result resultado = servicoCompromisso.Editar(new EditarCompromissoDto(
            compromissoExistente.Id,
            "Reunião remota",
            new DateTime(2026, 8, 10),
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            TipoCompromisso.Remoto,
            null,
            "https://meet.google.com/reuniao",
            null
        ));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioCompromisso.Verify(r => r.Editar(compromissoExistente.Id, It.IsAny<Compromisso>()), Times.Once);

        List<ListarCompromissosDto> compromissosListados = servicoCompromisso.SelecionarTodos();

        Assert.AreEqual(TipoCompromisso.Remoto, compromissosListados[0].Tipo);
        Assert.IsNull(compromissosListados[0].Local);
        Assert.AreEqual("https://meet.google.com/reuniao", compromissosListados[0].Link);
    }

    [TestMethod]
    public void Visualizar_CompromissoCadastrado_ExibeDadosCorretamente()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        Mock<IRepositorioContato> repositorioContato = new Mock<IRepositorioContato>();

        Contato contato = new Contato("Maria", "maria@email.com", "99999-9999", "Gerente", null);

        Compromisso compromisso = new Compromisso(
            "Reunião de equipe",
            new DateTime(2026, 8, 10),
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 101",
            null,
            contato
        );

        List<Compromisso> compromissos = new() { compromisso };

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns(() => compromissos);

        ServicoCompromisso servicoCompromisso = new ServicoCompromisso(
            repositorioCompromisso.Object,
            repositorioContato.Object
        );

        List<ListarCompromissosDto> compromissosListados = servicoCompromisso.SelecionarTodos();

        Assert.AreEqual(1, compromissosListados.Count);
        Assert.AreEqual("Reunião de equipe", compromissosListados[0].Assunto);
        Assert.AreEqual(new DateTime(2026, 8, 10), compromissosListados[0].DataOcorrencia);
        Assert.AreEqual(new TimeSpan(9, 0, 0), compromissosListados[0].HoraInicio);
        Assert.AreEqual(new TimeSpan(10, 0, 0), compromissosListados[0].HoraTermino);
        Assert.AreEqual(TipoCompromisso.Presencial, compromissosListados[0].Tipo);
        Assert.AreEqual("Sala 101", compromissosListados[0].Local);
        Assert.IsNull(compromissosListados[0].Link);
        Assert.AreEqual("Maria", compromissosListados[0].ContatoNome);
    }

    [TestMethod]
    public void Listar_TodosCompromissosCadastrados_ExibeTodosCorretamente()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        Mock<IRepositorioContato> repositorioContato = new Mock<IRepositorioContato>();

        Contato contato1 = new Contato("Maria", "maria@email.com", "99999-9999", "Vendas", "Empresa132");
        Contato contato2 = new Contato("João", "joao@email.com", "88888-8888", "Vendas", "Empresa12");

        Compromisso compromisso1 = new Compromisso(
            "Reunião presencial",
            new DateTime(2026, 8, 10),
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 101",
            null,
            contato1
        );

        Compromisso compromisso2 = new Compromisso(
            "Reunião remota",
            new DateTime(2026, 8, 11),
            new TimeSpan(14, 0, 0),
            new TimeSpan(15, 0, 0),
            TipoCompromisso.Remoto,
            null,
            "https://meet.google.com/reuniao",
            contato2
        );

        List<Compromisso> compromissos = new() { compromisso1, compromisso2 };

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns(() => compromissos);

        ServicoCompromisso servicoCompromisso = new ServicoCompromisso(
            repositorioCompromisso.Object,
            repositorioContato.Object
        );

        List<ListarCompromissosDto> compromissosListados = servicoCompromisso.SelecionarTodos();

        Assert.AreEqual(2, compromissosListados.Count);

        Assert.AreEqual("Reunião presencial", compromissosListados[0].Assunto);
        Assert.AreEqual(TipoCompromisso.Presencial, compromissosListados[0].Tipo);
        Assert.AreEqual("Sala 101", compromissosListados[0].Local);
        Assert.AreEqual("Maria", compromissosListados[0].ContatoNome);

        Assert.AreEqual("Reunião remota", compromissosListados[1].Assunto);
        Assert.AreEqual(TipoCompromisso.Remoto, compromissosListados[1].Tipo);
        Assert.AreEqual("https://meet.google.com/reuniao", compromissosListados[1].Link);
        Assert.AreEqual("João", compromissosListados[1].ContatoNome);
    }

    [TestMethod]
    public void Excluir_CompromissoCadastrado()
    {
        Mock<IRepositorioCompromisso> repositorioCompromisso = new Mock<IRepositorioCompromisso>();
        Mock<IRepositorioContato> repositorioContato = new Mock<IRepositorioContato>();

        Compromisso compromisso = new Compromisso(
            "Reunião de equipe",
            new DateTime(2026, 8, 10),
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0),
            TipoCompromisso.Presencial,
            "Sala 101",
            null,
            null
        );

        List<Compromisso> compromissos = new() { compromisso };

        repositorioCompromisso.Setup(r => r.SelecionarTodos()).Returns(() => compromissos);

        repositorioCompromisso
            .Setup(r => r.SelecionarPorId(compromisso.Id))
            .Returns(compromisso);

        repositorioCompromisso
            .Setup(r => r.Excluir(It.IsAny<Guid>()))
            .Callback<Guid>(id =>
            {
                compromissos.RemoveAll(c => c.Id == id);
            })
            .Returns<Guid>(id => true);

        ServicoCompromisso servicoCompromisso = new ServicoCompromisso(
            repositorioCompromisso.Object,
            repositorioContato.Object
        );

        Result resultado = servicoCompromisso.Excluir(compromisso.Id);

        Assert.IsTrue(resultado.IsSuccess);
        repositorioCompromisso.Verify(r => r.Excluir(compromisso.Id), Times.Once);

        List<ListarCompromissosDto> compromissosListados = servicoCompromisso.SelecionarTodos();

        Assert.AreEqual(0, compromissosListados.Count);
    }
}