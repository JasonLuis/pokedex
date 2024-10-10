using Pokedex.APIs.ListarPokemons;
using System.Text.Json;

namespace Pokedex.API;

public class PokemonAPI
{
    private readonly HttpClient _httpClient;

    public PokemonAPI(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("PokeAPI");
    }


    public async Task<(List<PokemonDetalhes> Pokemons, string Anterior, string Proximo)> GetPokemon(string url)
    {
        ListarPokemonsResponse? response;

        if (url is null || url == string.Empty)
        {
            response = await _httpClient.GetFromJsonAsync<ListarPokemonsResponse>("pokemon?limit=9&offset=0");
        } else
        {
            response = await _httpClient.GetFromJsonAsync<ListarPokemonsResponse>(url);
        }

        string Anterior = response!.Anterior!;
        string Proximo = response!.Proximo!;


        List<PokemonDetalhes> pokemons = [];

        var tasks = response!.Resultado.Select(x => GetAboutPokemon(x.Url));
        var results = await Task.WhenAll(tasks);
        pokemons.AddRange(results);
        
       

        return (pokemons, Anterior, Proximo);
    }

    public async Task<PokemonDetalhes> GetAboutPokemon(string url)
    {
        var pokemonResponse = await _httpClient.GetStringAsync(url);
        
        PokemonDetalhes pokemon = new();
        var json = JsonDocument.Parse(pokemonResponse);

        pokemon.Id = json.RootElement
        .GetProperty("id")
        .GetInt32();

        pokemon.Nome = json.RootElement
        .GetProperty("name")
        .GetString();


        pokemon.Imagem = json.RootElement
        .GetProperty("sprites")
        .GetProperty("versions")
        .GetProperty("generation-v")
        .GetProperty("black-white")
        .GetProperty("animated")
        .GetProperty("front_default")
        .GetString();

        

        if (json.RootElement.TryGetProperty("types", out JsonElement typesElement))
        {
            foreach (JsonElement typeEntry in typesElement.EnumerateArray())
            {
                string tipo = typeEntry.GetProperty("type").GetProperty("name").GetString()!;
                pokemon.Habilidades.Add(new Habilidades() { Tipo = tipo });
            }
        }

        return pokemon;
    }

}


