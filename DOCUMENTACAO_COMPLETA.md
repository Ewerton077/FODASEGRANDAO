# DOCUMENTAÇÃO COMPLETA DO PROJETO MFVCT

## Informações Gerais

| Campo | Valor |
|-------|-------|
| **Nome do Projeto** | MFVCT / EduSys |
| **Tipo** | Web Application (ASP.NET Core MVC) |
| **Framework** | .NET 9.0 |
| **Linguagem** | C# 12 |
| **Banco de Dados** | MySQL (via Pomelo EF Core) |
| **Autenticação** | ASP.NET Core Identity |
| **ORM** | Entity Framework Core 9.0 |
| **Frontend** | Razor Views + TailwindCSS |
| **Porta Padrão** | 5180 |
| **Namespace** | `backendconfigconecta` |

---

## Estrutura de Diretórios

```
MFVCT/
├── Controllers/          # Controladores MVC
│   ├── AccountController.cs    # Login/Logout/Autenticação
│   ├── AlunoController.cs      # Painel do Aluno
│   ├── AreaAluno.cs            # Área alternativa do Aluno
│   ├── HomeController.cs       # Página inicial e login antigo
│   └── Professor.cs            # Painel do Professor
├── Data/                # Camada de dados
│   ├── AppDbContext.cs        # Contexto do EF Core
│   └── SeedData.cs            # Seed de usuários e roles
├── Models/              # Modelos de domínio
│   ├── ApplicationUser.cs      # Usuário customizado (herda IdentityUser)
│   ├── Perfil.cs              # Enum de perfis
│   ├── Usuario.cs             # Classe Usuario genérica
│   ├── UpdateRequest.cs       # Requisição de atualização
│   └── ErrorViewModel.cs      # Modelo de erro
├── Views/                # Views Razor
│   ├── Account/Login.cshtml   # Página de login
│   ├── Aluno/Index.cshtml    # Dashboard do Aluno
│   ├── Aluno/Posts.cshtml    # Posts do Aluno
│   ├── Professor/Index.cshtml # Dashboard do Professor
│   ├── AreaAluno/            # Área alternativa
│   └── Shared/_Layout.cshtml  # Layout principal
├── Migrations/           # Migrations do EF Core
├── wwwroot/             # Arquivos estáticos
│   ├── css/geral.css         # CSS customizado
│   ├── css/output.css         # CSS compilado do Tailwind
│   └── lib/                  # Bibliotecas JS (jQuery)
├── Program.cs           # Configuração da aplicação
├── appsettings.json     # Configurações (connection string)
├── backendcodeteste.csproj   # Arquivo do projeto
├── package.json         # Dependências Node.js (Tailwind)
└── MUDANÇAS.md         # Histórico de alterações
```

---

## Pacotes e Dependências

### NuGet Packages (.NET)

| Pacote | Versão | Propósito |
|--------|--------|-----------|
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 9.0.13 | Sistema de autenticação e autorização |
| `Microsoft.EntityFrameworkCore` | 9.0.13 | ORM principal |
| `Microsoft.EntityFrameworkCore.Design` | 9.0.13 | Ferramentas CLI para migrations |
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.13 | Provider SQL Server (não usado) |
| `Microsoft.EntityFrameworkCore.Tools` | 9.0.13 | Comandos PMC (Add-Migration, etc.) |
| `Pomelo.EntityFrameworkCore.MySql` | 9.0.0 | Provider MySQL para EF Core |

### Node.js Packages

| Pacote | Versão | Propósito |
|--------|--------|-----------|
| `tailwindcss` | ^3.4.0 | Framework CSS utility-first |
| `concurrently` | ^8.2.0 | Rodar múltiplos processos simultaneamente |

---

## Banco de Dados

### Connection String
```json
"Server=localhost;Port=3306;Database=conecta_db;Uid=ads_user;Pwd=krakermanfodase;"
```

### Tabelas do Identity (criadas automaticamente)
- `AspNetUsers` - Usuários
- `AspNetRoles` - Roles do sistema
- `AspNetUserRoles` - Relacionamento Users ↔ Roles
- `AspNetUserClaims` - Claims dos usuários
- `AspNetUserLogins` - Logins externos
- `AspNetUserTokens` - Tokens de autenticação
- `AspNetRoleClaims` - Claims das roles

### Tabela Customizada
- `ApplicationUser.perfil` - Enum `Perfil` (Aluno/Professor)

---

## Modelos

