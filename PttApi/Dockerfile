FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PttApi.csproj", "."]
RUN dotnet restore "./PttApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "PttApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PttApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PttApi.dll"]