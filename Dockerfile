FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TodoApp.Api/TodoApp.Api.csproj", "TodoApp.Api/"]
COPY ["TodoApp.Application/TodoApp.Application.csproj", "TodoApp.Application/"]
COPY ["TodoApp.Core/TodoApp.Core.csproj", "TodoApp.Core/"]
COPY ["TodoApp.Domain/TodoApp.Domain.csproj", "TodoApp.Domain/"]
COPY ["TodoApp.Infrastructure/TodoApp.Infrastructure.csproj", "TodoApp.Infrastructure/"]

RUN dotnet restore "TodoApp.Api/TodoApp.Api.csproj"

COPY . .

WORKDIR "/src/TodoApp.Api"

RUN dotnet build "TodoApp.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TodoApp.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "TodoApp.Api.dll"]
