using Microsoft.AspNetCore.Mvc;

namespace UninetWebApi2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnvController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public EnvController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var envName = _env.EnvironmentName;
            var customerApi = _configuration["ExternalServices:CustomerApi:BaseUrl"];
            return Ok(new { Environment = envName, CustomerApiUrl = customerApi });
        }
    }

}
