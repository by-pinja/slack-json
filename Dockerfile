FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview7-alpine3.9 as dotnetBuild

COPY ./ /src/
WORKDIR /src/
RUN dotnet publish -c release -o /out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2.6-alpine3.9

WORKDIR /app
COPY --from=dotnetBuild /out/ /app/

EXPOSE 5000

ENTRYPOINT ["dotnet", "Slack.Json.dll"]
