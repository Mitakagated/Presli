#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Presli/Presli.csproj", "Presli/"]
RUN dotnet restore "Presli/Presli.csproj"
COPY . .
WORKDIR "/src/Presli"
RUN dotnet build "Presli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Presli.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY ["Presli/yugioh/", "/app/yugioh"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Presli.dll"]