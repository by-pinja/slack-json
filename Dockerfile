FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as dotnetBuild

COPY ./ /src/
WORKDIR /src/
RUN dotnet publish -c release -o /out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app
COPY --from=dotnetBuild /out/ /app/

EXPOSE 5000

ENTRYPOINT ["dotnet", "Slack.Json.dll"]
