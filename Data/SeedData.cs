using backendconfigconecta.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backendconfigconecta.Data;

public static class SeedData
{
    private static readonly string[] SystemRoles = { "Aluno", "Professor", "Admin" };

    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole>? roleManager = null, AppDbContext? context = null)
    {
        if (roleManager != null)
        {
            foreach (var role in SystemRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        #region Aluno 1
        var aluno1 = await userManager.FindByEmailAsync("aluno1@teste.com");
        if (aluno1 == null)
        {
            aluno1 = new ApplicationUser
            {
                UserName = "aluno1@teste.com",
                Email = "aluno1@teste.com",
                Nome = "João Silva",
                Perfil = Perfil.Aluno
            };
            await userManager.CreateAsync(aluno1, "Aluno@123");
        }
        if (!await userManager.IsInRoleAsync(aluno1, "Aluno"))
            await userManager.AddToRoleAsync(aluno1, "Aluno");
        #endregion

        #region Aluno 2
        var aluno2 = await userManager.FindByEmailAsync("aluno2@teste.com");
        if (aluno2 == null)
        {
            aluno2 = new ApplicationUser
            {
                UserName = "aluno2@teste.com",
                Email = "aluno2@teste.com",
                Nome = "Maria Oliveira",
                Perfil = Perfil.Aluno
            };
            await userManager.CreateAsync(aluno2, "Aluno@123");
        }
        if (!await userManager.IsInRoleAsync(aluno2, "Aluno"))
            await userManager.AddToRoleAsync(aluno2, "Aluno");
        #endregion

        #region Aluno 3
        var aluno3 = await userManager.FindByEmailAsync("aluno3@teste.com");
        if (aluno3 == null)
        {
            aluno3 = new ApplicationUser
            {
                UserName = "aluno3@teste.com",
                Email = "aluno3@teste.com",
                Nome = "Pedro Santos",
                Perfil = Perfil.Aluno
            };
            await userManager.CreateAsync(aluno3, "Aluno@123");
        }
        if (!await userManager.IsInRoleAsync(aluno3, "Aluno"))
            await userManager.AddToRoleAsync(aluno3, "Aluno");
        #endregion

        #region Professor 1
        var professor1 = await userManager.FindByEmailAsync("professor1@teste.com");
        if (professor1 == null)
        {
            professor1 = new ApplicationUser
            {
                UserName = "professor1@teste.com",
                Email = "professor1@teste.com",
                Nome = "Carlos Mendes",
                Perfil = Perfil.Professor
            };
            await userManager.CreateAsync(professor1, "Professor@123");
        }
        if (!await userManager.IsInRoleAsync(professor1, "Professor"))
            await userManager.AddToRoleAsync(professor1, "Professor");
        #endregion

        #region Professor 2
        var professor2 = await userManager.FindByEmailAsync("professor2@teste.com");
        if (professor2 == null)
        {
            professor2 = new ApplicationUser
            {
                UserName = "professor2@teste.com",
                Email = "professor2@teste.com",
                Nome = "Ana Costa",
                Perfil = Perfil.Professor
            };
            await userManager.CreateAsync(professor2, "Professor@123");
        }
        if (!await userManager.IsInRoleAsync(professor2, "Professor"))
            await userManager.AddToRoleAsync(professor2, "Professor");
        #endregion

        #region Professor 3
        var professor3 = await userManager.FindByEmailAsync("professor3@teste.com");
        if (professor3 == null)
        {
            professor3 = new ApplicationUser
            {
                UserName = "professor3@teste.com",
                Email = "professor3@teste.com",
                Nome = "Roberto Lima",
                Perfil = Perfil.Professor
            };
            await userManager.CreateAsync(professor3, "Professor@123");
        }
        if (!await userManager.IsInRoleAsync(professor3, "Professor"))
            await userManager.AddToRoleAsync(professor3, "Professor");
        #endregion

        // Load seed images from wwwroot
        var seedDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css", "imagens", "seed");
        var seedImages = new List<(byte[] data, string tipo)>();
        if (Directory.Exists(seedDir))
        {
            foreach (var f in Directory.GetFiles(seedDir, "seed_img_*.jpg").OrderBy(f => f))
            {
                seedImages.Add((File.ReadAllBytes(f), "image/jpeg"));
            }
        }

        // Add sample posts for institutional area
        if (context != null)
        {
            if (!context.Posts.Any())
            {
                var posts = new List<Post>
                {
                    new Post
                    {
                        Titulo = "Conecta Talk com profissional de TI: carreira, mercado e dicas",
                        Descricao = "A turma recebeu um profissional da área de desenvolvimento de software para uma tarde de troca intensa. Foram abordados temas como portfólio, primeiros empregos, soft skills e muito mais.",
                        UsuarioId = professor1.Id,
                        DataCriacao = DateTime.Now.AddDays(-2)
                    },
                    new Post
                    {
                        Titulo = "Módulo 4 — Banco de dados relacional: primeira query dos alunos",
                        Descricao = "Hoje iniciamos o módulo de banco de dados. Os alunos criaram suas primeiras queries e aprenderam sobre normalização e chaves estrangeiras.",
                        UsuarioId = professor2.Id,
                        DataCriacao = DateTime.Now.AddDays(-5)
                    },
                    new Post
                    {
                        Titulo = "Visita à empresa parceira: como é a rotina de um analista",
                        Descricao = "A turma visitou a TechSolutions e pôde conhecer a rotina real de um analista de sistemas. Foram muitas descobertas!",
                        UsuarioId = professor1.Id,
                        DataCriacao = DateTime.Now.AddDays(-10)
                    },
                    new Post
                    {
                        Titulo = "Semana da Cooperação — alunos participam de painel sobre ESG",
                        Descricao = "Eventos e palestras sobre cooperativismo, sustentabilidade e responsabilidade social. Nossos alunos estiveram presentes!",
                        UsuarioId = professor3.Id,
                        DataCriacao = DateTime.Now.AddDays(-15)
                    },
                    new Post
                    {
                        Titulo = "O programa mudou minha perspectiva — depoimento da egressa",
                        Descricao = "Depoimento inspirador Fernanda Nunes, da turma 01. Sua trajetória do Conecta para o mercado de trabalho é incrível!",
                        UsuarioId = aluno1.Id,
                        DataCriacao = DateTime.Now.AddDays(-20)
                    }
                };

                for (int i = 0; i < posts.Count && i < seedImages.Count; i++)
                {
                    posts[i].ImagemData = seedImages[i].data;
                    posts[i].ImagemTipo = seedImages[i].tipo;
                }

                context.Posts.AddRange(posts);
                await context.SaveChangesAsync();
            }
            else
            {
                // Fill in images for existing posts that don't have one
                var postsSemImagem = await context.Posts.Where(p => p.ImagemData == null).OrderBy(p => p.DataCriacao).ToListAsync();
                for (int i = 0; i < postsSemImagem.Count && i < seedImages.Count; i++)
                {
                    postsSemImagem[i].ImagemData = seedImages[i].data;
                    postsSemImagem[i].ImagemTipo = seedImages[i].tipo;
                }
                if (postsSemImagem.Count > 0)
                    await context.SaveChangesAsync();
            }
        }
    }
}