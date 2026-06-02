using backendconfigconecta.Data;
using backendconfigconecta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backendconfigconecta.Controllers;

[Authorize(Roles = "Professor,Admin")]
public class Professor : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public Professor(UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> Students()
    {
        var students = await _userManager.GetUsersInRoleAsync("Aluno");
        var viewModel = students.Select(s => new StudentViewModel
        {
            Id = s.Id,
            Nome = s.Nome,
            Email = s.Email ?? "",
            Blocked = s.Blocked,
            PostCount = _context.Posts.Count(p => p.UsuarioId == s.Id)
        }).ToList();
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult CreateStudent() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateStudent(CreateStudentViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var existing = await _userManager.FindByEmailAsync(model.Email);
        if (existing != null)
        {
            ModelState.AddModelError("", "Já existe um usuário com este email.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            Nome = model.Nome,
            Perfil = Perfil.Aluno
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, "Aluno");
        return RedirectToAction("Students");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteStudent(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            var posts = _context.Posts.Where(p => p.UsuarioId == id);
            _context.Posts.RemoveRange(posts);
            await _context.SaveChangesAsync();
            await _userManager.DeleteAsync(user);
        }
        return RedirectToAction("Students");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleBlock(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            user.Blocked = !user.Blocked;
            await _userManager.UpdateAsync(user);
        }
        return RedirectToAction("Students");
    }
}

public class StudentViewModel
{
    public string Id { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public bool Blocked { get; set; }
    public int PostCount { get; set; }
}

public class CreateStudentViewModel
{
    [System.ComponentModel.DataAnnotations.Required]
    public string Nome { get; set; } = "";

    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.EmailAddress]
    public string Email { get; set; } = "";

    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
    public string Password { get; set; } = "";
}