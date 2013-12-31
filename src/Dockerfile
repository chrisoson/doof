FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["doof.csproj", "./"]
RUN dotnet restore "doof.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "doof.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "doof.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "doof.dll"]
