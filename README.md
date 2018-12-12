[![Docker pulls](https://img.shields.io/docker/pulls/ptcos/slack-json.svg)](https://hub.docker.com/r/ptcos/slack-json/)
[![Build Status](https://jenkins.protacon.cloud/buildStatus/icon?job=www.github.com/slack-json/master)](https://jenkins.protacon.cloud/job/www.github.com/job/slack-json/job/master/)

# slack-integration
Service which integrates slack to github based on configuration file `slack.json` at repository.

There are plenty slack integration for github around. However solution where configuration exists in version control and it supports fine grained configurations about what and to which channels should be notified. For example slack side apps supports multiple different notification types but only to one channel at time.

At best this solution is when hooks can be enabled at organization level. After that no further configuration is needed repository basis -> just add `slack.json`.

# Setup for production
## Deployment
Deploy this .NET core app for you favorite hosting service or docker cluster.

### Docker example
```bash
docker run -it \
    --env=SlackIntegrationUri=https://hooks.slack.com/services/12345/12345123451234512345 \
    --env=GithubPersonalAccessToken=1234512345123451234512345123451234512345 \
    -p 5000:5000 \
    ptcos/slack-json:latest
```

## Github configuration
1. Setup github webhook (global or repository) for address `https://www.yourinstallation.io/v1/api/github/`.
2. Create personal access token and add it to slack-integration service appsettings.json or to environment variable `GithubPersonalAccessToken`.

## Slack configuration
1. Create new 'Incoming webhook' app and set 'Webhook URL' to appsettings.json or environment variable `SlackIntegrationUri`.

# slack.json
Add file `slack.json` to repository root folder.

Action structure:
```json
{
    "type": "this_is_type",
    "channel": "#ThisIsTargetChannel",
    "enabled": true,
    "data": [ "somedata" ]
}
```

Only `type` and `channel` are mandatory. Enabled defaults to true if not set and data is available only on few configurations.

## Global configuration
App supports globally configured actions which are invoked on every repository event when slack.json is missing. Use `GlobalSlackJson` on environment variable or configuration file to support this scenario.

## Supported types
| Type                        | Description                                                   | Misc                    |
| --------------------------- | --------------------------------------------------------      | ----------------------- |
| new_issue                   | Get notification when new issue is posted to repository.      |                         |
| new_repository              | Notifies channel when new repository is created.       | This must be enabled globally, not allowed on repository basic configuration. |
| new_release                 | Notifies channel when new release is created.          |                         |
| pull_request                | Notifies channel about new pull requests on repository.       |                         |
| review_request              | Notifies channel about new review requests on repository.     |                         |
| review_status               | Notifies channel about updates on reviews, like reviewed or needs fix. | |
| issue_label                 | Notifies channel about labels on issues. | Supports filtering with `data: [ "needs help", "bug" ]`, if data is not defined all labels are accepted. |
| pullrequest_label           | Notifies channel about new labels on pull requests. | Supports filtering with `data: [ "needs help", "bug" ]`, if data is not defined all labels are accepted. |
| jenkins_build_error         | Jenkins ci build failed. |  |
| jenkins_build_tag           | When tag is built on jenkins notifications of states like success, pending and error is sent. |  |

## Full example
```json
{
    "version": "1",
    "actions": [
        {
            "type": "new_issue",
            "channel": "#best_project"
        },
        {
            "type": "new_issue",
            "channel": "#anotherChannel"
        },
        {
            "type": "new_release",
            "channel": "#anotherChannel"
        },
        {
            "type": "pull_request",
            "channel": "#best_project"
        },
        {
            "type": "review_request",
            "channel": "#best_devops_channel_ever"
        },
        {
            "type": "issue_label",
            "channel": "#lotsOfBugsChannel",
            "enabled": true,
            "data": [ "bug" ]
        },
        {
            "type": "pullrequest_label",
            "channel": "#evochannel",
            "data": [ "needs help" ]
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

See swagger document at http://0.0.0.0:5000/doc
