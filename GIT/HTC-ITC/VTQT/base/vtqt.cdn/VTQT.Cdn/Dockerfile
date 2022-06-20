#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base

ENV ASPNETCORE_ENVIRONMENT=Production
WORKDIR /app
ADD ./deploy/ /app
ENTRYPOINT ["dotnet", "VTQT.Cdn.dll"]
