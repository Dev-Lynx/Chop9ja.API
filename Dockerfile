FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Chop9ja.API/Chop9ja.API.csproj", "Chop9ja.API/"]
RUN dotnet restore "Chop9ja.API/Chop9ja.API.csproj"
COPY . .
WORKDIR "/src/Chop9ja.API"
RUN dotnet build "Chop9ja.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Chop9ja.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Chop9ja.API.dll"]