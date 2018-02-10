using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;

namespace Slack.Json.Util
{
    public static class JsonExtensions
    {
        public static T Get<T>(this JObject json, Func<dynamic, dynamic> selector, [CallerFilePath] string callerFile = "", [CallerLineNumber] int callerLine = 0)
        {
            try
            {
                var result = selector((dynamic)json);

                if(result == null)
                    throw new InvalidOperationException($"Invalid last select from JSON {callerFile}:{callerLine}");

                switch(result)
                {
                    case JObject o:
                        return o.Value<T>();
                    case JToken o:
                        return o.Value<T>();
                    default:
                        throw new InvalidOperationException($"No defined conversion for JSON object {result.GetType()}");
                }
            }
            catch(RuntimeBinderException)
            {
                throw new InvalidOperationException($"Invalid path before last from JSON {callerFile}:{callerLine}");
            }
        }

        public static string Get(this JObject json, Func<dynamic, dynamic> selector, [CallerFilePath] string callerFile = "", [CallerLineNumber] int callerLine = 0)
        {
            return Get<string>(json, selector, callerFile, callerLine);
        }
    }
}