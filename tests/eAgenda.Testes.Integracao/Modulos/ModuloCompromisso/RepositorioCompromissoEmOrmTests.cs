using eAgenda.Dominio.Modulos.ModuloCompromisso;
using eAgenda.Dominio.Modulos.ModuloContato;
using eAgenda.Testes.Integracao.Compartilhado.Orm;
using FizzWare.NBuilder;

namespace eAgenda.Testes.Integracao.Modulos.ModuloCompromisso;

[TestClass]
public class RepositorioCompromissoEmOrmTests : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public void Cadastrar_PresencialComDadosValidos_DevePersistirComSucesso()
    {
        Contato contato = Builder<Contato>
            .CreateNew()
            .With(c => c.Nome = "João Souza")
            .With(c => c.Email = "joao@gmail.com")
            .With(c => c.Telefone = "(51) 99454-4565")
            .With(c => c.Cargo = "Gerente")
            .With(c => c.Empresa = "Tanac")
            .Persist();

        Compromisso compromisso = Builder<Compromisso>
            .CreateNew()
            .With(c => c.Assunto = "Reunião Almoço")
            .With(c => c.DataOcorrencia = DateTime.Today.AddDays(1))
            .With(c => c.HoraInicio = new TimeSpan(12, 0, 0))
            .With(c => c.HoraTermino = new TimeSpan(13, 0, 0))
            .With(c => c.Tipo = TipoCompromisso.Presencial)
            .With(c => c.Local = "Restaurante Centro")
            .With(c => c.Link = null)
            .With(c => c.Contato = contato)
            .Build();

        repositorioCompromisso.Cadastrar(compromisso);
        dbContext.ChangeTracker.Clear();

        Compromisso? compromissoSelecionado = repositorioCompromisso.SelecionarPorId(compromisso.Id);

        Assert.IsNotNull(compromissoSelecionado);
        Assert.AreEqual("Reunião Almoço", compromissoSelecionado.Assunto);
        Assert.AreEqual("Restaurante Centro", compromissoSelecionado.Local);
        Assert.AreEqual(TipoCompromisso.Presencial, compromissoSelecionado.Tipo);

        Assert.IsNotNull(compromissoSelecionado.Contato);
        Assert.AreEqual("João Souza", compromissoSelecionado.Contato!.Nome);
        Assert.AreEqual("Tanac", compromissoSelecionado.Contato!.Empresa);
    }

    [TestMethod]
    public void Cadastrar_RemotoComDadosValidos_DevePersistirComSucesso()
    {
        Contato contato = Builder<Contato>
            .CreateNew()
            .With(c => c.Nome = "João Souza")
            .With(c => c.Email = "joao@gmail.com")
            .With(c => c.Telefone = "(51) 99454-4565")
            .With(c => c.Cargo = "Gerente")
            .With(c => c.Empresa = "Tanac")
            .Persist();

        Compromisso compromisso = Builder<Compromisso>
            .CreateNew()
            .With(c => c.Assunto = "Reunião Almoço")
            .With(c => c.DataOcorrencia = DateTime.Today.AddDays(1))
            .With(c => c.HoraInicio = new TimeSpan(12, 0, 0))
            .With(c => c.HoraTermino = new TimeSpan(13, 0, 0))
            .With(c => c.Tipo = TipoCompromisso.Remoto)
            .With(c => c.Local = null)
            .With(c => c.Link = "www.meet.com.br")
            .With(c => c.Contato = contato)
            .Build();

        repositorioCompromisso.Cadastrar(compromisso);
        dbContext.ChangeTracker.Clear();

        Compromisso? compromissoSelecionado = repositorioCompromisso.SelecionarPorId(compromisso.Id);

        Assert.IsNotNull(compromissoSelecionado);
        Assert.AreEqual("Reunião Almoço", compromissoSelecionado.Assunto);
        Assert.AreEqual("www.meet.com.br", compromissoSelecionado.Link);
        Assert.AreEqual(TipoCompromisso.Remoto, compromissoSelecionado.Tipo);

        Assert.IsNotNull(compromissoSelecionado.Contato);
        Assert.AreEqual("João Souza", compromissoSelecionado.Contato!.Nome);
        Assert.AreEqual("Tanac", compromissoSelecionado.Contato!.Empresa);
    }

    [TestMethod]
    public void Cadastrar_SemVincularContato_DevePersistirComSucesso()
    {
        Compromisso compromisso = Builder<Compromisso>
            .CreateNew()
            .With(c => c.Assunto = "Reunião Almoço")
            .With(c => c.DataOcorrencia = DateTime.Today.AddDays(1))
            .With(c => c.HoraInicio = new TimeSpan(12, 0, 0))
            .With(c => c.HoraTermino = new TimeSpan(13, 0, 0))
            .With(c => c.Tipo = TipoCompromisso.Presencial)
            .With(c => c.Local = "Restaurante Centro")
            .With(c => c.Link = null)
            .With(c => c.Contato = null)
            .Build();

        repositorioCompromisso.Cadastrar(compromisso);
        dbContext.ChangeTracker.Clear();

        Compromisso? compromissoEncontrado = repositorioCompromisso.SelecionarPorId(compromisso.Id);

        Assert.IsNotNull(compromissoEncontrado);
        Assert.AreEqual(compromisso.Id, compromissoEncontrado.Id);
        Assert.AreEqual("Reunião Almoço", compromissoEncontrado.Assunto);
        Assert.IsNull(compromissoEncontrado.Contato);
    }


    [TestMethod]
    public void Cadastrar_VincularContato_DevePersistirComSucesso()
    {
        Contato contato = Builder<Contato>
            .CreateNew()
            .With(c => c.Nome = "João Souza")
            .With(c => c.Email = "joao@gmail.com")
            .With(c => c.Telefone = "(51) 99454-4565")
            .With(c => c.Cargo = "Gerente")
            .With(c => c.Empresa = "Tanac")
            .Persist();

        Compromisso compromisso = Builder<Compromisso>
            .CreateNew()
            .With(c => c.Assunto = "Reunião Almoço")
            .With(c => c.DataOcorrencia = DateTime.Today.AddDays(1))
            .With(c => c.HoraInicio = new TimeSpan(12, 0, 0))
            .With(c => c.HoraTermino = new TimeSpan(13, 0, 0))
            .With(c => c.Tipo = TipoCompromisso.Presencial)
            .With(c => c.Local = "Restaurante Centro")
            .With(c => c.Link = null)
            .With(c => c.Contato = contato)
            .Build();

        repositorioCompromisso.Cadastrar(compromisso);
        dbContext.ChangeTracker.Clear();

        Compromisso? compromissoEncontrado = repositorioCompromisso.SelecionarPorId(compromisso.Id);

        Assert.IsNotNull(compromissoEncontrado);
        Assert.AreEqual(compromisso.Id, compromissoEncontrado.Id);
        Assert.AreEqual("Reunião Almoço", compromissoEncontrado.Assunto);

        Assert.IsNotNull(compromissoEncontrado.Contato);
        Assert.AreEqual("João Souza", compromissoEncontrado.Contato!.Nome);
        Assert.AreEqual("Tanac", compromissoEncontrado.Contato!.Empresa);
    }

    [TestMethod]
    public void Cadastrar_CompromissoMesmoHorarioEmDataDiferente_DevePersistirComSucesso()
    {
        Compromisso compromissoExistente = Builder<Compromisso>
            .CreateNew()
            .With(c => c.Assunto = "Reunião de planejamento")
            .With(c => c.DataOcorrencia = new DateTime(2026, 8, 10))
            .With(c => c.HoraInicio = new TimeSpan(9, 0, 0))
            .With(c => c.HoraTermino = new TimeSpan(10, 0, 0))
            .With(c => c.Tipo = TipoCompromisso.Presencial)
            .With(c => c.Local = "Sala 101")
            .With(c => c.Link = null)
            .With(c => c.Contato = null)
            .Build();

        repositorioCompromisso.Cadastrar(compromissoExistente);
        dbContext.ChangeTracker.Clear();

        Compromisso compromissoOutraData = Builder<Compromisso>
            .CreateNew()
            .With(c => c.Assunto = "Reunião de status")
            .With(c => c.DataOcorrencia = new DateTime(2026, 8, 11))
            .With(c => c.HoraInicio = new TimeSpan(9, 0, 0))
            .With(c => c.HoraTermino = new TimeSpan(10, 0, 0))
            .With(c => c.Tipo = TipoCompromisso.Presencial)
            .With(c => c.Local = "Sala 102")
            .With(c => c.Link = null)
            .With(c => c.Contato = null)
            .Build();

        repositorioCompromisso.Cadastrar(compromissoOutraData);
        dbContext.ChangeTracker.Clear();

        Compromisso? compromissoEncontrado = repositorioCompromisso.SelecionarPorId(compromissoOutraData.Id);

        Assert.IsNotNull(compromissoEncontrado);
        Assert.AreEqual("Reunião de status", compromissoEncontrado.Assunto);
        Assert.AreEqual(new DateTime(2026, 8, 11), compromissoEncontrado.DataOcorrencia);
        Assert.AreEqual(new TimeSpan(9, 0, 0), compromissoEncontrado.HoraInicio);
        Assert.AreEqual(new TimeSpan(10, 0, 0), compromissoEncontrado.HoraTermino);
    }
    [TestMethod]
    public void Editar_CompromissoComDadosValidos_DevePersistirAlteracoes()
    {
        Compromisso compromisso = Builder<Compromisso>
            .CreateNew()
            .With(c => c.Assunto = "Reunião de planejamento")
            .With(c => c.DataOcorrencia = new DateTime(2026, 8, 10))
            .With(c => c.HoraInicio = new TimeSpan(9, 0, 0))
            .With(c => c.HoraTermino = new TimeSpan(10, 0, 0))
            .With(c => c.Tipo = TipoCompromisso.Presencial)
            .With(c => c.Local = "Sala 101")
            .With(c => c.Link = null)
            .With(c => c.Contato = null)
            .Build();

        repositorioCompromisso.Cadastrar(compromisso);
        dbContext.ChangeTracker.Clear();

        Compromisso compromissoAtualizado = Builder<Compromisso>
            .CreateNew()
            .With(c => c.Assunto = "Reunião de status")
            .With(c => c.DataOcorrencia = new DateTime(2026, 8, 10))
            .With(c => c.HoraInicio = new TimeSpan(10, 0, 0))
            .With(c => c.HoraTermino = new TimeSpan(11, 0, 0))
            .With(c => c.Tipo = TipoCompromisso.Presencial)
            .With(c => c.Local = "Sala 202")
            .With(c => c.Link = null)
            .With(c => c.Contato = null)
            .Build();

        bool conseguiuEditar = repositorioCompromisso.Editar(compromisso.Id, compromissoAtualizado);
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(conseguiuEditar);

        Compromisso? compromissoEncontrado = repositorioCompromisso.SelecionarPorId(compromisso.Id);

        Assert.IsNotNull(compromissoEncontrado);
        Assert.AreEqual("Reunião de status", compromissoEncontrado.Assunto);
        Assert.AreEqual("Sala 202", compromissoEncontrado.Local);
        Assert.AreEqual(new TimeSpan(10, 0, 0), compromissoEncontrado.HoraInicio);
        Assert.AreEqual(new TimeSpan(11, 0, 0), compromissoEncontrado.HoraTermino);
    }

    [TestMethod]
    public void Editar_CompromissoMantendoHorario_DevePersistirAlteracoes()
    {
        Compromisso compromisso = Builder<Compromisso>
            .CreateNew()
            .With(c => c.Assunto = "Reunião de planejamento")
            .With(c => c.DataOcorrencia = new DateTime(2026, 8, 10))
            .With(c => c.HoraInicio = new TimeSpan(9, 0, 0))
            .With(c => c.HoraTermino = new TimeSpan(10, 0, 0))
            .With(c => c.Tipo = TipoCompromisso.Presencial)
            .With(c => c.Local = "Sala 101")
            .With(c => c.Link = null)
            .With(c => c.Contato = null)
            .Build();

        repositorioCompromisso.Cadastrar(compromisso);
        dbContext.ChangeTracker.Clear();

        Compromisso compromissoAtualizado = Builder<Compromisso>
            .CreateNew()
            .With(c => c.Assunto = "Reunião de status")
            .With(c => c.DataOcorrencia = new DateTime(2026, 8, 10))
            .With(c => c.HoraInicio = new TimeSpan(9, 0, 0))   // mesmo horário
            .With(c => c.HoraTermino = new TimeSpan(10, 0, 0)) // mesmo horário
            .With(c => c.Tipo = TipoCompromisso.Presencial)
            .With(c => c.Local = "Sala 202")
            .With(c => c.Link = null)
            .With(c => c.Contato = null)
            .Build();

        bool conseguiuEditar = repositorioCompromisso.Editar(compromisso.Id, compromissoAtualizado);
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(conseguiuEditar);

        Compromisso? compromissoEncontrado = repositorioCompromisso.SelecionarPorId(compromisso.Id);

        Assert.IsNotNull(compromissoEncontrado);
        Assert.AreEqual("Reunião de status", compromissoEncontrado.Assunto);
        Assert.AreEqual("Sala 202", compromissoEncontrado.Local);
        Assert.AreEqual(new TimeSpan(9, 0, 0), compromissoEncontrado.HoraInicio);
        Assert.AreEqual(new TimeSpan(10, 0, 0), compromissoEncontrado.HoraTermino);
    }

    [TestMethod]
    public void Editar_AlterarTipoDePresencialParaRemoto_DevePersistirComSucesso()
    {
        Compromisso compromisso = Builder<Compromisso>
            .CreateNew()
            .With(c => c.Assunto = "Reunião de planejamento")
            .With(c => c.DataOcorrencia = new DateTime(2026, 8, 10))
            .With(c => c.HoraInicio = new TimeSpan(9, 0, 0))
            .With(c => c.HoraTermino = new TimeSpan(10, 0, 0))
            .With(c => c.Tipo = TipoCompromisso.Presencial)
            .With(c => c.Local = "Sala 101")
            .With(c => c.Link = null)
            .With(c => c.Contato = null)
            .Build();

        repositorioCompromisso.Cadastrar(compromisso);
        dbContext.ChangeTracker.Clear();

        Compromisso compromissoAtualizado = Builder<Compromisso>
            .CreateNew()
            .With(c => c.Assunto = "Reunião de planejamento (remota)")
            .With(c => c.DataOcorrencia = new DateTime(2026, 8, 10))
            .With(c => c.HoraInicio = new TimeSpan(9, 0, 0))
            .With(c => c.HoraTermino = new TimeSpan(10, 0, 0))
            .With(c => c.Tipo = TipoCompromisso.Remoto)
            .With(c => c.Local = null)
            .With(c => c.Link = "https://meet.empresa.com/reuniao123")
            .With(c => c.Contato = null)
            .Build();

        bool conseguiuEditar = repositorioCompromisso.Editar(compromisso.Id, compromissoAtualizado);
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(conseguiuEditar);

        Compromisso? compromissoEncontrado = repositorioCompromisso.SelecionarPorId(compromisso.Id);

        Assert.IsNotNull(compromissoEncontrado);
        Assert.AreEqual(TipoCompromisso.Remoto, compromissoEncontrado.Tipo);
        Assert.AreEqual("https://meet.empresa.com/reuniao123", compromissoEncontrado.Link);
        Assert.IsNull(compromissoEncontrado.Local);
        Assert.AreEqual("Reunião de planejamento (remota)", compromissoEncontrado.Assunto);
    }

    [TestMethod]
    public void Visualizar_CompromissoCadastrado_DeveRetornarDadosCorretos()
    {
        Compromisso compromisso = Builder<Compromisso>
            .CreateNew()
            .With(c => c.Assunto = "Reunião de planejamento")
            .With(c => c.DataOcorrencia = new DateTime(2026, 8, 10))
            .With(c => c.HoraInicio = new TimeSpan(9, 0, 0))
            .With(c => c.HoraTermino = new TimeSpan(10, 0, 0))
            .With(c => c.Tipo = TipoCompromisso.Presencial)
            .With(c => c.Local = "Sala 101")
            .With(c => c.Link = null)
            .With(c => c.Contato = null)
            .Build();

        repositorioCompromisso.Cadastrar(compromisso);
        dbContext.ChangeTracker.Clear();

        Compromisso? compromissoEncontrado = repositorioCompromisso.SelecionarPorId(compromisso.Id);

        Assert.IsNotNull(compromissoEncontrado);
        Assert.AreEqual("Reunião de planejamento", compromissoEncontrado.Assunto);
        Assert.AreEqual(new DateTime(2026, 8, 10), compromissoEncontrado.DataOcorrencia);
        Assert.AreEqual(new TimeSpan(9, 0, 0), compromissoEncontrado.HoraInicio);
        Assert.AreEqual(new TimeSpan(10, 0, 0), compromissoEncontrado.HoraTermino);
        Assert.AreEqual(TipoCompromisso.Presencial, compromissoEncontrado.Tipo);
        Assert.AreEqual("Sala 101", compromissoEncontrado.Local);
    }

    [TestMethod]
    public void Excluir_CompromissoCadastrado_DeveRemoverDoBanco()
    {
        Compromisso compromisso = Builder<Compromisso>
            .CreateNew()
            .With(c => c.Assunto = "Reunião de planejamento")
            .With(c => c.DataOcorrencia = new DateTime(2026, 8, 10))
            .With(c => c.HoraInicio = new TimeSpan(9, 0, 0))
            .With(c => c.HoraTermino = new TimeSpan(10, 0, 0))
            .With(c => c.Tipo = TipoCompromisso.Presencial)
            .With(c => c.Local = "Sala 101")
            .With(c => c.Link = null)
            .With(c => c.Contato = null)
            .Build();

        repositorioCompromisso.Cadastrar(compromisso);
        dbContext.ChangeTracker.Clear();

        bool conseguiuExcluir = repositorioCompromisso.Excluir(compromisso.Id);
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(conseguiuExcluir);

        Compromisso? compromissoEncontrado = repositorioCompromisso.SelecionarPorId(compromisso.Id);
        Assert.IsNull(compromissoEncontrado);
    }
}