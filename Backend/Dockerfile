FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

WORKDIR /build

COPY . .

RUN dotnet restore

RUN dotnet publish ServerList.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine

WORKDIR /app

COPY --from=build /build/out .

CMD ["dotnet", "ServerList.dll"]