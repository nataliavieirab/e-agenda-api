using eAgenda.Dominio.Modulos.ModuloContato;
using eAgenda.Testes.Integracao.Compartilhado.Orm;
using FizzWare.NBuilder;
namespace eAgenda.Testes.Integracao.Modulos.ModuloContato;

[TestClass]
public class RepositorioContatoEmOrmTests : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public void Cadastrar_ComTodosOsCampos_DevePersistirContato()
    {
        Contato contato = Builder<Contato>
            .CreateNew()
            .With(c => c.Nome = "Natalia Bortoli Vieira")
            .With(c => c.Email = "nbv@email.com")
            .With(c => c.Telefone = "(48) 99999-0000")
            .With(c => c.Cargo = "Cargo X")
            .With(c => c.Empresa = "Empresa X")
            .Build();

        repositorioContato.Cadastrar(contato);

        dbContext.ChangeTracker.Clear();

        Contato? contatoSelecionado = repositorioContato.SelecionarPorId(contato.Id);

        Assert.IsNotNull(contatoSelecionado);
        Assert.AreEqual("Natalia Bortoli Vieira", contatoSelecionado.Nome);
        Assert.AreEqual("nbv@email.com", contatoSelecionado.Email);
        Assert.AreEqual("(48) 99999-0000", contatoSelecionado.Telefone);
        Assert.AreEqual("Cargo X", contatoSelecionado.Cargo);
        Assert.AreEqual("Empresa X", contatoSelecionado.Empresa);
    }

    [TestMethod]
    public void Cadastrar_ApenasCamposObrigatorios_PersisteContato()
    {
        Contato contato = Builder<Contato>
            .CreateNew()
            .With(c => c.Nome = "Natalia Bortoli Vieira")
            .With(c => c.Email = "nbv@email.com")
            .With(c => c.Telefone = "(48) 99999-0000")
            .With(c => c.Cargo = null)
            .With(c => c.Empresa = null)
            .Build();

        repositorioContato.Cadastrar(contato);

        dbContext.ChangeTracker.Clear();

        Contato? contatoSelecionado = repositorioContato.SelecionarPorId(contato.Id);

        Assert.IsNotNull(contatoSelecionado);
        Assert.AreEqual("Natalia Bortoli Vieira", contatoSelecionado.Nome);
        Assert.AreEqual("nbv@email.com", contatoSelecionado.Email);
        Assert.AreEqual("(48) 99999-0000", contatoSelecionado.Telefone);
        Assert.IsNull(contatoSelecionado.Cargo);
        Assert.IsNull(contatoSelecionado.Empresa);
    }

    [TestMethod]
    public void Cadastrar_ComEmailDuplicado_DevePersistir_Ou_RetornarEstadoConsistente()
    {
        Builder<Contato>
            .CreateNew()
            .With(c => c.Email = "nbv@email.com")
            .With(c => c.Nome = "Natalia Bortoli Vieira")
            .With(c => c.Telefone = "(48) 99999-0000")
            .Persist();

        Contato contatoComMesmoEmail = Builder<Contato>
            .CreateNew()
            .With(c => c.Email = "nbv@email.com")
            .With(c => c.Nome = "Natalia Vieira")
            .With(c => c.Telefone = "(48) 99999-0001")
            .Build();

        repositorioContato.Cadastrar(contatoComMesmoEmail);

        dbContext.ChangeTracker.Clear();

        List<Contato> contatosNoBanco = repositorioContato.Filtrar(c => c.Email == "nbv@email.com");

        Assert.IsGreaterThanOrEqualTo(1, contatosNoBanco.Count);
    }

    [TestMethod]
    public void Cadastrar_ComTelefoneDuplicado_DevePersistir_Ou_RetornarEstadoConsistente()
    {
        Builder<Contato>
            .CreateNew()
            .With(c => c.Email = "nbv@email.com")
            .With(c => c.Nome = "Natalia Bortoli Vieira")
            .With(c => c.Telefone = "(48) 99999-0000")
            .Persist();

        Contato contatoComMesmoEmail = Builder<Contato>
            .CreateNew()
            .With(c => c.Email = "nataliabv@email.com")
            .With(c => c.Nome = "Natalia Vieira")
            .With(c => c.Telefone = "(48) 99999-0000")
            .Build();

        repositorioContato.Cadastrar(contatoComMesmoEmail);
        dbContext.ChangeTracker.Clear();

        List<Contato> contatosNoBanco = repositorioContato.Filtrar(c => c.Telefone == "(48) 99999-0000");

        Assert.IsGreaterThanOrEqualTo(1, contatosNoBanco.Count);

    }

    [TestMethod]
    public void Editar_ComDadosValidos_AtualizaContato()
    {
        Contato contato = Builder<Contato>
            .CreateNew()
            .Persist();

        Contato contatoAtualizado = Builder<Contato>
            .CreateNew()
            .With(c => c.Nome = "Novo Nome")
            .With(c => c.Email = "novoemail@gmail.com")
            .With(c => c.Telefone = "(48) 99980-1111")
            .Build();

        bool conseguiuEditar = repositorioContato.Editar(contato.Id, contatoAtualizado);

        dbContext.ChangeTracker.Clear();

        Contato? contatoSelecionado = repositorioContato.SelecionarPorId(contato.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.IsNotNull(contatoSelecionado);
        Assert.AreEqual("Novo Nome", contatoSelecionado.Nome);
        Assert.AreEqual("novoemail@gmail.com", contatoSelecionado.Email);
        Assert.AreEqual("(48) 99980-1111", contatoSelecionado.Telefone);
    }

    [TestMethod]
    public void Editar_MantendoEmailETelefone_AtualizaContato()
    {
        Contato contatoCadastrado = Builder<Contato>
            .CreateNew()
            .With(c => c.Nome = "Carlos Oliveira")
            .With(c => c.Email = "carlos@email.com")
            .With(c => c.Telefone = "(47) 98888-1111")
            .Persist();

        Contato novosDados = Builder<Contato>
            .CreateNew()
            .With(c => c.Nome = "Carlos Alberto")
            .With(c => c.Email = "carlos@email.com")
            .With(c => c.Telefone = "(47) 98888-1111")
            .Build();

        bool edicaoRealizada = repositorioContato.Editar(
            contatoCadastrado.Id,
            novosDados
        );

        dbContext.ChangeTracker.Clear();

        Contato? contatoEncontrado = repositorioContato.SelecionarPorId(
            contatoCadastrado.Id
        );

        Assert.IsTrue(edicaoRealizada);
        Assert.IsNotNull(contatoEncontrado);
        Assert.AreEqual("Carlos Alberto", contatoEncontrado.Nome);
        Assert.AreEqual("carlos@email.com", contatoEncontrado.Email);
    }

    [TestMethod]
    public void SelecionarPorId_RetornaContato()
    {
        Contato contato = Builder<Contato>
            .CreateNew()
            .Persist();

        dbContext.ChangeTracker.Clear();

        Contato? contatoEncontrado = repositorioContato.SelecionarPorId(contato.Id);

        Assert.IsNotNull(contatoEncontrado);
        Assert.AreEqual(contato.Id, contatoEncontrado.Id);
    }

    [TestMethod]
    public void SelecionarTodos_RetornaTodosOsContatos()
    {
        Builder<Contato>
            .CreateListOfSize(3)
            .All()
            .Persist();

        dbContext.ChangeTracker.Clear();

        Assert.HasCount(3, repositorioContato.SelecionarTodos());
    }

    [TestMethod]
    public void Excluir_SemCompromissosVinculados_RemoveContato()
    {
        Contato contato = Builder<Contato>
            .CreateNew()
            .Persist();

        bool conseguiuExcluir = repositorioContato.Excluir(contato.Id);

        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(repositorioContato.SelecionarPorId(contato.Id));
    }
}
