#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base

ENV ASPNETCORE_ENVIRONMENT=Development 
WORKDIR /app
ADD ./Deploy/ /app
ENTRYPOINT ["dotnet", "VTQT.Api.Asset.dll"]
EXPOSE 80/tcp