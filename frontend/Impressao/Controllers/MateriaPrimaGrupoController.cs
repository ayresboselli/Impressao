using Impressao.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Impressao.Controllers
{
    public class MateriaPrimaGrupoController : Controller
    {
        // GET: MateriaPrimaGrupoController
        public ActionResult Index()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_mat_prima_grupos"))
            {
                DataContext context = new();
                List<MateriaPrimaGrupo> grupo = context.MateriaPrimaGrupo.ToList();
                for (int i = 0; i < grupo.Count; i++)
                {
                    grupo[i].MateriasPrima = context.MateriaPrima.Where(t => t.IdGrupo == grupo[i].Id).ToList();
                }

                if (TempData["success"] != null)
                {
                    ViewBag.Success = TempData["success"];
                    TempData["success"] = null;
                }

                return View(grupo);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public JsonResult ValidaId(int id)
        {
            DataContext context = new();
            MateriaPrimaGrupo grupo = context.MateriaPrimaGrupo.Find(id);

            return Json(new { valid = grupo == null });
        }

        // GET: MateriaPrimaGrupoController/Create
        public ActionResult Create()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_mat_prima_grupos"))
            {
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: MateriaPrimaGrupoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MateriaPrimaGrupo grupo)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_mat_prima_grupos"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(grupo.Titulo))
                    {
                        DataContext context = new();

                        context.MateriaPrimaGrupo.Add(grupo);
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

        // GET: MateriaPrimaGrupoController/Edit/5
        public ActionResult Edit(int? id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_mat_prima_grupos"))
            {
                if (id != null)
                {
                    DataContext context = new();
                    var grupo = context.MateriaPrimaGrupo.Find(id);

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

        // POST: MateriaPrimaGrupoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MateriaPrimaGrupo grupo)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_mat_prima_grupos"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(grupo.Titulo))
                    {
                        DataContext context = new();

                        context.MateriaPrimaGrupo.Update(grupo);
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

        // POST: MateriaPrimaGrupoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_mat_prima_grupos"))
            {
                try
                {
                    DataContext context = new();
                    MateriaPrimaGrupo grupo = context.MateriaPrimaGrupo.Find(id);
                    if (grupo != null)
                    {
                        context.MateriaPrimaGrupo.Remove(grupo);
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