### ApplicationUser.cs
```csharp
public class ApplicationUser : IdentityUser
{
    public string Nome { get; set; }
    public Perfil perfil { get; set; }  // Enum customizado
    public string Tipo { get; set; }
}
```
- Herda de `IdentityUser` (já tem Email, PasswordHash, UserName, etc.)
- Adiciona campos customizados

### Perfil.cs
```csharp
public enum Perfil
{
    Aluno,
    Professor
}
```
- Enum para lógica de negócio adicional
- **Diferente das Roles do Identity**

### UpdateRequest.cs
```csharp
public class UpdateRequest
{
    public int Id { get; set; }
    public ApplicationUser User { get; set; }
    public string Campo { get; set; }
    public string NovoValor { get; set; }
    public string UserId { get; set; }
    public bool Aprovado { get; set; }
    public bool Revisado { get; set; }
    public DateTime DataSolicitacao { get; set; }
}
```

---

## Controllers

### AccountController.cs
**Responsabilidade:** Login, Logout, AccessDenied

| Action | Verbo | Descrição |
|--------|-------|-----------|
| `Login` | GET | Exibe formulário de login |
| `Login` | POST | Processa login, verifica roles, redireciona |
| `Logout` | POST | Faz signOut, redireciona para login |
| `AccessDenied` | GET | Página de acesso negado |

**Fluxo de Login:**
1. Recebe email + senha
2. `UserManager.FindByEmailAsync()` - busca usuário
3. `SignInManager.PasswordSignInAsync()` - valida senha
4. `UserManager.IsInRoleAsync()` - verifica role
5. Redireciona para controller correto

### AlunoController.cs
**Responsabilidade:** Painel do aluno

```csharp
[Authorize(Roles = "Aluno")]
public class AlunoController : Controller
{
    public IActionResult Index() => View();  // Dashboard
    public IActionResult Posts() => View();   // Posts
}
```

### Professor.cs
**Responsabilidade:** Painel do professor

```csharp
[Authorize(Roles = "Professor,Admin")]
public class Professor : Controller
{
    public IActionResult Index() => View();
}
```

### HomeController.cs
**Responsabilidade:** Página inicial (legacy)

| Action | Descrição |
|--------|-----------|
| `Index` | Página inicial |
| `Login` | Login legacy (usa roles minúsculas) |
| `Logout` | Logout legacy |
| `Error` | Página de erro |

### AreaAluno.cs
**Responsabilidade:** Área alternativa do aluno (sem Authorize)

---

## Sistema de Autenticação e Autorização

### Roles do Sistema
```
Aluno     → Accesso a AlunoController
Professor → Acesso a ProfessorController
Admin     → Acesso a ProfessorController (e outros)
```

### Como o [Authorize] funciona
```
1. Usuário tenta acessar /Aluno
2. [Authorize(Roles = "Aluno")] intercepta
3. Identity verifica se está logado
4. Identity verifica se tem role "Aluno"
5. Se sim → permite acesso
   Se não → redireciona para AccessDenied
```

### Configuração no Program.cs
```csharp
// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// RoleManager (necessário para criar roles)
builder.Services.AddScoped<RoleManager<IdentityRole>>();
```

---

## SeedData

### O que faz
1. Cria as roles se não existirem
2. Cria usuários de teste se não existirem
3. Atribui roles aos usuários

### Usuários de Teste
| Email | Senha | Perfil | Role |
|-------|-------|--------|------|
| aluno@teste.com | Aluno@123 | Aluno | Aluno |
| professor@teste.com | Professor@123 | Professor | Professor |

### Código Principal
```csharp
// Roles
foreach (var role in SystemRoles)
{
    if (!await roleManager.RoleExistsAsync(role))
        await roleManager.CreateAsync(new IdentityRole(role));
}

// Usuário + Role
await userManager.CreateAsync(usuario, "Senha@123");
await userManager.AddToRoleAsync(usuario, "Aluno");
```

---

## Views

### Login (Account/Login.cshtml)
- Formulário com email e senha
- Validação client-side via jQuery
- Checkbox "Lembrar-me"
- Credenciais de teste visíveis

### Dashboard Aluno (Aluno/Index.cshtml)
- Layout próprio (`Layout = null`)
- Sidebar com navegação
- Cards de tarefas
- Biografia editável
- Tags de especialidades
- Barra de progresso de tecnologias
- Modal para adicionar itens

### Dashboard Professor (Professor/Index.cshtml)
- Layout próprio (`Layout = null`)
- Estatísticas (alunos ativos, média geral)
- Lista de turmas
- Agenda do dia
- Comunicados
- Tabela de desempenho de alunos

