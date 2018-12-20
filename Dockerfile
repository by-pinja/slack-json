FROM microsoft/aspnetcore:2.2

WORKDIR /app
COPY out .

EXPOSE 5000

ENTRYPOINT ["dotnet", "Slack.Json.dll"]
