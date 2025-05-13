using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SupervisorBravo.Web.Controllers.Authentication
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Index(string username, string password)
        {
            // Usuarios fijos
            string role = username switch
            {
                "admin" when password == "admin123" => "Admin",
                "tecnico" when password == "tecnico123" => "Tecnico",
                "visitante" when password == "visitante123" => "Visitante",
                _ => null
            };

            if (role == null)
            {
                ViewBag.Error = "Credenciales inválidas";
                return View();
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

            var identity = new ClaimsIdentity(claims, "MiCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MiCookieAuth", principal);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MiCookieAuth");
            return RedirectToAction("Index");
        }
    }
}
