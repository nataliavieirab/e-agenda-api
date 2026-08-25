using AutoMapper;
using eAgenda.Aplicacao.Modulos.ModuloTarefa;

namespace eAgenda.WebApp.Modulos.ModuloTarefa;

public class TarefaProfile : Profile
{
    public TarefaProfile()
    {
        CreateMap<ItemTarefaDto, ItemTarefaViewModel>();
        CreateMap<ListarTarefasDto, ListarTarefasViewModel>();
        CreateMap<CadastrarTarefaViewModel, CadastrarTarefaDto>();
        CreateMap<EditarTarefaViewModel, EditarTarefaDto>();
        CreateMap<AdicionarItemTarefaViewModel, AdicionarItemTarefaDto>();
        CreateMap<DetalhesTarefaDto, EditarTarefaViewModel>();
        CreateMap<DetalhesTarefaDto, ExcluirTarefaViewModel>();

        CreateMap<DetalhesTarefaDto, GerenciarItensTarefaViewModel>()
            .ForCtorParam("NovoItemTitulo", opt => opt.MapFrom(_ => string.Empty));
    }
}
