using Microsoft.AspNetCore.Mvc;

namespace llmChat.Controllers
{
    [ApiController]
    [Route("/helloWorld")]
    public class HelloWorldController : ControllerBase
    {

        [HttpGet]
        public ActionResult<string> Get()
        {
            return Ok("Hello, World! qwe");
        }
    }
}
