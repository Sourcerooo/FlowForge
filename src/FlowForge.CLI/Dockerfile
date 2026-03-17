FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["global.json", "."]
COPY ["src/FlowForge.Domain/FlowForge.Domain.csproj", "src/FlowForge.Domain/"]
COPY ["src/FlowForge.Application/FlowForge.Application.csproj", "src/FlowForge.Application/"]
COPY ["src/FlowForge.Infrastructure/FlowForge.Infrastructure.csproj", "src/FlowForge.Infrastructure/"]
COPY ["src/FlowForge.CLI/FlowForge.CLI.csproj", "src/FlowForge.CLI/"]

RUN dotnet restore "src/FlowForge.CLI/FlowForge.CLI.csproj"

COPY . .
RUN dotnet publish "src/FlowForge.CLI/FlowForge.CLI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FlowForge.CLI.dll"]
