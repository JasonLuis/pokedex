namespace Pokedex.Services;

public class PokemonService
{
    private readonly HttpClient _httpClient;

    public PokemonService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("PokeAPI");
    }


    public async Task<PokeApiResponse?> GetNextLink()
    {
        var response = await _httpClient.GetFromJsonAsync<PokeApiResponse>("pokemon?limit=1&offset=10");
        
        return response;
    }

    public class PokeApiResponse
    {
        public int Count { get; set; }
        public string? Next { get; set; }
        public string? Previous { get; set; }
        public List<PokemonResult> Results { get; set; } = new List<PokemonResult>();
    }

    public class PokemonResult
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}


