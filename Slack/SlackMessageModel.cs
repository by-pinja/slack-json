namespace Slack.Json.Slack
{
    public class SlackMessageModel
    {
        public SlackMessageModel(string title, string href)
        {
            Title = title;
            Href = href;
        }

        public string Title { get; set; }
        public string Text { get; set; } = "";
        public string Href { get; set; }
        public string Icon { get; set;} = ":github:";
        public string Color { get; set; } = "warning";
    }
}