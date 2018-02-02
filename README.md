# slack-integration
Integration part that integrates slack to github based on configuration file at repository.

# Setup
Deploy integration to public web address.

Setup global webhook for address `todo`.

# Github json format
Add file `slack.json` to repository.

```json
{
    version: "1",
    channels: ["#general", "#labs"],
    notify: ["pull-request"]
}
```
