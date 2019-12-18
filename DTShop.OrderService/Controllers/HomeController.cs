using Microsoft.AspNetCore.Mvc;

namespace DTShop.OrderService.Controllers
{
    [Route("/")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("info")]
        public ActionResult GetInfo()
        {
            return Ok("Order Service for DTShop application.");
        }

        [HttpGet("Status")]
        public ActionResult GetStatus()
        {
            return Ok(1);
        }

        [HttpGet("healthcheck")]
        public ActionResult CheckHealth()
        {
            return Ok("Order Service is running normally.");
        }
    }
}
