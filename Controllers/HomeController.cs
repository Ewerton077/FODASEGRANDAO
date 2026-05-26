using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using backendconfigconecta.Models;

namespace backendconfigconecta.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // 🔹 GET: /
        public IActionResult Index()
        {
            return View();
        }

        // 🔹 POST: /Home/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Preencha todos os campos");
                return View("Index");
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ModelState.AddModelError("", "Usuário não encontrado");
                return View("Index");
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Senha inválida");
                return View("Index");
            }

            // 🔥 REDIRECIONAMENTO POR ROLE (otimizado - busca roles uma vez)
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Aluno"))
                return RedirectToAction("Index", "Aluno");

            if (roles.Contains("Professor") || roles.Contains("Admin"))
                return RedirectToAction("Index", "Professor");

            // fallback
            return RedirectToAction("Index", "Home");
        }

        // 🔹 GET: /Home/Logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        // 🔹 Página de erro padrão
        public IActionResult Error()
        {
            return View();
        }
    }
}