#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

#ARG GIT_DESCRIBE=unspecified
#LABEL git_describe=$GIT_DESCRIBE

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY TempMonitor/Server/TempMonitor.Server.csproj TempMonitor/Server/
COPY TempMonitor/Client/TempMonitor.Client.csproj TempMonitor/Client/
COPY TempMonitor/Shared/TempMonitor.Shared.csproj TempMonitor/Shared/
RUN dotnet restore "TempMonitor/Server/TempMonitor.Server.csproj"
COPY . .
WORKDIR "/src/TempMonitor/Server"


FROM build AS publish
ARG GIT_DESCRIBE=notset
RUN echo "GIT_DESCRIBE: ${GIT_DESCRIBE}"
RUN dotnet publish "TempMonitor.Server.csproj" -c Release -p:InformationalVersion=$GIT_DESCRIBE -o /app/publish 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TempMonitor.Server.dll"]
