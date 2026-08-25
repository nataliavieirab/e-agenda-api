using eAgenda.Dominio.Modulos.ModuloCategoria;
using eAgenda.Dominio.Modulos.ModuloDespesa;

namespace eAgenda.Testes.Unidade.Modulos.ModuloDespesa;

[TestClass]
public sealed class DespesaTests
{
    [TestMethod]
    public void ValidarDespesa_DadosValidos_ComUmaCategoria()
    {
        List<Categoria> categorias = new List<Categoria>
        {
            new Categoria { Titulo = "Transporte" }
        };

        Despesa despesa = new Despesa(
            "Uber",
            new DateTime(2026, 12, 8),
            10.00m,
            FormaPagamento.Credito,
            categorias
        );

        List<string> erros = despesa.Validar();

        Assert.HasCount(0, erros);
        Assert.HasCount(1, despesa.Categorias);
    }

    [TestMethod]
    public void ValidarDespesa_DadosValidos_ComDuasCategoria()
    {
        List<Categoria> categorias = new List<Categoria>
        {
            new Categoria { Titulo = "Transporte" },
            new Categoria { Titulo = "Almoço" },
        };

        Despesa despesa = new Despesa(
            "Uber",
            new DateTime(2026, 12, 8),
            10.00m,
            FormaPagamento.Credito,
            categorias
        );

        List<string> erros = despesa.Validar();

        Assert.HasCount(0, erros);
        Assert.HasCount(2, despesa.Categorias);
    }

    [TestMethod]
    public void CadastrarDespesa_SemDataOcorrencia_DeveAssumirDataDeCadastro()
    {
        List<Categoria> categorias = new List<Categoria>
        {
            new Categoria { Titulo = "Transporte" }
        };

        Despesa despesa = new Despesa(
            "Uber",
            default,
            25.00m,
            FormaPagamento.Credito,
            categorias
        );

        List<string> erros = despesa.Validar();

        Assert.HasCount(0, erros);
        Assert.AreEqual(DateTime.Today, despesa.DataOcorrencia);
    }

