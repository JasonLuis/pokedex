using Pokedex.APIs.ListarPokemons;
using System;
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

        var tasks = response!.Resultado.Select(x => GetDetailsPokemon(x.Url));
        var results = await Task.WhenAll(tasks);
        pokemons.AddRange(results);
        
       

        return (pokemons, Anterior, Proximo);
    }

    public async Task<PokemonDetalhes> GetDetailsPokemon(string url)
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

        pokemon.Descricao = await GetDescriptionPokemon(pokemon.Id);

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

    public async Task<string> GetDescriptionPokemon(int id)
    {
        var pokemonResponse = await _httpClient.GetStringAsync($"https://pokeapi.co/api/v2/pokemon-species/{id}/");

        var json = JsonDocument.Parse(pokemonResponse);

        string description = string.Empty;

        if (json.RootElement.TryGetProperty("flavor_text_entries", out JsonElement flavorTextElement))
        {
            foreach (JsonElement entry in flavorTextElement.EnumerateArray())
            {
                if (entry.GetProperty("language").GetProperty("name").GetString() == "en")
                {
                    description = entry.GetProperty("flavor_text").GetString()!;
                    description = description.Replace("\f", " ");
                    description = description.Replace("\n", " ");
                    break;
                }
            }
        }

        return description;
    }

    public async Task<List<PokemonDetalhes>> GetPokemonByName(string name)
    {
        List<PokemonDetalhes> pokemons = [];
        pokemons.Add(await GetDetailsPokemon($"https://pokeapi.co/api/v2/pokemon/{name}/"));

        return pokemons;
    }
}


