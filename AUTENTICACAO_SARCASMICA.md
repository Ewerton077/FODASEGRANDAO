# Como a Autenticação Funciona (Explicação Sarcástica)

## TL;DR
O usuário digita senha → o sistema verifica se ele é quem diz ser → decide se deixa entrar ou não.

---

## As Camadas de Segurança (do mais preguiçoso ao mais neurótico)

### 1. **Identity (Guarda do Predio)**
```
"Você é o João?"
"Sim, sou eu."
"Ok, pode entrar." ✅
```
- Gerencia usuários e senhas
- Faz login/logout
- Guarda suas credenciais com hash (senha criptografada)
- Não confia em ninguém, verifica TUDO

### 2. **Roles (porteiro que só libera andares específicos)**
```
"Quero ir no 5° andar."
"Você tem chave do 5° andar?" → [Authorize(Roles = "Aluno")]
"Tenho."
"Ok, pode passar." ✅
```
- Sem role = sem acesso
- Roles são criadas no banco (tabela `AspNetRoles`)
-绑定 ao usuário na tabela `AspNetUserRoles`

### 3. **[Authorize] (o Security Guard com raiva)**
```csharp
[Authorize(Roles = "Professor,Admin")]  // "Tchau, volte amanhã."
public class Professor : Controller { }
```
- Decorador que bloqueia endpoints
- Funciona assim: ninguém acessa → Identity verifica login → verifica role → acesso ou não

### 4. **AppDbContext (o arquivo morto)**
```csharp
: IdentityDbContext<ApplicationUser>  // Herda tabelas de autenticação
```
- Contém Users, Roles, UserRoles, Claims, Logins
- Você não vê, mas tá lá

---

## Fluxo de um Login (passo a passo)

```
1. Usuário abre site → Vê página de login
2. Digita email + senha → Clica em "Entrar"
3. AccountController.GetLogin() → Exibe formulário
4. Usuário submete → AccountController.PostLogin()
5. UserManager.FindByEmailAsync() → "Quem é esse cara?"
6. SignInManager.PasswordSignInAsync() → "A senha tá correta?"
7. Se OK → IsInRoleAsync() → "Ele pode acessar isso?"
8. Se puder → RedirectToAction() → 🚪 Entrou!
9. Se não puder → AccessDenied() → 🚫 "Volte pra casa."
```

---

## Por que o [Authorize] quebrava antes?

```csharp
// ANTES (não funcionava):
[Authorize(Roles = "Aluno")]
// ↓ Mas o banco não tinha nenhuma role "Aluno" linked ao usuário
// ↓ Resultado: 403 Forbidden ××× MORTE ×××
```

**Solução:** SeedData agora faz:
```csharp
await userManager.AddToRoleAsync(usuario, "Aluno");
// ↑ "OK, ele PODE ser aluno, vou liberar."
```

---

## Roles Disponíveis

| Role | Quem acessa |
|------|-------------|
| `Aluno` | AlunoController |
| `Professor` | ProfessorController |
| `Admin` | ProfessorController + tudo |

---

##常识 (Bônus - Coisas que você deveria saber)

1. **Roles são case-sensitive** - `"Aluno"` ≠ `"aluno"`
2. **Perfil ≠ Role** - Perfil é enum customizado, Role é do Identity
3. **Senhas são hasheadas** - Se o banco vazar, a senha não aparece (quase)
4. **SeedData roda na inicialização** - Adiciona roles a cada restart

---

## Resumo Final

```
┌─────────────────────────────────────────┐
│  Usuário quer acessar /Aluno            │
│         ↓                               │
│  [Authorize(Roles = "Aluno")]           │
│         ↓                               │
│  Identity: "Tá logado?"                │
│         ↓                               │
│  RoleManager: "Tem role Aluno?"        │
│         ↓                               │
│  Sim → Entrou! ✅                       │
│  Não → AccessDenied 🚫                 │
└─────────────────────────────────────────┘
```

Fim. Não foi tão difícil, foi?
