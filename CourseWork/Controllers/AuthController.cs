using InventoryManagement.Models.DTO;
using InventoryManagement.Services;
using InventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CourseWork.Controllers
{
    [Route("api/[controller]")]
    [EnableRateLimiting("CommonPolicy")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDTO request)
        {
            var result = _authService.Register(request);
            if (!result.Success)
                return BadRequest(result.Error);

            return Ok(new { message = "Registered successfully." });
        }

        [HttpPost("signin")]
        public IActionResult Signin(LoginDTO request)
        {
            var result = _authService.Signin(request);
            if (!result.Success)
                return Unauthorized(result.Error);

            return Ok(new { token = result.Token });
        }
    }
}
