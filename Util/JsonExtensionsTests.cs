using System;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Slack.Json.Util.Tests
{
    public class JsonExtensionsTests
    {
        [Fact]
        public void WhenValidJsonObjectIsFetched_ThenReturnItAsString()
        {
            var obj = JObject.Parse("{ val: { varsub: 3 }}");

            obj.Get(x => x.val.varsub).Should().Be("3");
        }

        [Fact]
        public void WhenInvalidRootIsDefined_ThenThrowInvalidOperationException()
        {
            var obj = JObject.Parse("{ val: { varsub: 3 }}");

            obj.Invoking(o => o.Get(x => x.thisdoesntexist.varsub))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void WhenInvalidLeafIsDefined_ThenThrowInvalidOperationException()
        {
            var obj = JObject.Parse("{ val: { varsub: 3 }}");

            obj.Invoking(o => o.Get(x => x.val.varsubfoo))
                .Should().Throw<InvalidOperationException>();
        }
    }
}