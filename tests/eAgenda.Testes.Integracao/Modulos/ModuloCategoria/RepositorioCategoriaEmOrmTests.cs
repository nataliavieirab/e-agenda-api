using eAgenda.Dominio.Modulos.ModuloCategoria;
using eAgenda.Dominio.Modulos.ModuloDespesa;
using eAgenda.Testes.Integracao.Compartilhado.Orm;
using FizzWare.NBuilder;

namespace eAgenda.Testes.Integracao.Modulos.ModuloCategoria;

[TestClass]
public class RepositorioCategoriaEmOrmTests : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public void Cadastrar_ComTodosOsCampos_RegistraCategoria()
    {
        Categoria categoria = Builder<Categoria>
            .CreateNew()
            .With(c => c.Titulo = "Alimentação")
            .Build();

        repositorioCategoria.Cadastrar(categoria);

        dbContext.ChangeTracker.Clear();

        Categoria? categoriaSelecionada = repositorioCategoria.SelecionarPorId(categoria.Id);

        Assert.IsNotNull(categoriaSelecionada);
        Assert.AreEqual("Alimentação", categoriaSelecionada.Titulo);
    }

    [TestMethod]
    public void Cadastrar_ComTituloDuplicado_DeveRegistrar_Ou_RetornarEstadoConsistente()
    {
        Builder<Categoria>
            .CreateNew()
            .With(c => c.Titulo = "Lazer")
            .Persist();

        Categoria categoriaDuplicada = Builder<Categoria>
            .CreateNew()
            .With(c => c.Titulo = "Lazer")
            .Build();

        repositorioCategoria.Cadastrar(categoriaDuplicada);
        dbContext.ChangeTracker.Clear();

        List<Categoria> categorias = repositorioCategoria.Filtrar(c => c.Titulo == "Lazer");

        Assert.IsGreaterThanOrEqualTo(1, categorias.Count);
    }

    [TestMethod]
    public void Editar_ComDadosValidos_AtualizaCategoria()
    {
        Categoria categoria = Builder<Categoria>
            .CreateNew()
            .With(c => c.Titulo = "Saúde")
            .Persist();

        Categoria categoriaAtualizada = Builder<Categoria>
            .CreateNew()
            .With(c => c.Titulo = "Saúde e Bem-estar")
            .Build();

        bool conseguiuEditar = repositorioCategoria.Editar(categoria.Id, categoriaAtualizada);
        dbContext.ChangeTracker.Clear();

        Categoria? categoriaSelecionada = repositorioCategoria.SelecionarPorId(categoria.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.IsNotNull(categoriaSelecionada);
        Assert.AreEqual("Saúde e Bem-estar", categoriaSelecionada.Titulo);
    }

    [TestMethod]
    public void Excluir_SemDespesasVinculadas_RemoveCategoria()
    {
        Categoria categoria = Builder<Categoria>
            .CreateNew()
            .With(c => c.Titulo = "Viagem")
            .Persist();

        bool conseguiuExcluir = repositorioCategoria.Excluir(categoria.Id);
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(repositorioCategoria.SelecionarPorId(categoria.Id));
    }

    [TestMethod]
    public void SelecionarPorId_RetornaCategoria()
    {
        Categoria categoria = Builder<Categoria>
            .CreateNew()
            .With(c => c.Titulo = "Educação")
            .Persist();

        dbContext.ChangeTracker.Clear();

        Categoria? categoriaSelecionada = repositorioCategoria.SelecionarPorId(categoria.Id);

        Assert.IsNotNull(categoriaSelecionada);
        Assert.AreEqual(categoria.Id, categoriaSelecionada.Id);
    }

    [TestMethod]
    public void SelecionarPorId_ComDespesasVinculadas_RetornaCategoriaComDespesas()
    {
        Categoria categoria = Builder<Categoria>
            .CreateNew()
            .With(c => c.Titulo = "Lazer")
            .Persist();

        Despesa despesa1 = new Despesa(
            "Cinema",
            DateTime.Today,
            50.00m,
            FormaPagamento.Credito,
            new List<Categoria> { categoria }
        );

        Despesa despesa2 = new Despesa(
            "Show",
            DateTime.Today,
            120.00m,
            FormaPagamento.Debito,
            new List<Categoria> { categoria }
        );

        repositorioDespesa.Cadastrar(despesa1);
        repositorioDespesa.Cadastrar(despesa2);

        dbContext.ChangeTracker.Clear();

        Categoria? categoriaSelecionada = repositorioCategoria.SelecionarPorId(categoria.Id);

        Assert.IsNotNull(categoriaSelecionada);
        Assert.AreEqual(categoria.Id, categoriaSelecionada.Id);
        Assert.HasCount(2, categoriaSelecionada.Despesas);
        Assert.IsTrue(categoriaSelecionada.Despesas.Any(d => d.Descricao == "Cinema"));
        Assert.IsTrue(categoriaSelecionada.Despesas.Any(d => d.Descricao == "Show"));
    }

    [TestMethod]
    public void SelecionarTodos_RetornaTodasAsCategorias()
    {
        Builder<Categoria>
            .CreateListOfSize(3)
            .All()
            .With(c => c.Titulo = "Categoria " + Guid.NewGuid())
            .Persist();

        dbContext.ChangeTracker.Clear();

        List<Categoria> categorias = repositorioCategoria.SelecionarTodos();

        Assert.HasCount(3, categorias);
    }
}
