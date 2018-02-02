using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Slack.Integration.Controllers
{
    public class WebHookController: Controller
    {
        [HttpPost("v1/api/github")]
        public IActionResult IncomingHook([FromBody] string content)
        {
            var tempFile = System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllText(content, tempFile);
            Console.WriteLine(tempFile);
            return Ok();
        }
    }
}