using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.Api.Controllers
{

    [ApiController]
    [Route("/")]
    public class DefaultController
    {
        private readonly ILogger<DefaultController> _logger;

        public DefaultController(ILogger<DefaultController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get() 
        {
            return "Running....";
        }
    }
}
