using RestauranteService.Dtos;

namespace RestauranteService; 

public interface IItemServiceHttpClient {
    public void EnviaRestauranteParaItemService(RestauranteReadDto readDto);
}