    [TestMethod]
    public void Validar_DescricaoEmBranco_DeveApresentarErro()
    {
        List<Categoria> categorias = new List<Categoria>
        {
            new Categoria { Titulo = "Transporte" }
        };

        Despesa despesa = new Despesa(
            string.Empty,
            new DateTime(2026, 12, 8),
            10.00m,
            FormaPagamento.Credito,
            categorias
        );

        List<string> erros = despesa.Validar();

        Assert.HasCount(1, erros);
        Assert.HasCount(1, despesa.Categorias);
        Assert.AreEqual(
            "O campo \"Descrição\" deve conter entre 2 e 100 caracteres.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_FormaPagamentoInvalida_DeveApresentarErro()
    {
        List<Categoria> categorias = new List<Categoria>
        {
            new Categoria { Titulo = "Transporte" }
        };

        Despesa despesa = new Despesa(
            "Táxi",
            new DateTime(2026, 12, 8),
            10.00m,
            (FormaPagamento)99,
            categorias
        );

        List<string> erros = despesa.Validar();

        Assert.HasCount(1, erros);
        Assert.HasCount(1, despesa.Categorias);
        Assert.AreEqual(
            "O campo \"Forma de Pagamento\" deve ser preenchido.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_CategoriaEmBranco_DeveApresentarErro()
    {
        Despesa despesa = new Despesa(
            "Táxi",
            new DateTime(2026, 12, 8),
            10.00m,
            FormaPagamento.AVista,
            []
        );

        // Act
        List<string> erros = despesa.Validar();

        // Assert
        Assert.HasCount(1, erros);
        Assert.HasCount(0, despesa.Categorias);
        Assert.AreEqual(
            "Selecione ao menos uma categoria.",
            erros.First()
        );
    }

    [TestMethod]
    public void ValidarDespesa_Descricao_AbaixoDeUmCaractere()
    {
        List<Categoria> categorias = new List<Categoria>
        {
            new Categoria { Titulo = "Transporte" }
        };

        Despesa despesa = new Despesa(
            new string('A', 1),
            new DateTime(2026, 12, 8),
            10.00m,
            FormaPagamento.Credito,
            categorias
        );

        // Act
        List<string> erros = despesa.Validar();

        // Assert
        Assert.HasCount(1, erros);
        Assert.HasCount(1, despesa.Categorias);
        Assert.AreEqual(
            "O campo \"Descrição\" deve conter entre 2 e 100 caracteres.",
         erros.First()
        );
    }

    [TestMethod]
    public void ValidarDespesa_Descricao_LimiteDeCaractere()
    {
        List<Categoria> categorias = new List<Categoria>
        {
            new Categoria { Titulo = "Transporte" }
        };

        Despesa despesa = new Despesa(
            new string('A', 100),
            new DateTime(2026, 12, 8),
            10.00m,
            FormaPagamento.Credito,
            categorias
        );

        List<string> erros = despesa.Validar();

        Assert.HasCount(0, erros);
        Assert.HasCount(1, despesa.Categorias);
    }

    [TestMethod]
    public void ValidarDespesa_Descricao_AcimaDoMaximoDeCaractere()
    {
        List<Categoria> categorias = new List<Categoria>
        {
            new Categoria { Titulo = "Transporte" }
        };

        Despesa despesa = new Despesa(
            new string('A', 101),
            new DateTime(2026, 12, 8),
            10.00m,
            FormaPagamento.Credito,
            categorias
        );

        List<string> erros = despesa.Validar();

        Assert.HasCount(1, erros);
        Assert.HasCount(1, despesa.Categorias);
        Assert.AreEqual(
            "O campo \"Descrição\" deve conter entre 2 e 100 caracteres.",
         erros.First()
        );
    }

    [TestMethod]
    public void ValidarDespesa_ValorZero_DeveApresentarErro()
    {
        List<Categoria> categorias = new List<Categoria>
        {
            new Categoria { Titulo = "Transporte" }
        };

        Despesa despesa = new Despesa(
            "Táxi",
            new DateTime(2026, 12, 8),
            0,
            FormaPagamento.Credito,
            categorias
        );

        List<string> erros = despesa.Validar();

        Assert.HasCount(1, erros);
        Assert.HasCount(1, despesa.Categorias);
        Assert.AreEqual(
            "O campo \"Valor\" deve ser maior que zero.",
            erros.First()
        );
    }

    [TestMethod]
    public void ValidarDespesa_ValorNegativo_DeveApresentarErro()
    {
        List<Categoria> categorias = new List<Categoria>
        {
            new Categoria { Titulo = "Transporte" }
        };

        Despesa despesa = new Despesa(
            "Táxi",
            new DateTime(2026, 12, 8),
            -10,
            FormaPagamento.Credito,
            categorias
        );

        List<string> erros = despesa.Validar();

        Assert.HasCount(1, erros);
        Assert.HasCount(1, despesa.Categorias);
        Assert.AreEqual(
            "O campo \"Valor\" deve ser maior que zero.",
            erros.First()
        );
    }

    [TestMethod]
    public void ValidarDespesa_ValorDecimal_DeveApresentarErro()
    {
        List<Categoria> categorias = new List<Categoria>
        {
            new Categoria { Titulo = "Transporte" }
        };

        Despesa despesa = new Despesa(
            "Táxi",
            new DateTime(2026, 12, 8),
            10.00m,
            FormaPagamento.Credito,
            categorias
        );

        List<string> erros = despesa.Validar();

        Assert.HasCount(0, erros);
        Assert.HasCount(1, despesa.Categorias);

    }

    [TestMethod]
    public void ValidarDespesa_SemCategoria_DeveApresentarErro()
    {
        List<Categoria> categorias = new List<Categoria>();

        Despesa despesa = new Despesa(
            "Táxi",
            new DateTime(2026, 12, 8),
            10.00m,
            FormaPagamento.Credito,
            categorias
        );

        List<string> erros = despesa.Validar();

        Assert.HasCount(1, erros);
        Assert.HasCount(0, despesa.Categorias);
        Assert.AreEqual(
            "Selecione ao menos uma categoria.",
            erros.First()
        );
    }

    [TestMethod]
    public void Atualizar_DeveAtualizar_AlteraValorCategoria()
    {
        List<Categoria> categoriasOriginais = new List<Categoria>
        {
            new Categoria { Titulo = "Transporte" }
        };

        List<Categoria> categoriasNovas = new List<Categoria>
        {
            new Categoria { Titulo = "Reunião" }
        };

        Despesa despesa = new Despesa(
            "Táxi",
            new DateTime(2026, 12, 8),
            10,
            FormaPagamento.Credito,
            categoriasOriginais
        );

        Despesa despesaAtualizada = new Despesa(
            "Táxi",
            new DateTime(2026, 12, 8),
            50,
            FormaPagamento.Credito,
            categoriasNovas
        );

        despesa.Atualizar(despesaAtualizada);

        Assert.AreEqual("Táxi", despesa.Descricao);
        Assert.AreEqual(new DateTime(2026, 12, 8), despesa.DataOcorrencia);
        Assert.AreEqual(50, despesa.Valor);
        Assert.AreEqual(FormaPagamento.Credito, despesa.FormaPagamento);
        Assert.AreEqual(categoriasNovas, despesa.Categorias);

    }

    [TestMethod]
    public void Atualizar_DeveRetornarErro_QuandoSemCategoria()
    {
        List<Categoria> categoriasOriginais = new List<Categoria>
    {
        new Categoria { Titulo = "Transporte" }
    };

        List<Categoria> categoriasNovas = new List<Categoria>();

        Despesa despesa = new Despesa(
            "Táxi",
            new DateTime(2026, 12, 8),
            10,
            FormaPagamento.Credito,
            categoriasOriginais
        );

        Despesa despesaAtualizada = new Despesa(
            "Táxi",
            new DateTime(2026, 12, 8),
            50,
            FormaPagamento.Credito,
            categoriasNovas
        );

        despesa.Atualizar(despesaAtualizada);
        List<string> erros = despesa.Validar();

        Assert.AreEqual("Táxi", despesa.Descricao);
        Assert.AreEqual(new DateTime(2026, 12, 8), despesa.DataOcorrencia);
        Assert.AreEqual(50, despesa.Valor);
        Assert.AreEqual(FormaPagamento.Credito, despesa.FormaPagamento);

        Assert.IsEmpty(despesa.Categorias);

        Assert.HasCount(1, erros);
        Assert.AreEqual("Selecione ao menos uma categoria.", erros.First());
    }
}
