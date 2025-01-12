using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace llmChat.Controllers
{
    [ApiController]
    [Route("/helloWorld")]
    public class HelloWorldController : ControllerBase
    {


        public HelloWorldController()
        {

        }

        [HttpGet]
        [Authorize]
        public ActionResult<string> Get()
        {
            return Ok("Hello, World! qwe");
        }
    }
}
