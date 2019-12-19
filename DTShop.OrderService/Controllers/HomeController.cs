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
    }
}
