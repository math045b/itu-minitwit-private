FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["minitwit.web.csproj", "./"]
RUN dotnet restore "minitwit.web.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "minitwit.web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "minitwit.web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

RUN mkdir -p /app/data

COPY --from=publish /app/publish .

COPY minitwit.db /app/

RUN chmod 666 /app/minitwit.db

ENV ConnectionStrings__DefaultConnection="Data Source=/app/minitwit.db"

ENV ASPNETCORE_URLS="http://0.0.0.0:5000"

ENTRYPOINT ["dotnet", "minitwit.web.dll"]
