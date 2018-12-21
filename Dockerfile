FROM microsoft/dotnet:2.2-aspnetcore-runtime-alpine

WORKDIR /app
COPY out .

EXPOSE 5000

ENTRYPOINT ["dotnet", "Slack.Json.dll"]
