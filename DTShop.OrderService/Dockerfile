FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["DTShop.OrderService/DTShop.OrderService.csproj", "DTShop.OrderService/"]
COPY ["DTShop.OrderService.Data/DTShop.OrderService.Data.csproj", "DTShop.OrderService.Data/"]
COPY ["DTShop.OrderService.Core/DTShop.OrderService.Core.csproj", "DTShop.OrderService.Core/"]
RUN dotnet restore "DTShop.OrderService/DTShop.OrderService.csproj"
COPY . .
WORKDIR "/src/DTShop.OrderService"
RUN dotnet build "DTShop.OrderService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DTShop.OrderService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DTShop.OrderService.dll"]