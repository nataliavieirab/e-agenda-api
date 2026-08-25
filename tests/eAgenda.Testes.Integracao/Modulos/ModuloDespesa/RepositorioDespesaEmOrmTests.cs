using eAgenda.Dominio.Modulos.ModuloCategoria;
using eAgenda.Dominio.Modulos.ModuloDespesa;
using eAgenda.Testes.Integracao.Compartilhado.Orm;
using FizzWare.NBuilder;

namespace eAgenda.Testes.Integracao.Modulos.ModuloDespesa;

[TestClass]
public class RepositorioDespesaEmOrmTests : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public void Cadastrar_DadosValidosEUmaCategoria_DevePersistirDespesaComSucesso()
    {
        Despesa despesa = Builder<Despesa>
            .CreateNew()
            .With(d => d.Descricao = "Docinhos festa")
            .With(d => d.DataOcorrencia = new DateTime(2024, 8, 11))
            .With(d => d.Valor = 200.00m)
            .With(d => d.FormaPagamento = FormaPagamento.AVista)
            .With(d => d.Categorias = new List<Categoria> { new Categoria("Aniversário") })
            .Build();

        repositorioDespesa.Cadastrar(despesa);
        dbContext.ChangeTracker.Clear();

        Despesa? despesaSelecionada = repositorioDespesa.SelecionarPorId(despesa.Id);

        Assert.IsNotNull(despesaSelecionada);
        Assert.AreEqual("Docinhos festa", despesaSelecionada.Descricao);
        Assert.AreEqual(new DateTime(2024, 8, 11), despesaSelecionada.DataOcorrencia);
        Assert.AreEqual(200.00m, despesaSelecionada.Valor);
        Assert.AreEqual(FormaPagamento.AVista, despesaSelecionada.FormaPagamento);

        Assert.IsNotNull(despesaSelecionada.Categorias);
        Assert.HasCount(1, despesaSelecionada.Categorias);
        Assert.AreEqual("Aniversário", despesaSelecionada.Categorias[0].Titulo);
    }

    [TestMethod]
    public void Cadastrar_DadosValidosEMultiplasCategorias_DevePersistirDespesaComSucesso()
    {
        Despesa despesa = Builder<Despesa>
            .CreateNew()
            .With(d => d.Descricao = "Docinhos festa")
            .With(d => d.DataOcorrencia = new DateTime(2024, 8, 11))
            .With(d => d.Valor = 200.00m)
            .With(d => d.FormaPagamento = FormaPagamento.AVista)
            .With(d => d.Categorias = new List<Categoria>
            {
            new Categoria("Aniversário"),
            new Categoria("Alimentação")
            })
            .Build();

        repositorioDespesa.Cadastrar(despesa);
        dbContext.ChangeTracker.Clear();

        Despesa? despesaSelecionada = repositorioDespesa.SelecionarPorId(despesa.Id);

        Assert.IsNotNull(despesaSelecionada);
        Assert.AreEqual("Docinhos festa", despesaSelecionada.Descricao);
        Assert.AreEqual(new DateTime(2024, 8, 11), despesaSelecionada.DataOcorrencia);
        Assert.AreEqual(200.00m, despesaSelecionada.Valor);
        Assert.AreEqual(FormaPagamento.AVista, despesaSelecionada.FormaPagamento);

        Assert.IsNotNull(despesaSelecionada.Categorias);
        Assert.HasCount(2, despesaSelecionada.Categorias);
        Assert.AreEqual("Alimentação", despesaSelecionada.Categorias[0].Titulo);
        Assert.AreEqual("Aniversário", despesaSelecionada.Categorias[1].Titulo);
    }

    [TestMethod]
    public void Cadastrar_DadosValidosValorDecimal_DevePersistirDespesaComSucesso()
    {
        Despesa despesa = Builder<Despesa>
            .CreateNew()
            .With(d => d.Descricao = "Brigadeiro")
            .With(d => d.DataOcorrencia = new DateTime(2024, 8, 11))
            .With(d => d.Valor = 20.00m)
            .With(d => d.FormaPagamento = FormaPagamento.AVista)
            .With(d => d.Categorias = new List<Categoria> { new Categoria("Lanche") })
            .Build();

        repositorioDespesa.Cadastrar(despesa);
        dbContext.ChangeTracker.Clear();

        Despesa? despesaSelecionada = repositorioDespesa.SelecionarPorId(despesa.Id);

        Assert.IsNotNull(despesaSelecionada);
        Assert.AreEqual("Brigadeiro", despesaSelecionada.Descricao);
        Assert.AreEqual(new DateTime(2024, 8, 11), despesaSelecionada.DataOcorrencia);
        Assert.AreEqual(20.00m, despesaSelecionada.Valor);
        Assert.AreEqual(FormaPagamento.AVista, despesaSelecionada.FormaPagamento);

        Assert.IsNotNull(despesaSelecionada.Categorias);
        Assert.HasCount(1, despesaSelecionada.Categorias);
        Assert.AreEqual("Lanche", despesaSelecionada.Categorias[0].Titulo);
    }

    [TestMethod]
    public void Cadastrar_SemDataOcorrencia_ComUmaCategoria_DevePersistirDespesaComSucesso()
    {
        Despesa despesa = Builder<Despesa>
            .CreateNew()
            .With(d => d.Descricao = "Docinhos festa")
            .With(d => d.Valor = 150.00m)
            .With(d => d.FormaPagamento = FormaPagamento.AVista)
            .With(d => d.Categorias = new List<Categoria> { new Categoria("Aniversário") })
            .Build();

        repositorioDespesa.Cadastrar(despesa);
        dbContext.ChangeTracker.Clear();

        Despesa? despesaSelecionada = repositorioDespesa.SelecionarPorId(despesa.Id);

        Assert.IsNotNull(despesaSelecionada);
        Assert.AreEqual("Docinhos festa", despesaSelecionada.Descricao);
        Assert.AreEqual(150.00m, despesaSelecionada.Valor);
        Assert.AreEqual(FormaPagamento.AVista, despesaSelecionada.FormaPagamento);

        Assert.AreEqual(DateTime.Today, despesaSelecionada.DataOcorrencia);

        Assert.IsNotNull(despesaSelecionada.Categorias);
        Assert.HasCount(1, despesaSelecionada.Categorias);
        Assert.AreEqual("Aniversário", despesaSelecionada.Categorias[0].Titulo);
    }

    [TestMethod]
    public void Editar_AlteraValorECategoria_DevePersistirAlteracoes()
    {
        Despesa despesa = Builder<Despesa>
            .CreateNew()
            .With(d => d.Descricao = "Brigadeiro")
            .With(d => d.DataOcorrencia = new DateTime(2024, 8, 11))
            .With(d => d.Valor = 20.00m)
            .With(d => d.FormaPagamento = FormaPagamento.AVista)
            .With(d => d.Categorias = new List<Categoria> { new Categoria("Lanche") })
            .Build();

        repositorioDespesa.Cadastrar(despesa);
        dbContext.ChangeTracker.Clear();

        Despesa despesaAtualizada = Builder<Despesa>
            .CreateNew()
            .With(d => d.Descricao = "Brigadeiro")
            .With(d => d.DataOcorrencia = new DateTime(2024, 8, 11))
            .With(d => d.Valor = 200.00m)
            .With(d => d.FormaPagamento = FormaPagamento.AVista)
            .With(d => d.Categorias = new List<Categoria> { new Categoria("Lanche"),
            new Categoria("Festa") })
            .Build();

        bool conseguiuEditar = repositorioDespesa.Editar(despesa.Id, despesaAtualizada);
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(conseguiuEditar);

        Despesa? despesaSelecionada = repositorioDespesa.SelecionarPorId(despesa.Id);

        Assert.IsNotNull(despesaSelecionada);
        Assert.AreEqual("Brigadeiro", despesaSelecionada.Descricao);
        Assert.AreEqual(new DateTime(2024, 8, 11), despesaSelecionada.DataOcorrencia);
        Assert.AreEqual(200.00m, despesaSelecionada.Valor);
        Assert.AreEqual(FormaPagamento.AVista, despesaSelecionada.FormaPagamento);
        Assert.AreEqual("Festa", despesaSelecionada.Categorias[0].Titulo);
    }

    [TestMethod]
    public void Visualizar_DadosDeUmaDespesa_DeveRetornarComSucesso()
    {
        Despesa despesa = Builder<Despesa>
            .CreateNew()
            .With(d => d.Descricao = "Pizza")
            .With(d => d.DataOcorrencia = new DateTime(2024, 8, 11))
            .With(d => d.Valor = 80.00m)
            .With(d => d.FormaPagamento = FormaPagamento.Credito)
            .With(d => d.Categorias = new List<Categoria> { new Categoria("Alimentação") })
            .Build();

        repositorioDespesa.Cadastrar(despesa);
        dbContext.ChangeTracker.Clear();

        Despesa? despesaSelecionada = repositorioDespesa.SelecionarPorId(despesa.Id);

        Assert.IsNotNull(despesaSelecionada);
        Assert.AreEqual("Pizza", despesaSelecionada.Descricao);
        Assert.AreEqual(new DateTime(2024, 8, 11), despesaSelecionada.DataOcorrencia);
        Assert.AreEqual(80.00m, despesaSelecionada.Valor);
        Assert.AreEqual(FormaPagamento.Credito, despesaSelecionada.FormaPagamento);

        Assert.IsNotNull(despesaSelecionada.Categorias);
        Assert.HasCount(1, despesaSelecionada.Categorias);
        Assert.AreEqual("Alimentação", despesaSelecionada.Categorias[0].Titulo);
    }

    [TestMethod]
    public void Listar_TodasAsDespesas_DeveRetornarAoMenosDuas()
    {
        Despesa despesa1 = Builder<Despesa>
            .CreateNew()
            .With(d => d.Descricao = "Pizza")
            .With(d => d.DataOcorrencia = new DateTime(2024, 8, 11))
            .With(d => d.Valor = 80.00m)
            .With(d => d.FormaPagamento = FormaPagamento.Credito)
            .With(d => d.Categorias = new List<Categoria> { new Categoria("Alimentação") })
            .Build();

        Despesa despesa2 = Builder<Despesa>
            .CreateNew()
            .With(d => d.Descricao = "Docinhos festa")
            .With(d => d.DataOcorrencia = new DateTime(2024, 8, 12))
            .With(d => d.Valor = 200.00m)
            .With(d => d.FormaPagamento = FormaPagamento.AVista)
            .With(d => d.Categorias = new List<Categoria> { new Categoria("Aniversário") })
            .Build();

        repositorioDespesa.Cadastrar(despesa1);
        repositorioDespesa.Cadastrar(despesa2);
        dbContext.ChangeTracker.Clear();

        List<Despesa> despesas = repositorioDespesa.SelecionarTodos();

        Assert.IsNotNull(despesas);
        Assert.IsTrue(despesas.Count >= 2);

        Assert.IsTrue(despesas.Any(d => d.Descricao == "Pizza" && d.Valor == 80.00m));
        Assert.IsTrue(despesas.Any(d => d.Descricao == "Docinhos festa" && d.Valor == 200.00m));
    }

    [TestMethod]
    public void Excluir_DespesaCadastrada_DeveRemoverComSucesso()
    {
        Despesa despesa = Builder<Despesa>
            .CreateNew()
            .With(d => d.Descricao = "Pizza")
            .With(d => d.DataOcorrencia = new DateTime(2024, 8, 11))
            .With(d => d.Valor = 80.00m)
            .With(d => d.FormaPagamento = FormaPagamento.Credito)
            .With(d => d.Categorias = new List<Categoria> { new Categoria("Alimentação") })
            .Build();

        repositorioDespesa.Cadastrar(despesa);
        dbContext.ChangeTracker.Clear();

        bool conseguiuExcluir = repositorioDespesa.Excluir(despesa.Id);
        dbContext.ChangeTracker.Clear();

        Assert.IsTrue(conseguiuExcluir);

        Despesa? despesaSelecionada = repositorioDespesa.SelecionarPorId(despesa.Id);
        Assert.IsNull(despesaSelecionada);
    }
}