using Microsoft.AspNetCore.Identity;

namespace backendconfigconecta.Models
{
    // IdentityUser já tem Email, PasswordHash, etc. (tabela AspNetUsers)
    public class ApplicationUser : IdentityUser
    {
       public string Nome { get; set; } = string.Empty;
       // Campo customizado - não confundi com Roles do Identity
       public Perfil Perfil { get; set; }
       public string Tipo { get; set; } = string.Empty;
    }
}