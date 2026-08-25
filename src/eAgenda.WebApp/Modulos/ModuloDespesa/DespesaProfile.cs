using AutoMapper;
using eAgenda.Aplicacao.Modulos.ModuloDespesa;

namespace eAgenda.WebApp.Modulos.ModuloDespesa;

public class DespesaProfile : Profile
{
    public DespesaProfile()
    {
        CreateMap<CategoriaDespesaDto, CategoriaDespesaViewModel>();
        CreateMap<ListarDespesasDto, ListarDespesasViewModel>();
        CreateMap<CadastrarDespesaViewModel, CadastrarDespesaDto>();
        CreateMap<EditarDespesaViewModel, EditarDespesaDto>();

        CreateMap<DetalhesDespesaDto, EditarDespesaViewModel>()
            .ForCtorParam("CategoriaIds", opt => opt.MapFrom(src => src.Categorias.Select(c => c.Id).ToList()))
            .ForCtorParam("Categorias", opt => opt.MapFrom(_ => new List<CategoriaDespesaViewModel>()));

        CreateMap<DetalhesDespesaDto, ExcluirDespesaViewModel>();
    }
}
