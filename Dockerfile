FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

RUN rm -rf /source

COPY . /source
WORKDIR /source

RUN dotnet restore

WORKDIR /source/

RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT dotnet ReceitasApi.dll