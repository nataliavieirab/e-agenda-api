using eAgenda.Aplicacao.Modulos.ModuloCategoria;
using eAgenda.Dominio.Modulos.ModuloCategoria;
using eAgenda.Dominio.Modulos.ModuloDespesa;
using FluentResults;
using Moq;

namespace eAgenda.Testes.Unidade.Modulos.ModuloCategoria;

[TestClass]
public sealed class ServicoCategoriaTestes
{
    [TestMethod]
    public void Cadastrar_ComDadosValidos_PersisteCategoria()
    {
        Mock<IRepositorioCategoria> repositorioCategoria = new();
        Mock<IRepositorioDespesa> repositorioDespesa = new();

        repositorioCategoria.Setup(r => r.SelecionarTodos()).Returns([]);

        Categoria? categoriaCadastrada = null;

        repositorioCategoria
            .Setup(r => r.Cadastrar(It.IsAny<Categoria>()))
            .Callback<Categoria>(
                categoria => categoriaCadastrada = categoria
            );

        ServicoCategoria servicoCategoria = new ServicoCategoria(
            repositorioCategoria.Object,
            repositorioDespesa.Object
        );

        Result resultado = servicoCategoria.Cadastrar(new CadastrarCategoriaDto(
            "Limpeza"
        ));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(categoriaCadastrada);
        Assert.AreEqual("Limpeza", categoriaCadastrada.Titulo);

        repositorioCategoria.Verify(r => r.Cadastrar(It.IsAny<Categoria>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_ComTituloVazio_RetornaErro()
    {
        Mock<IRepositorioCategoria> repositorioCategoria = new();
        Mock<IRepositorioDespesa> repositorioDespesa = new();

        repositorioCategoria.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoCategoria servicoCategoria = new(
            repositorioCategoria.Object,
            repositorioDespesa.Object
        );

        Result resultado = servicoCategoria.Cadastrar(new CadastrarCategoriaDto(
            string.Empty
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("O campo \"Título\" deve ser preenchido.", resultado.Errors.First().Message);

        repositorioCategoria.Verify(r => r.Cadastrar(It.IsAny<Categoria>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_TituloDuplicado_RetornaFalha()
    {
        Mock<IRepositorioCategoria> repositorioCategoria = new();
        Mock<IRepositorioDespesa> repositorioDespesa = new();

        repositorioCategoria.Setup(r => r.SelecionarTodos())
        .Returns([new Categoria(
            "Limpeza"
        )]);

        ServicoCategoria servicoCategoria = new(
            repositorioCategoria.Object,
            repositorioDespesa.Object
        );

        Result resultado = servicoCategoria.Cadastrar(new CadastrarCategoriaDto(
            "Limpeza"
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Titulo", resultado.Errors.Single().Metadata["Campo"]);
        Assert.Contains("Já existe", resultado.Errors.Single().Message);

        repositorioCategoria.Verify(r => r.Cadastrar(It.IsAny<Categoria>()), Times.Never);
    }

    [TestMethod]
    public void Editar_ComDadosValidos_PersisteCategoria()
    {
        Mock<IRepositorioCategoria> repositorioCategoria = new();
        Mock<IRepositorioDespesa> repositorioDespesa = new();

        Categoria categoriaExistente = new Categoria(
            "Limpeza"
        );

        List<Categoria> categorias = new() { categoriaExistente };

        repositorioCategoria.Setup(r => r.SelecionarTodos()).Returns(() => categorias);
        repositorioCategoria
            .Setup(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Categoria>()))
            .Callback<Guid, Categoria>((id, categoriaAtualizada) =>
            {
                categoriaAtualizada.Id = id;
                int index = categorias.FindIndex(c => c.Id == id);
                if (index >= 0)
                    categorias[index].Atualizar(categoriaAtualizada);
            })
            .Returns<Guid, Categoria>((id, contatoAtualizado) => categorias.Any(c => c.Id == id));

        ServicoCategoria servicoCategoria = new ServicoCategoria(
            repositorioCategoria.Object,
            repositorioDespesa.Object
        );

        Result resultado = servicoCategoria.Editar(new EditarCategoriaDto(
            categoriaExistente.Id,
            "Mercado"
        ));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioCategoria.Verify(r => r.Editar(categoriaExistente.Id, It.IsAny<Categoria>()), Times.Once);

        List<ListarCategoriasDto> categoriasListadas = servicoCategoria.SelecionarTodos();

        Assert.HasCount(1, categoriasListadas);
        Assert.AreEqual("Mercado", categoriasListadas[0].Titulo);
    }

    [TestMethod]
    public void Editar_ComTituloDuplicado_RetornaFalha()
    {
        Mock<IRepositorioCategoria> repositorioCategoria = new();
        Mock<IRepositorioDespesa> repositorioDespesa = new();

        Categoria categoriaAlimentacao = new Categoria(
            "Alimentação"
        );

        Categoria categoriaTransporte = new Categoria(
            "Transporte"
        );

        List<Categoria> categorias = new() { categoriaAlimentacao, categoriaTransporte };

        repositorioCategoria.Setup(r => r.SelecionarTodos()).Returns(() => categorias);

        ServicoCategoria servicoCategoria = new ServicoCategoria(
            repositorioCategoria.Object,
            repositorioDespesa.Object
        );

        Result resultado = servicoCategoria.Editar(new EditarCategoriaDto(
            categoriaTransporte.Id,
            "Alimentação"
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Titulo", resultado.Errors.Single().Metadata["Campo"]);
        Assert.Contains("Já existe", resultado.Errors.Single().Message);
        repositorioCategoria.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Categoria>()), Times.Never);
    }

    [TestMethod]
    public void SelecionarDespesas_PorCategoria_RetornaDespesas()
    {
        Mock<IRepositorioCategoria> repositorioCategoria = new();
        Mock<IRepositorioDespesa> repositorioDespesa = new();

        Categoria categoria = new Categoria(
            "Alimentação"
        );

        Despesa despesa1 = new Despesa(
            "Supermercado",
            DateTime.Today,
            120.50m,
            FormaPagamento.Debito,
            new List<Categoria> { categoria }
        );

        Despesa despesa2 = new Despesa(
            "Padaria",
            DateTime.Today,
            25.00m,
            FormaPagamento.AVista,
            new List<Categoria> { categoria }
        );

        categoria.Despesas.Add(despesa1);
        categoria.Despesas.Add(despesa2);

        repositorioCategoria
            .Setup(r => r.SelecionarPorId(categoria.Id))
            .Returns(categoria);

        ServicoCategoria servicoCategoria = new ServicoCategoria(
            repositorioCategoria.Object,
            repositorioDespesa.Object
        );

        Result<DetalhesCategoriaDto> resultado = servicoCategoria.SelecionarPorId(categoria.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(resultado.Value);
        Assert.AreEqual(categoria.Id, resultado.Value.Id);
        Assert.AreEqual("Alimentação", resultado.Value.Titulo);
        Assert.HasCount(2, categoria.Despesas);
        Assert.AreEqual("Supermercado", categoria.Despesas[0].Descricao);
        Assert.AreEqual("Padaria", categoria.Despesas[1].Descricao);
    }

    [TestMethod]
    public void SelecionarTodas_RetornaCategoriasCadastradas()
    {
        Mock<IRepositorioCategoria> repositorioCategoria = new();
        Mock<IRepositorioDespesa> repositorioDespesa = new();

        Categoria categoriaAlimentacao = new Categoria(
            "Alimentação"
        );

        Categoria categoriaTransporte = new Categoria(
            "Transporte"
        );

        categoriaAlimentacao.Despesas.Add(new Despesa(
            "Supermercado",
            DateTime.Today,
            100m,
            FormaPagamento.Debito,
            new List<Categoria> { categoriaAlimentacao }
        ));

        categoriaAlimentacao.Despesas.Add(new Despesa(
            "Padaria",
            DateTime.Today,
            30m,
            FormaPagamento.AVista,
            new List<Categoria> { categoriaAlimentacao }
        ));

        categoriaTransporte.Despesas.Add(new Despesa(
            "Ônibus",
            DateTime.Today,
            8.50m,
            FormaPagamento.AVista,
            new List<Categoria> { categoriaTransporte }
        ));

        List<Categoria> categorias = new() { categoriaAlimentacao, categoriaTransporte };

        repositorioCategoria.Setup(r => r.SelecionarTodos()).Returns(() => categorias);

        ServicoCategoria servicoCategoria = new ServicoCategoria(
            repositorioCategoria.Object,
            repositorioDespesa.Object
        );

        List<ListarCategoriasDto> categoriasListadas = servicoCategoria.SelecionarTodos();

        Assert.HasCount(2, categoriasListadas);
        Assert.AreEqual("Alimentação", categoriasListadas[0].Titulo);
        Assert.AreEqual("Transporte", categoriasListadas[1].Titulo);
        Assert.HasCount(2, categoriaAlimentacao.Despesas);
        Assert.HasCount(1, categoriaTransporte.Despesas);
    }

    [TestMethod]
    public void Excluir_SemDespesasVinculadas_ExcluiCategoria()
    {
        Mock<IRepositorioCategoria> repositorioCategoria = new();
        Mock<IRepositorioDespesa> repositorioDespesa = new();

        Categoria categoria = new Categoria(
            "Lazer"
        );

        repositorioCategoria
            .Setup(r => r.SelecionarPorId(categoria.Id))
            .Returns(categoria);
        repositorioDespesa
            .Setup(r => r.SelecionarTodos())
            .Returns(new List<Despesa>());

        ServicoCategoria servicoCategoria = new ServicoCategoria(
            repositorioCategoria.Object,
            repositorioDespesa.Object
        );

        Result resultado = servicoCategoria.Excluir(categoria.Id);

        Assert.IsTrue(resultado.IsSuccess);
        repositorioCategoria.Verify(r => r.Excluir(categoria.Id), Times.Once);
    }

    [TestMethod]
    public void Excluir_ComDespesasVinculadas_RetornaFalha()
    {
        Mock<IRepositorioCategoria> repositorioCategoria = new();
        Mock<IRepositorioDespesa> repositorioDespesa = new();

        Categoria categoria = new Categoria(
            "Lazer"
        );

        Despesa despesa = new Despesa(
            "Cinema",
            DateTime.Today,
            35.00m,
            FormaPagamento.AVista,
            new List<Categoria> { categoria }
        );

        repositorioCategoria
            .Setup(r => r.SelecionarPorId(categoria.Id))
            .Returns(categoria);
        repositorioDespesa
            .Setup(r => r.SelecionarTodos())
            .Returns(new List<Despesa> { despesa });

        ServicoCategoria servicoCategoria = new ServicoCategoria(
            repositorioCategoria.Object,
            repositorioDespesa.Object
        );

        Result resultado = servicoCategoria.Excluir(categoria.Id);

        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("despesas vinculadas", resultado.Errors.Single().Message);
        repositorioCategoria.Verify(r => r.Excluir(It.IsAny<Guid>()), Times.Never);
    }
}
