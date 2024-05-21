using Impressao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Management;
using System.Text;
using System.Security.Cryptography;

namespace Impressao.Controllers
{
    public class AuthController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Usuario param)
        {
            DataContext context = new();
            var usuario = context.Usuario.Where(u => u.Login == param.Login).FirstOrDefault();

            if (usuario != null && BCrypt.Net.BCrypt.Verify(param.Senha, usuario.Senha))
            {
                var func = context.UsuarioPerfil
                    .Join(
                        context.PerfilFuncao,
                        up => up.PerfilId,
                        pf => pf.PerfilId,
                        (up, pf) => new
                        {
                            pf.FuncaoId
                        }
                    ).Join(
                        context.Funcao,
                        pf => pf.FuncaoId,
                        f => f.Id,
                        (pf, f) => new
                        {
                            f.Chave
                        }
                    ).ToList();

                List<string> funcoes = new();
                foreach (var f in func)
                {
                    funcoes.Add(f.Chave);
                }


                HttpContext.Session.SetString("User Id", usuario.Id.ToString());
                HttpContext.Session.SetString("User Name", usuario.Nome);
                HttpContext.Session.SetString("Functions", String.Join(",", funcoes.ToArray()));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Usuário ou senha incorretos";
                return View();
            }
        }

        // GET: AuthController
        public ActionResult Logout()
        {
            HttpContext.Session.Remove("User Id");
            HttpContext.Session.Remove("User Name");

            return RedirectToAction("Login", "Auth");
        }

        // POST: AuthController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ChangePassword()
        {
            try
            {
                DataContext context = new();
                var usuario = context.Usuario.Find(Convert.ToInt32(HttpContext.Session.GetString("User Id")));
                if(usuario != null)
                {
                    // Verify the old password
                    if(BCrypt.Net.BCrypt.Verify(Request.Form["senhaA"].ToString(), usuario.Senha))
                    {
                        // Update password
                        usuario.Senha = BCrypt.Net.BCrypt.HashPassword(Request.Form["senhaN"].ToString());

                        context.Usuario.Update(usuario);
                        context.Entry(usuario).State = EntityState.Modified;
                        context.SaveChanges();

                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, msg = "A senha antiga não confere", errorCode = 2 });
                    }
                }
                else
                {
                    return Json(new { success = false, msg = "Usuário não encontrado", errorCode = 1 });
                }
            }
            catch(Exception e)
            {
                return Json(new { success = false, msg = e.Message });
            }
        }

        // GET: AuthController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AuthController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: AuthController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AuthController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
    }
}
