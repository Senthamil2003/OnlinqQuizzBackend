#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CapstoneQuizzCreationApp/QuizzAppDeploy.csproj", "CapstoneQuizzCreationApp/"]
RUN dotnet restore "./CapstoneQuizzCreationApp/QuizzAppDeploy.csproj"
COPY . .
WORKDIR "/src/CapstoneQuizzCreationApp"
RUN dotnet build "./QuizzAppDeploy.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./QuizzAppDeploy.csproj" -c $BUILD_CONFIGURATION -o /app/publish 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuizzAppDeploy.dll"]