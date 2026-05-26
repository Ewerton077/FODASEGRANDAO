using backendconfigconecta.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace backendconfigconecta.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager; //private faz com que śo possa ser usado dentro dessa classe, nada mais que isso
    //readonly define que os valores um só vez, não podendo ser alterado;
    //o _userManager é o nome da variavel, usando _ pra evitar conflito;
    private readonly SignInManager<ApplicationUser> _signInManager;
    
    //cuidando da parte de login de usuario, gerenciando usuario e log in
    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();  
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Email ou senha incorretos.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // Busca roles uma vez e verifica
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Aluno"))
                return RedirectToAction("Index", "Aluno");
            if (roles.Contains("Professor") || roles.Contains("Admin"))
                return RedirectToAction("Index", "Professor");
        }

        ModelState.AddModelError(string.Empty, "Email ou senha incorretos.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    public IActionResult AccessDenied() => View();
}

public class LoginViewModel
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Lembrar-me")]
    public bool RememberMe { get; set; }
}