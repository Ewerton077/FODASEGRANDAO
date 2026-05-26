using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace backendconfigconecta.Controllers;

// Restringe acesso a usuários com role "Professor" ou "Admin"
// Sem isso, qualquer usuário logado poderia acessar
[Authorize(Roles = "Professor,Admin")]
public class Professor : Controller
{
      public IActionResult Index() => View();
}