---

## Middleware (Program.cs)

```csharp
app.UseStaticFiles();     // Arquivos estáticos (wwwroot)
app.UseRouting();         // Roteamento MVC
app.UseAuthentication();  // Autenticação (quem é você?)
app.UseAuthorization();   // Autorização (você pode acessar isso?)
```

### Ordem Importante
```
UseRouting → UseAuthentication → UseAuthorization → MapControllerRoute
```

---

## Rotas

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
```

| URL | Controller | Action |
|-----|------------|--------|
| `/` | Home | Index |
| `/Home/Login` | Home | Login (POST) |
| `/Account/Login` | Account | Login (GET) |
| `/Aluno` | Aluno | Index |
| `/Aluno/Posts` | Aluno | Posts |
| `/Professor` | Professor | Index |

---

## CSS

### geral.css
- CSS customizado com variáveis CSS
- Estilos para sidebar, cards, inputs, buttons
- Animações e transições
- Temas escuros com cor verde (#76C057)

### output.css
- Compilado do TailwindCSS
- Requer `npm run watch:css` para atualizar

---

## Scripts Disponíveis

### npm
```bash
npm run watch:css   # Compila Tailwind em watch mode
npm run build:css    # Compila Tailwind uma vez
npm start            # Roda dotnet + tailwind simultaneamente
```

### dotnet
```bash
dotnet build         # Compila o projeto
dotnet run           # Roda a aplicação
dotnet watch run     # Watch mode
dotnet ef migrations add Nome   # Cria migration
dotnet ef database update       # Aplica migrations
```

---

## Variáveis de Ambiente / Configuração

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=conecta_db;Uid=ads_user;Pwd=krakermanfodase;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

## Fluxo Completo de uma Requisição

```
┌──────────────────────────────────────────────────────────────┐
│  BROWSER                                                      │
│  GET /Aluno                                                   │
└────────────────────────────┬───────────────────────────────────┘
                             ▼
┌──────────────────────────────────────────────────────────────┐
│  KESTREL SERVER (porta 5180)                                  │
└────────────────────────────┬───────────────────────────────────┘
                             ▼
┌──────────────────────────────────────────────────────────────┐
│  MIDDLEWARE PIPELINE                                          │
│  UseStaticFiles → UseRouting → Authentication → Authorization │
└────────────────────────────┬───────────────────────────────────┘
                             ▼
┌──────────────────────────────────────────────────────────────┐
│  AUTORIZAÇÃO                                                  │
│  [Authorize(Roles = "Aluno")]                                 │
│  • Identity verifica se logado                                │
│  • Identity verifica role "Aluno"                             │
└────────────────────────────┬───────────────────────────────────┘
                             ▼
                    ┌────────┴────────┐
                    │  TEM PERMISSÃO  │
                    └────────┬────────┘
                    Sim      │ Não
                      ▼       ▼
              ┌───────────┐  ┌────────────────┐
              │ Controller│  │ AccessDenied() │
              │   Action  │  └────────────────┘
              └─────┬─────┘
                    ▼
              ┌───────────┐
              │ View()    │
              │ Index.cshtml
              └─────┬─────┘
                    ▼
              ┌───────────┐
              │ HTML/CSS/JS
              │ Renderizado
              └───────────┘
```

---

## Comandos Úteis para Manutenção

### Resetar Banco
```bash
# Apagar migrations e recriar
rm -rf Migrations/
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Limpar Cache
```bash
dotnet clean
rm -rf bin/ obj/
dotnet restore
dotnet build
```

### Adicionar Migration
```bash
dotnet ef migrations add NomeDaMigration -c AppDbContext
```

---

## Notas Importantes

1. **Roles são case-sensitive** - `"Aluno"` ≠ `"aluno"`
2. **Perfil ≠ Role** - Perfil é enum, Role é do Identity
3. **Senhas hasheadas** - Nunca salvas em texto plano
4. **SeedData roda no startup** - Recria roles se necessário
5. **Views com `Layout = null`** - Têm CSS próprio, ignoram _Layout.cshtml

---

## Contato / Suporte

Para recriar este projeto em outro ambiente:

1. `git clone`
2. `dotnet restore`
3. `dotnet ef database update`
4. `npm install`
5. `dotnet run`
6. Acessar `http://localhost:5180/Account/Login`
7. Login: `aluno@teste.com` / `Aluno@123`
