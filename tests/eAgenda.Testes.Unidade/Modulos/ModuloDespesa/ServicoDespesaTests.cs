using eAgenda.Aplicacao.Modulos.ModuloDespesa;
using eAgenda.Dominio.Modulos.ModuloCategoria;
using eAgenda.Dominio.Modulos.ModuloDespesa;
using FluentResults;
using Moq;

namespace eAgenda.Testes.Unidade.Modulos.ModuloDespesa;

[TestClass]
public sealed class ServicoDespesaTests
{
    [TestMethod]
    public void Cadastrar_DespesaComDadosValidos()
    {
        Mock<IRepositorioDespesa> repositorioDespesa = new Mock<IRepositorioDespesa>();
        Mock<IRepositorioCategoria> repositorioCategoria = new Mock<IRepositorioCategoria>();

        List<Despesa> despesas = new();

        repositorioDespesa.Setup(r => r.SelecionarTodos()).Returns(() => despesas);

        Despesa? despesaCadastrada = null;

        repositorioDespesa
            .Setup(r => r.Cadastrar(It.IsAny<Despesa>()))
            .Callback<Despesa>(d =>
            {
                despesaCadastrada = d;
                despesas.Add(d);
            });

        Categoria categoria = new Categoria("Limpeza Casa");

        repositorioCategoria
            .Setup(r => r.SelecionarTodos())
            .Returns(new List<Categoria> { categoria });

        ServicoDespesa servicoDespesa = new ServicoDespesa(
            repositorioDespesa.Object,
            repositorioCategoria.Object
        );

        Result resultado = servicoDespesa.Cadastrar(new CadastrarDespesaDto(
            "Almoço de negócios",
            new DateTime(2026, 8, 8),
            120.50m,
            FormaPagamento.Credito,   // use o enum correto
            new List<Guid> { categoria.Id }
        ));

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(despesaCadastrada);
        Assert.AreEqual("Almoço de negócios", despesaCadastrada.Descricao);
        Assert.AreEqual(new DateTime(2026, 8, 8), despesaCadastrada.DataOcorrencia);
        Assert.AreEqual(120.50m, despesaCadastrada.Valor);
        Assert.AreEqual(FormaPagamento.Credito, despesaCadastrada.FormaPagamento);
        Assert.HasCount(1, despesaCadastrada.Categorias);
        Assert.AreEqual(categoria, despesaCadastrada.Categorias[0]);

        repositorioDespesa.Verify(r => r.Cadastrar(It.IsAny<Despesa>()), Times.Once);

        List<ListarDespesasDto> despesasListadas = servicoDespesa.SelecionarTodos();
        Assert.HasCount(1, despesasListadas);
        Assert.AreEqual("Almoço de negócios", despesasListadas[0].Descricao);
        Assert.AreEqual(120.50m, despesasListadas[0].Valor);
        Assert.AreEqual(FormaPagamento.Credito, despesasListadas[0].FormaPagamento);
        Assert.AreEqual("Limpeza Casa", despesasListadas[0].Categorias[0].Titulo);
    }


