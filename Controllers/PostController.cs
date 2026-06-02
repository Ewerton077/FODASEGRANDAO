using backendconfigconecta.Data;
using backendconfigconecta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace backendconfigconecta.Controllers;

[Authorize(Roles = "Aluno,Professor,Admin")]
public class PostController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PostController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var posts = _context.Posts
            .OrderByDescending(p => p.DataCriacao)
            .Take(20)
            .ToList();
        return View(posts);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PostViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("Aluno"))
        {
            var postCount = _context.Posts.Count(p => p.UsuarioId == user.Id);
            if (postCount >= 2)
            {
                ModelState.AddModelError("", "Você atingiu o limite máximo de 2 posts.");
                return View(model);
            }
        }

        var post = new Post
        {
            Titulo = model.Titulo,
            Descricao = model.Descricao,
            UsuarioId = user.Id,
            DataCriacao = DateTime.Now
        };

        if (model.Imagem != null && model.Imagem.Length > 0)
        {
            var maxFileSize = 5 * 1024 * 1024;
            if (model.Imagem.Length > maxFileSize)
            {
                ModelState.AddModelError("Imagem", "A imagem deve ter no máximo 5MB.");
                return View(model);
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(model.Imagem.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("Imagem", "Formato de imagem inválido. Use JPG, PNG ou GIF.");
                return View(model);
            }

            using var memoryStream = new MemoryStream();
            await model.Imagem.CopyToAsync(memoryStream);
            post.ImagemData = memoryStream.ToArray();
            post.ImagemTipo = model.Imagem.ContentType;
        }

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        if (roles.Contains("Aluno"))
            return RedirectToAction("Index", "Aluno");
        return RedirectToAction("Index", "Professor");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var post = await _context.Posts.FindAsync(id);
        if (post == null) return NotFound();

        if (post.UsuarioId != user.Id)
            return Forbid();

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Aluno");
    }
}

public class PostViewModel
{
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "O título é obrigatório")]
    [System.ComponentModel.DataAnnotations.StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "A descrição é obrigatória")]
    [System.ComponentModel.DataAnnotations.StringLength(2000, ErrorMessage = "Máximo 2000 caracteres")]
    public string Descricao { get; set; } = string.Empty;

    public IFormFile? Imagem { get; set; }
}