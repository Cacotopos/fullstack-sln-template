FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /src
COPY . .
ENTRYPOINT ["dotnet", "test", "src/Company.Product.Tests/Company.Product.Tests.csproj", "-c", "Release"]
