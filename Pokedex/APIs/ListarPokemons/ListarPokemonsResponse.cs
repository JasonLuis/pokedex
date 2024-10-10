using System.Text.Json.Serialization;

namespace Pokedex.APIs.ListarPokemons;

public class ListarPokemonsResponse
{
    [JsonPropertyName("count")]
    public int Quantidade { get; set; }

    [JsonPropertyName("next")]
    public string? Proximo { get; set; }

    [JsonPropertyName("previous")]
    public string? Anterior { get; set; }

    [JsonPropertyName("results")]
    public PokemonItemResponse[] Resultado { get; set; } = Array.Empty<PokemonItemResponse>(); 
}

public class PokemonItemResponse {

    [JsonPropertyName("name")]
    public string Nome { get; set; } = string.Empty;
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}




