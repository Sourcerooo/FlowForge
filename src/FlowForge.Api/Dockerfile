FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["global.json", "."]
COPY ["src/FlowForge.Domain/FlowForge.Domain.csproj", "src/FlowForge.Domain/"]
COPY ["src/FlowForge.Application/FlowForge.Application.csproj", "src/FlowForge.Application/"]
COPY ["src/FlowForge.Infrastructure/FlowForge.Infrastructure.csproj", "src/FlowForge.Infrastructure/"]
COPY ["src/FlowForge.Api/FlowForge.Api.csproj", "src/FlowForge.Api/"]

RUN dotnet restore "src/FlowForge.Api/FlowForge.Api.csproj"

COPY . .
RUN dotnet publish "src/FlowForge.Api/FlowForge.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FlowForge.Api.dll"]
