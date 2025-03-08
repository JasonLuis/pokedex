namespace Pokedex.APIs.PokemonInfo;

public class PokemonDetalhes
{
    public int Id { get; set; }

    public string? Nome { get; set; }
    public string? Imagem { get; set; }

    public string? Descricao { get; set; }
    public List<Tipos> Tipos { get; set; } = [];

    public List<Habilidades> Habilidades { get; set; } = [];

    public Estatisticas Estatisticas { get; set; } = new Estatisticas();

    public decimal Altura { get; set; }
    public decimal Peso { get; set; }

    public List<Evolucao> Evolucoes { get; set; } = [];
}

public class Tipos
{
    public string Tipo { get; set; } = string.Empty;
}

public class Habilidades
{
    public string Habilidade { get; set; } = string.Empty;
}

public class Estatisticas
{
    public int HP { get; set; }
    public int Ataque { get; set; }
    public int Defesa { get; set; }
    public int AtaqueEspecial { get; set; }
    public int DefesaEspecial { get; set; }
    public int Velocidade { get; set; }
}

public class Evolucao
{
    public string? Nome { get; set; } = string.Empty;
    public string? Imagem { get; set; } = string.Empty;
}