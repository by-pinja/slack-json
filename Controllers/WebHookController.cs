using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Slack.Integration.Controllers
{
    public class WebHookController: Controller
    {
        [HttpPost("v1/api/github")]
        public IActionResult IncomingHook([FromBody] JObject content)
        {
            var tempFile = System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllText(tempFile, content.ToString());
            Console.WriteLine(tempFile);
            return Ok();
        }
    }
}