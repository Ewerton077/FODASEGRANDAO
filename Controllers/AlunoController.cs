using backendconfigconecta.Data;
using backendconfigconecta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backendconfigconecta.Controllers;

[Authorize(Roles = "Aluno")]
public class AlunoController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AlunoController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> Posts()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var posts = _context.Posts
            .Where(p => p.UsuarioId == user.Id)
            .OrderByDescending(p => p.DataCriacao)
            .ToList();
        return View(posts);
    }
}
