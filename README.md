[![Build Status](https://jenkins.protacon.cloud/buildStatus/icon?job=www.github.com/slack-integration/master)](https://jenkins.protacon.cloud/job/www.github.com/job/slack-integration/job/master/)

# slack-integration
Integration part that integrates slack to github based on configuration file at repository.

# Setup for production
Deploy this .NET core app for you favorite hosting service or docker cluster.

Setup webhook (global or repository) for address `https://www.yourinstallation.io/v1/api/github/`.

# slack.json format
Add file `slack.json` to repository root folder.

```json
{
    "version": "1",
    "actions": [
        {
            "type": "pull_request",
            "channel": "#best_project"
        },
        {
            "type": "build.failure",
            "channel": "#best_project"
        },
        {
            "type": "review_request",
            "channel": "#best_project"
        },
        {
            "type": "reviewed",
            "channel": "#best_project",
            "enabled": false
        }
    ]
}
```

# Development
Install .NET Core 2.x. and configure appsettings.json or (git ignored) appsettings.localdev.json with correct keys.

```bash
dotnet run
```

Now service runs at http://0.0.0.0:5000/, setup github hooks against your public address and you should get hook messages from github in your local development environment.