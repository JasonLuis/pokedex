using Pokedex.APIs.ListarPokemons;
using Pokedex.APIs.PokemonInfo;
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



    public async Task<(List<PokemonDetalhes> Pokemons, string Anterior, string Proximo)> ListarPokemons(string url)
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

        var tasks = response!.Resultado.Select(x => GetPokemonDetalhes(x.Url));
        var results = await Task.WhenAll(tasks);
        pokemons.AddRange(results);
        
       

        return (pokemons, Anterior, Proximo);
    }


    public async Task<PokemonDetalhes> GetPokemonDetalhes(string? url, int? id = null)
    {
        var urlPokemon = url ?? $"https://pokeapi.co/api/v2/pokemon/{id}/";

        var pokemonResponse = await _httpClient.GetStringAsync(urlPokemon);
        
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

        pokemon.Descricao = await GetDescricaoPokemon(pokemon.Id);

        pokemon.Altura = (decimal)json.RootElement.GetProperty("height").GetInt32() / 10; 
        pokemon.Peso = (decimal)json.RootElement.GetProperty("weight").GetDecimal() / 10;

        if (json.RootElement.TryGetProperty("types", out JsonElement typesElementToTypes))
        {
            foreach (JsonElement typeEntry in typesElementToTypes.EnumerateArray())
            {
                string tipo = typeEntry.GetProperty("type").GetProperty("name").GetString()!;
                pokemon.Tipos.Add(new Tipos() { Tipo = tipo });
            }
        }

        if (json.RootElement.TryGetProperty("abilities", out JsonElement typesElementToAbility))
        {
            foreach (JsonElement typeEntry in typesElementToAbility.EnumerateArray())
            {
                string habilidade = typeEntry.GetProperty("ability").GetProperty("name").GetString()!;
                pokemon.Habilidades.Add(new Habilidades() { Habilidade = habilidade });
            }
        }

        if (json.RootElement.TryGetProperty("stats", out JsonElement typesElementToStats))
        {
            var estatisticas = new Estatisticas();
            foreach (JsonElement typeEntry in typesElementToStats.EnumerateArray())
            {
                string stat = typeEntry.GetProperty("stat").GetProperty("name").GetString()!;
                int valor = typeEntry.GetProperty("base_stat").GetInt32();
                switch (stat)
                {
                    case "hp":
                        estatisticas.HP = valor;
                        break;
                    case "attack":
                        estatisticas.Ataque = valor;
                        break;
                    case "defense":
                        estatisticas.Defesa = valor;
                        break;
                    case "special-attack":
                        estatisticas.AtaqueEspecial = valor;
                        break;
                    case "special-defense":
                        estatisticas.DefesaEspecial = valor;
                        break;
                    case "speed":
                        estatisticas.Velocidade = valor;
                        break;
                }
            }
            
            pokemon.Estatisticas = estatisticas;
        }

        return pokemon;
    }

    public async Task<string> GetDescricaoPokemon(int id)
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

    public async Task<List<PokemonDetalhes>> GetPokemonPorNome(string name)
    {
        List<PokemonDetalhes> pokemons = [];
        pokemons.Add(await GetPokemonDetalhes($"https://pokeapi.co/api/v2/pokemon/{name}/"));

        return pokemons;
    }
}


