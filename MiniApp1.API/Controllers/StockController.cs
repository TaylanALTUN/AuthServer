using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace MiniApp1.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        [Authorize(Roles = "admin",Policy = "IstanbulPolicy")]
        [HttpGet]
        public IActionResult GetStock()
        {
            var userName= HttpContext.User.Identity.Name;
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return Ok($"Stock => UserName : {userName} - USerId : {userIdClaim.Value}");
        }
    }
}