    [TestMethod]
    public void Cadastrar_DespesaComMultiplasCategorias()
    {
        Mock<IRepositorioDespesa> repositorioDespesa = new Mock<IRepositorioDespesa>();
        Mock<IRepositorioCategoria> repositorioCategoria = new Mock<IRepositorioCategoria>();

        List<Despesa> despesas = new();

        repositorioDespesa.Setup(r => r.SelecionarTodos()).Returns(() => despesas);

        Despesa? despesaCadastrada = null;

        repositorioDespesa
            .Setup(r => r.Cadastrar(It.IsAny<Despesa>()))
            .Callback<Despesa>(d =>
            {
                despesaCadastrada = d;
                despesas.Add(d);
            });

        Categoria categoria1 = new Categoria("Alimentação");
        Categoria categoria2 = new Categoria("Transporte");

        repositorioCategoria
            .Setup(r => r.SelecionarTodos())
            .Returns(new List<Categoria> { categoria1, categoria2 });

        ServicoDespesa servicoDespesa = new ServicoDespesa(
            repositorioDespesa.Object,
            repositorioCategoria.Object
        );

        Result resultado = servicoDespesa.Cadastrar(new CadastrarDespesaDto(
            "Viagem de negócios",
            new DateTime(2026, 8, 9),
            500.00m,
            FormaPagamento.AVista,
            new List<Guid> { categoria1.Id, categoria2.Id }
        ));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(despesaCadastrada);
        Assert.AreEqual("Viagem de negócios", despesaCadastrada.Descricao);
        Assert.AreEqual(new DateTime(2026, 8, 9), despesaCadastrada.DataOcorrencia);
        Assert.AreEqual(500.00m, despesaCadastrada.Valor);
        Assert.AreEqual(FormaPagamento.AVista, despesaCadastrada.FormaPagamento);
        Assert.HasCount(2, despesaCadastrada.Categorias);
        CollectionAssert.Contains(despesaCadastrada.Categorias, categoria1);
        CollectionAssert.Contains(despesaCadastrada.Categorias, categoria2);

        List<ListarDespesasDto> despesasListadas = servicoDespesa.SelecionarTodos();
        Assert.HasCount(1, despesasListadas);
        Assert.AreEqual("Viagem de negócios", despesasListadas[0].Descricao);
        Assert.HasCount(2, despesasListadas[0].Categorias);
        Assert.AreEqual("Alimentação", despesasListadas[0].Categorias[0].Titulo);
        Assert.AreEqual("Transporte", despesasListadas[0].Categorias[1].Titulo);

        repositorioDespesa.Verify(r => r.Cadastrar(It.IsAny<Despesa>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_DespesaSemDescricao_RetornaFalha()
    {
        Mock<IRepositorioDespesa> repositorioDespesa = new Mock<IRepositorioDespesa>();
        Mock<IRepositorioCategoria> repositorioCategoria = new Mock<IRepositorioCategoria>();

        Categoria categoria = new Categoria("Alimentação");
        repositorioCategoria
            .Setup(r => r.SelecionarTodos())
            .Returns(new List<Categoria> { categoria });

        ServicoDespesa servicoDespesa = new ServicoDespesa(
            repositorioDespesa.Object,
            repositorioCategoria.Object
        );

        Result resultado = servicoDespesa.Cadastrar(new CadastrarDespesaDto(
            string.Empty,
            DateTime.Today,
            100m,
            FormaPagamento.AVista,
            new List<Guid> { categoria.Id }
        ));

        Assert.IsFalse(resultado.IsSuccess);
        Assert.AreEqual("O campo \"Descrição\" deve conter entre 2 e 100 caracteres.", resultado.Errors[0].Message);
        repositorioDespesa.Verify(r => r.Cadastrar(It.IsAny<Despesa>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_DespesaSemDataOcorrencia_AssumeDataDeCadastro()
    {
        Mock<IRepositorioDespesa> repositorioDespesa = new Mock<IRepositorioDespesa>();
        Mock<IRepositorioCategoria> repositorioCategoria = new Mock<IRepositorioCategoria>();
        Despesa? despesaCadastrada = null;
        repositorioDespesa
            .Setup(r => r.Cadastrar(It.IsAny<Despesa>()))
            .Callback<Despesa>(d => despesaCadastrada = d);
        Categoria categoria = new Categoria("Transporte");
        repositorioCategoria
            .Setup(r => r.SelecionarTodos())
            .Returns(new List<Categoria> { categoria });
        ServicoDespesa servicoDespesa = new ServicoDespesa(
            repositorioDespesa.Object,
            repositorioCategoria.Object
        );
        Result resultado = servicoDespesa.Cadastrar(new CadastrarDespesaDto(
            "Táxi",
            null,
            50m,
            FormaPagamento.Debito,
            new List<Guid> { categoria.Id }
        ));
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(despesaCadastrada);
        Assert.AreEqual(DateTime.Today, despesaCadastrada.DataOcorrencia);
        repositorioDespesa.Verify(r => r.Cadastrar(It.IsAny<Despesa>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_DespesaSemValor_RetornaErro()
    {
        Mock<IRepositorioDespesa> repositorioDespesa = new Mock<IRepositorioDespesa>();
        Mock<IRepositorioCategoria> repositorioCategoria = new Mock<IRepositorioCategoria>();

        Categoria categoria = new Categoria("Educação");
        repositorioCategoria
            .Setup(r => r.SelecionarTodos())
            .Returns(new List<Categoria> { categoria });

        ServicoDespesa servicoDespesa = new ServicoDespesa(
            repositorioDespesa.Object,
            repositorioCategoria.Object
        );

        Result resultado = servicoDespesa.Cadastrar(new CadastrarDespesaDto(
            "Curso online",
            DateTime.Today,
            0m,
            FormaPagamento.Credito,
            new List<Guid> { categoria.Id }
        ));

        Assert.IsFalse(resultado.IsSuccess);
        Assert.AreEqual("O campo \"Valor\" deve ser maior que zero.", resultado.Errors[0].Message);
        repositorioDespesa.Verify(r => r.Cadastrar(It.IsAny<Despesa>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_DespesaSemFormaPagamento_RetornaErro()
    {
        Mock<IRepositorioDespesa> repositorioDespesa = new Mock<IRepositorioDespesa>();
        Mock<IRepositorioCategoria> repositorioCategoria = new Mock<IRepositorioCategoria>();

        Categoria categoria = new Categoria("Saúde");
        repositorioCategoria
            .Setup(r => r.SelecionarTodos())
            .Returns(new List<Categoria> { categoria });

        ServicoDespesa servicoDespesa = new ServicoDespesa(
            repositorioDespesa.Object,
            repositorioCategoria.Object
        );

        Result resultado = servicoDespesa.Cadastrar(new CadastrarDespesaDto(
            "Consulta médica",
            DateTime.Today,
            200m,
            (FormaPagamento)99,
            new List<Guid> { categoria.Id }
        ));

        Assert.IsFalse(resultado.IsSuccess);
        Assert.AreEqual("O campo \"Forma de Pagamento\" deve ser preenchido.", resultado.Errors[0].Message);
        repositorioDespesa.Verify(r => r.Cadastrar(It.IsAny<Despesa>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_DespesaSemCategorias_RetornaErro()
    {
        Mock<IRepositorioDespesa> repositorioDespesa = new Mock<IRepositorioDespesa>();
        Mock<IRepositorioCategoria> repositorioCategoria = new Mock<IRepositorioCategoria>();

        ServicoDespesa servicoDespesa = new ServicoDespesa(
            repositorioDespesa.Object,
            repositorioCategoria.Object
        );

        Result resultado = servicoDespesa.Cadastrar(new CadastrarDespesaDto(
            "Despesa Transporte",
            DateTime.Today,
            150m,
            FormaPagamento.AVista,
            new List<Guid>()
        ));

        Assert.IsFalse(resultado.IsSuccess);
        Assert.AreEqual("Selecione ao menos uma categoria.", resultado.Errors[0].Message);
        repositorioDespesa.Verify(r => r.Cadastrar(It.IsAny<Despesa>()), Times.Never);
    }

    [TestMethod]
    public void Editar_DespesaAlterandoValorECategorias_AtualizaCorretamente()
    {
        Mock<IRepositorioDespesa> repositorioDespesa = new Mock<IRepositorioDespesa>();
        Mock<IRepositorioCategoria> repositorioCategoria = new Mock<IRepositorioCategoria>();

        Categoria categoriaInicial = new Categoria("Alimentação");
        Categoria categoriaNova1 = new Categoria("Transporte");
        Categoria categoriaNova2 = new Categoria("Hospedagem");

        Despesa despesaExistente = new Despesa(
            "Viagem de negócios",
            new DateTime(2026, 8, 9),
            500.00m,
            FormaPagamento.Debito,
            new List<Categoria> { categoriaInicial }
        );

        List<Despesa> despesas = new() { despesaExistente };

        repositorioDespesa.Setup(r => r.SelecionarTodos()).Returns(() => despesas);
        repositorioCategoria
            .Setup(r => r.SelecionarTodos())
            .Returns(new List<Categoria> { categoriaNova1, categoriaNova2 });

        repositorioDespesa
            .Setup(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Despesa>()))
            .Callback<Guid, Despesa>((id, despesaAtualizada) =>
            {
                despesaAtualizada.Id = id;
                int index = despesas.FindIndex(d => d.Id == id);
                if (index >= 0)
                    despesas[index].Atualizar(despesaAtualizada);
            })
            .Returns<Guid, Despesa>((id, despesaAtualizada) => despesas.Any(d => d.Id == id));

        repositorioCategoria.Setup(r => r.SelecionarPorId(categoriaNova1.Id)).Returns(categoriaNova1);
        repositorioCategoria.Setup(r => r.SelecionarPorId(categoriaNova2.Id)).Returns(categoriaNova2);

        ServicoDespesa servicoDespesa = new ServicoDespesa(
            repositorioDespesa.Object,
            repositorioCategoria.Object
        );

        Result resultado = servicoDespesa.Editar(new EditarDespesaDto(
            despesaExistente.Id,
            "Viagem de negócios atualizada",
            new DateTime(2026, 8, 9),
            800.00m,
            FormaPagamento.Credito,
            new List<Guid> { categoriaNova1.Id, categoriaNova2.Id }
        ));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioDespesa.Verify(r => r.Editar(despesaExistente.Id, It.IsAny<Despesa>()), Times.Once);

        List<ListarDespesasDto> despesasListadas = servicoDespesa.SelecionarTodos();

        Assert.HasCount(1, despesasListadas);
        Assert.AreEqual("Viagem de negócios atualizada", despesasListadas[0].Descricao);
        Assert.AreEqual(800.00m, despesasListadas[0].Valor);
        Assert.AreEqual(FormaPagamento.Credito, despesasListadas[0].FormaPagamento);
        Assert.HasCount(2, despesasListadas[0].Categorias);
        Assert.AreEqual("Transporte", despesasListadas[0].Categorias[0].Titulo);
        Assert.AreEqual("Hospedagem", despesasListadas[0].Categorias[1].Titulo);
    }

    [TestMethod]
    public void Editar_DespesaRemovendoTodasCategorias_RetornaErro()
    {
        Mock<IRepositorioDespesa> repositorioDespesa = new Mock<IRepositorioDespesa>();
        Mock<IRepositorioCategoria> repositorioCategoria = new Mock<IRepositorioCategoria>();

        Categoria categoriaInicial = new Categoria("Alimentação");

        Despesa despesaExistente = new Despesa(
            "Jantar",
            new DateTime(2026, 8, 9),
            120.00m,
            FormaPagamento.Debito,
            new List<Categoria> { categoriaInicial }
        );

        List<Despesa> despesas = new() { despesaExistente };

        repositorioDespesa.Setup(r => r.SelecionarTodos()).Returns(() => despesas);
        repositorioCategoria
           .Setup(r => r.SelecionarTodos())
           .Returns(new List<Categoria> { categoriaInicial });


        ServicoDespesa servicoDespesa = new ServicoDespesa(
            repositorioDespesa.Object,
            repositorioCategoria.Object
        );

        Result resultado = servicoDespesa.Editar(new EditarDespesaDto(
            despesaExistente.Id,
            "Jantar",
            new DateTime(2026, 8, 9),
            150.00m,
            FormaPagamento.Credito,
            new List<Guid>()
        ));

        Assert.IsFalse(resultado.IsSuccess);
        Assert.AreEqual("Selecione ao menos uma categoria.", resultado.Errors[0].Message);

        repositorioDespesa.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Despesa>()), Times.Never);
    }

    [TestMethod]
    public void Visualizar_DespesaCadastrada_ExibeDadosCorretamente()
    {
        Mock<IRepositorioDespesa> repositorioDespesa = new Mock<IRepositorioDespesa>();
        Mock<IRepositorioCategoria> repositorioCategoria = new Mock<IRepositorioCategoria>();

        Categoria categoria1 = new Categoria("Alimentação");
        Categoria categoria2 = new Categoria("Transporte");

        Despesa despesa = new Despesa(
            "Viagem de negócios",
            new DateTime(2026, 8, 9),
            500.00m,
            FormaPagamento.Credito,
            new List<Categoria> { categoria1, categoria2 }
        );

        List<Despesa> despesas = new() { despesa };

        repositorioDespesa.Setup(r => r.SelecionarTodos()).Returns(() => despesas);

        ServicoDespesa servicoDespesa = new ServicoDespesa(
            repositorioDespesa.Object,
            repositorioCategoria.Object
        );

        List<ListarDespesasDto> despesasListadas = servicoDespesa.SelecionarTodos();

        Assert.HasCount(1, despesasListadas);
        Assert.AreEqual("Viagem de negócios", despesasListadas[0].Descricao);
        Assert.AreEqual(new DateTime(2026, 8, 9), despesasListadas[0].DataOcorrencia);
        Assert.AreEqual(500.00m, despesasListadas[0].Valor);
        Assert.AreEqual(FormaPagamento.Credito, despesasListadas[0].FormaPagamento);
        Assert.HasCount(2, despesasListadas[0].Categorias);
        Assert.AreEqual("Alimentação", despesasListadas[0].Categorias[0].Titulo);
        Assert.AreEqual("Transporte", despesasListadas[0].Categorias[1].Titulo);
    }

    [TestMethod]
    public void Listar_TodasDespesasCadastradas_ExibeTodasCorretamente()
    {
        Mock<IRepositorioDespesa> repositorioDespesa = new Mock<IRepositorioDespesa>();
        Mock<IRepositorioCategoria> repositorioCategoria = new Mock<IRepositorioCategoria>();

        Categoria categoria1 = new Categoria("Alimentação");
        Categoria categoria2 = new Categoria("Transporte");

        Despesa despesa1 = new Despesa(
            "Almoço",
            new DateTime(2026, 8, 8),
            50.00m,
            FormaPagamento.Debito,
            new List<Categoria> { categoria1 }
        );

        Despesa despesa2 = new Despesa(
            "Táxi",
            new DateTime(2026, 8, 9),
            30.00m,
            FormaPagamento.AVista,
            new List<Categoria> { categoria2 }
        );

        List<Despesa> despesas = new() { despesa1, despesa2 };

        repositorioDespesa.Setup(r => r.SelecionarTodos()).Returns(() => despesas);

        ServicoDespesa servicoDespesa = new ServicoDespesa(
            repositorioDespesa.Object,
            repositorioCategoria.Object
        );

        List<ListarDespesasDto> despesasListadas = servicoDespesa.SelecionarTodos();

        Assert.HasCount(2, despesasListadas);

        Assert.AreEqual("Almoço", despesasListadas[0].Descricao);
        Assert.AreEqual(50.00m, despesasListadas[0].Valor);
        Assert.AreEqual(FormaPagamento.Debito, despesasListadas[0].FormaPagamento);
        Assert.AreEqual("Alimentação", despesasListadas[0].Categorias[0].Titulo);

        Assert.AreEqual("Táxi", despesasListadas[1].Descricao);
        Assert.AreEqual(30.00m, despesasListadas[1].Valor);
        Assert.AreEqual(FormaPagamento.AVista, despesasListadas[1].FormaPagamento);
        Assert.AreEqual("Transporte", despesasListadas[1].Categorias[0].Titulo);
    }

    [TestMethod]
    public void Excluir_DespesaCadastrada_RemoveDaListagemEVinculosDesfeitos()
    {
        Mock<IRepositorioDespesa> repositorioDespesa = new Mock<IRepositorioDespesa>();
        Mock<IRepositorioCategoria> repositorioCategoria = new Mock<IRepositorioCategoria>();

        Categoria categoria = new Categoria("Alimentação");

        Despesa despesa = new Despesa(
            "Almoço",
            new DateTime(2026, 8, 8),
            50.00m,
            FormaPagamento.AVista,
            new List<Categoria> { categoria }
        );

        List<Despesa> despesas = new() { despesa };

        repositorioDespesa.Setup(r => r.SelecionarTodos()).Returns(() => despesas);
        repositorioDespesa.Setup(r => r.SelecionarPorId(despesa.Id)).Returns(despesa);

        repositorioDespesa
            .Setup(r => r.Excluir(It.IsAny<Guid>()))
            .Callback<Guid>(id =>
            {
                despesas.RemoveAll(d => d.Id == id);
            })
            .Returns<Guid>(id => true);

        ServicoDespesa servicoDespesa = new ServicoDespesa(
            repositorioDespesa.Object,
            repositorioCategoria.Object
        );

        Result resultado = servicoDespesa.Excluir(despesa.Id);

        Assert.IsTrue(resultado.IsSuccess);
        repositorioDespesa.Verify(r => r.Excluir(despesa.Id), Times.Once);

        List<ListarDespesasDto> despesasListadas = servicoDespesa.SelecionarTodos();
        Assert.IsEmpty(despesasListadas);

        Assert.IsFalse(despesas.Any(d => d.Categorias.Contains(categoria)));
    }

}
