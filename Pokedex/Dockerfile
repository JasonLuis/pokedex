# Use a imagem base do .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copie os arquivos publicados para o contêiner
COPY published/ ./

# Defina o comando de inicialização
ENTRYPOINT ["dotnet", "Pokedex.dll"]