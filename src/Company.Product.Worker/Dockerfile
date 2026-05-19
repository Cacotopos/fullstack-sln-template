FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish src/Company.Product.Worker/Company.Product.Worker.csproj -c Release -o /app/publish
FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Company.Product.Worker.dll"]
