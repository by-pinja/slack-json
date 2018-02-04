[![Build Status](https://jenkins.protacon.cloud/buildStatus/icon?job=www.github.com/slack-integration/master)](https://jenkins.protacon.cloud/job/www.github.com/job/slack-integration/job/master/)

# slack-integration
Service which integrates slack to github based on configuration file `slack.json` at repository.

There are plenty slack integration for github around. However solution where configuration exists in version control and it supports fine grained configurations about what and to which channels should be notified. For example slack side apps supports multiple different notification types but only to one channel at time.

At best this solution is when hooks can be enabled at organization level. After that no further configuration is needed repository basis -> just add `slack.json`.

# Setup for production
## Deployment
Deploy this .NET core app for you favorite hosting service or docker cluster.

### Docker example
...

## Github configuration
1. Setup github webhook (global or repository) for address `https://www.yourinstallation.io/v1/api/github/`.
2. Create personal access token and add it to slack-integration service appsettings.json or to environment variable `ASPNETCORE_GithubPersonalAccessToken`.

## Slack configuration
1. Create new 'Incoming webhook' app and set 'Webhook URL' to appsettings.json or environment variable `ASPNETCORE_SlackIntegrationUri`.

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
            "type": "build_failure",
            "channel": "#best_project"
        },
        {
            "type": "review_request",
            "channel": "#best_project"
        },
        {
            "type": "review_done",
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
