namespace RestauranteService.EventProcessor; 

public interface IProcessaEvento {
    void Processa(string message);
}