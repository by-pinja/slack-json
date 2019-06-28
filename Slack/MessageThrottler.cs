using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Slack.Json.Slack
{
    public class MessageThrottler
    {
        public MessageThrottler(IScheduler scheduler)
        {
            this.scheduler = scheduler;
        }

        private readonly ISubject<(string slackChannel, SlackMessageModel model)> _messageSubject =
            Subject.Synchronize(new Subject<(string slackChannel, SlackMessageModel model)>());

        private readonly TimeSpan groupingTimeForMessages = TimeSpan.FromSeconds(15);
        private readonly IScheduler scheduler;

        public IObservable<(string slackChannel, SlackMessageModel model)> Messages()
        {
            return ThrottleByTittle(_messageSubject.Synchronize());
        }

        public IObservable<(string slackChannel, SlackMessageModel model)> ThrottleByTittle(IObservable<(string slackChannel, SlackMessageModel model)> observable)
        {
            return observable.Buffer(groupingTimeForMessages, this.scheduler)
                .SelectMany(x =>
                    x.GroupBy(y => y.model.Title + y.slackChannel)
                        .Select(y => y.Last()));
        }

        public void Emit(string slackChannel, SlackMessageModel model)
        {
            _messageSubject.OnNext((slackChannel, model));
        }
    }
}