using llmChat.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace llmChat.Controllers
{
    [ApiController]
    [Route("/helloWorld")]
    public class HelloWorldController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;

        public HelloWorldController(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<string> Get()
        {
            return Ok("Hello, World! qwe");
        }
    }
}
