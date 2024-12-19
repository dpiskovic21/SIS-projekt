FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln ./
COPY SIS-projekt/*.csproj SIS-projekt/

RUN dotnet restore

COPY . .
WORKDIR /app/SIS-projekt
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

EXPOSE 8080

ENTRYPOINT ["dotnet", "SIS-projekt.dll"]
