FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7000

ENV ASPNETCORE_URLS=http://+:7000

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Olivia.Client/Olivia.Client.csproj", "Olivia.Client/"]
RUN dotnet restore "Olivia.Client/Olivia.Client.csproj"
COPY . .
WORKDIR "/src/Olivia.Client"
RUN dotnet build "Olivia.Client.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Olivia.Client.csproj" -c $configuration -r linux-x64 --self-contained false -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Olivia.Client.dll"]
