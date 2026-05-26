using backendconfigconecta.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backendconfigconecta.Controllers;

[Authorize(Roles = "Aluno")]
public class AlunoController : Controller
{
    private readonly AppDbContext _context;

    public AlunoController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index() => View();
    
    public IActionResult Posts()
    {
        var posts = _context.Posts
            .OrderByDescending(p => p.DataCriacao)
            .Take(10)
            .ToList();
        return View(posts);
    }
}