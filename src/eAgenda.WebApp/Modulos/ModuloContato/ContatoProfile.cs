using AutoMapper;
using eAgenda.Aplicacao.Modulos.ModuloContato;

namespace eAgenda.WebApp.Modulos.ModuloContato;

public class ContatoProfile : Profile
{
    public ContatoProfile()
    {
        CreateMap<ListarContatosDto, ListarContatosViewModel>();
        CreateMap<CadastrarContatoViewModel, CadastrarContatoDto>();
        CreateMap<EditarContatoViewModel, EditarContatoDto>();
        CreateMap<DetalhesContatoDto, EditarContatoViewModel>();
        CreateMap<DetalhesContatoDto, ExcluirContatoViewModel>();
    }
}
