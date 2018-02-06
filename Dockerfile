FROM microsoft/aspnetcore:2.0

WORKDIR /app
COPY out .

EXPOSE 5000

ENTRYPOINT ["dotnet", "Slack.Json.dll"]
