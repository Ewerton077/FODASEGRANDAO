using backendconfigconecta.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backendconfigconecta.Controllers;

public class AreaAlunoController : Controller
{
    private readonly AppDbContext _context;

    public AreaAlunoController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _context.Posts
            .Include(p => p.Usuario)
            .OrderByDescending(p => p.DataCriacao)
            .Take(6)
            .ToListAsync();
        return View(posts);
    }
}