namespace Pokedex.APIs.ListarPokemons;

public class PokemonDetalhes
{
    public int Id { get; set; }

    public string? Nome { get; set; }
    public string? Imagem { get; set; }

    public string? Descricao { get; set; }
    public List<Habilidades> Habilidades { get; set; } = [];

}

public class Habilidades
{
    public string Tipo { get; set; } = string.Empty;
}


