using Impressao.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Impressao.Controllers
{
    public class ClienteGrupoController : Controller
    {
        // GET: ClienteGrupoController
        public ActionResult Index()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_cliente_grupos"))
            {
                DataContext context = new();
                List<ClienteGrupo> grupos = context.ClienteGrupo.ToList();
                for (int i = 0; i < grupos.Count; i++)
                {
                    grupos[i].Clientes = context.Cliente.Where(c => c.ClienteGrupoId == grupos[i].Id).ToList();
                }

                if (TempData["success"] != null)
                {
                    ViewBag.Success = TempData["success"];
                    TempData["success"] = null;
                }

                return View(grupos);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public JsonResult ValidaId(int id)
        {
            DataContext context = new();
            ClienteGrupo grupo = context.ClienteGrupo.Find(id);

            return Json(new { valid = grupo == null });
        }

        // GET: ClienteGrupoController/Create
        public ActionResult Create()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_cliente_grupos"))
            {
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ClienteGrupoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClienteGrupo grupo)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_cliente_grupos"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(grupo.Titulo))
                    {
                        DataContext context = new();

                        context.ClienteGrupo.Add(grupo);
                        context.Entry(grupo).State = EntityState.Added;
                        context.SaveChanges();

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

        // GET: ClienteGrupoController/Edit/5
        public ActionResult Edit(int? id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_cliente_grupos"))
            {
                if (id != null)
                {
                    DataContext context = new();
                    var grupo = context.ClienteGrupo.Find(id);

                    return View(grupo);
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

        // POST: ClienteGrupoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClienteGrupo grupo)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_cliente_grupos"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(grupo.Titulo))
                    {
                        DataContext context = new();

                        context.ClienteGrupo.Update(grupo);
                        context.Entry(grupo).State = EntityState.Modified;
                        context.SaveChanges();

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

        // POST: ClienteGrupoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_cliente_grupos"))
            {
                try
                {
                    DataContext context = new();
                    ClienteGrupo grupo = context.ClienteGrupo.Find(id);
                    if (grupo != null)
                    {
                        context.ClienteGrupo.Remove(grupo);
                        context.SaveChanges();
                        return Json(new { success = true });
                    }
                    return Json(new { success = false, msg = "Grupo não encontrado" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, msg = ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, msg = "Acesso negado" });
            }
        }
    }
}
