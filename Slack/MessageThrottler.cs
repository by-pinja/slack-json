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

        private readonly ISubject<(string channel, SlackMessageModel model)> _messageSubject =
            Subject.Synchronize(new Subject<(string channel, SlackMessageModel model)>());

        private readonly TimeSpan groupingTimeForMessages = TimeSpan.FromSeconds(15);
        private readonly IScheduler scheduler;

        public IObservable<(string channel, SlackMessageModel model)> Messages()
        {
            return ThrottleByTittle(_messageSubject.Synchronize());
        }

        public IObservable<(string channel, SlackMessageModel model)> ThrottleByTittle(IObservable<(string channel, SlackMessageModel model)> observable)
        {
            return observable.Buffer(groupingTimeForMessages, this.scheduler)
                .SelectMany(x =>
                    x.GroupBy(y => y.model.Title + y.channel)
                        .Select(y => y.Last()));
        }

        public void Emit(string channel, SlackMessageModel model)
        {
            _messageSubject.OnNext((channel, model));
        }
    }
}