FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /src
COPY . .
ENTRYPOINT ["dotnet", "test", "--project", "src/Company.Product.Tests/Company.Product.Tests.csproj", "-c", "Release"]
