using Microsoft.AspNetCore.Identity;

namespace backendconfigconecta.Models
{
    public class ApplicationUser : IdentityUser
    {
       public string Nome { get; set; } = string.Empty;
       public Perfil Perfil { get; set; }
       public string Tipo { get; set; } = string.Empty;
       public bool Blocked { get; set; }
    }
}