using Impressao.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Impressao.Controllers
{
    public class UsuarioController : Controller
    {
        // GET: UsuarioController
        public ActionResult Index()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_usuarios"))
            {
                DataContext context = new();
                List<Usuario> usuarios = context.Usuario.ToList();

                if (TempData["success"] != null)
                {
                    ViewBag.Success = TempData["success"];
                    TempData["success"] = null;
                }

                return View(usuarios);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public JsonResult ValidaId(int id)
        {
            DataContext context = new();
            Usuario usuario = context.Usuario.Find(id);

            return Json(new { valid = usuario == null });
        }

        // GET: UsuarioController/Create
        public ActionResult Create()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_usuarios"))
            {
                Usuario usuario = new()
                {
                    Ativo = true
                };

                CarregaPerfis(usuario.Id);

                return View(usuario);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario usuario)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_usuarios"))
            {
                try
                {
                    if (
                        !string.IsNullOrEmpty(usuario.Nome) &&
                        !string.IsNullOrEmpty(usuario.Email) &&
                        !string.IsNullOrEmpty(usuario.Login)
                        )
                    {
                        DataContext context = new();

                        context.Usuario.Add(usuario);
                        context.Entry(usuario).State = EntityState.Added;
                        context.SaveChanges();

                        SalvaPerfis(usuario);

                        TempData["success"] = "Usuário salvo com sucesso!";

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return View();
                    }
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: UsuarioController/Edit/5
        public ActionResult Edit(int? id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_usuarios"))
            {
                if (id != null)
                {
                    DataContext context = new();
                    Usuario usuario = context.Usuario.Find(id);

                    CarregaPerfis(usuario.Id);

                    return View(usuario);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Usuario usuario)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_usuarios"))
            {
                try
                {
                    DataContext context = new();

                    var user = context.Usuario.Find(usuario.Id);
                    user.Nome = usuario.Nome;
                    user.Email = usuario.Email;
                    user.Login = usuario.Login;
                    user.Ativo = usuario.Ativo;

                    context.Usuario.Update(user);
                    context.Entry(user).State = EntityState.Modified;
                    context.SaveChanges();

                    SalvaPerfis(usuario);

                    TempData["success"] = "Usuário salvo com sucesso!";

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        private void CarregaPerfis(int id)
        {
            DataContext context = new();

            List<Perfil> perfis = context.Perfil.OrderBy(p => p.Id).ToList();

            foreach (var perfil in perfis)
            {
                var result = context.UsuarioPerfil.Where(u => u.UsuarioId == id && u.PerfilId == perfil.Id).ToList();
                if (result.Count > 0)
                {
                    var usuarioPerfil = result.First();
                    perfil.UsuarioPerfis.Add(usuarioPerfil);
                }
            }

            ViewBag.perfis = perfis;
        }

        private void SalvaPerfis(Usuario usuario)
        {
            DataContext context = new();

            string perfis = Request.Form["usuarioPerfis"];
            string[] ids = perfis.Split(',');
            if (perfis != "")
            {
                foreach (string id in ids)
                {
                    var result = context.UsuarioPerfil.Where(u => u.UsuarioId == usuario.Id && u.PerfilId == Convert.ToInt32(id)).ToList();
                    if (result.Count == 0)
                    {
                        var usuarioPerfil = new UsuarioPerfil
                        {
                            UsuarioId = usuario.Id,
                            PerfilId = Convert.ToInt32(id)
                        };

                        context.UsuarioPerfil.Add(usuarioPerfil);
                        context.Entry(usuarioPerfil).State = EntityState.Added;
                        context.SaveChanges();
                    }
                }
            }

            var perfilList = context.UsuarioPerfil.Where(u => u.UsuarioId == usuario.Id).ToList();
            foreach (var r in perfilList)
            {
                if (!ids.Contains(r.Id.ToString()))
                {
                    context.UsuarioPerfil.Remove(r);
                    context.Entry(r).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
        }

        // GET: UsuarioController/Password/5
        public ActionResult Password(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("alt_senha_usuarios"))
            {
                try
                {
                    DataContext context = new();
                    Usuario usuario = context.Usuario.Find(id);
                    return View(usuario);
                }
                catch
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: UsuarioController/Password/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Password(Usuario param)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("alt_senha_usuarios"))
            {
                try
                {
                    DataContext context = new();
                    Usuario usuario = context.Usuario.Find(param.Id);
                    usuario.Senha = BCrypt.Net.BCrypt.HashPassword(param.Senha);
                    context.Usuario.Update(usuario);
                    context.Entry(usuario).State = EntityState.Modified;
                
                    context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: UsuarioController/Delete/5
        public ActionResult Delete(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_usuarios"))
            {
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: UsuarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_usuarios"))
            {
                try
                {
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
