FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7001

ENV ASPNETCORE_URLS=http://+:7001

RUN mkdir -p /app/Data
RUN chmod 777 /app/Data

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Olivia.Api/Olivia.Api.csproj", "Olivia.Api/"]
RUN dotnet restore "Olivia.Api/Olivia.Api.csproj"
COPY . .
WORKDIR "/src/Olivia.Api"
RUN dotnet build "Olivia.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Olivia.Api.csproj" -c $configuration -r linux-x64 --self-contained false -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Olivia.Api.dll"]
