using Impressao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Impressao.Controllers
{
    public class CelulaController : Controller
    {
        // GET: CelulaController
        public ActionResult Index()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("ver_celulas"))
            {
                DataContext context = new();
                List<Celula> celulas = context.Celula.ToList();

                for (int i = 0; i < celulas.Count; i++)
                {
                    celulas[i].PedidoItemApontamentos = context.PedidoItemApontamento.Where(c => c.CelulaId == celulas[i].Id).ToList();
                }

                if (TempData["success"] != null)
                {
                    ViewBag.Success = TempData["success"];
                    TempData["success"] = null;
                }

                return View(celulas);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public JsonResult ValidaId(int id)
        {
            DataContext context = new();
            Celula celula = context.Celula.Find(id);

            return Json(new { valid = celula == null });
        }

        // GET: CelulaController/Create
        public ActionResult Create()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_celulas"))
            {
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: CelulaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Celula celula)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_celulas"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(celula.Titulo))
                    {
                        DataContext context = new();

                        context.Celula.Add(celula);
                        context.Entry(celula).State = EntityState.Added;
                        context.SaveChanges();

                        TempData["success"] = "Célula salva com sucesso!";

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

        // GET: CelulaController/Edit/5
        public ActionResult Edit(int? id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_celulas"))
            {
                if (id != null)
                {
                    DataContext context = new();
                    var celula = context.Celula.Find(id);

                    return View(celula);
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

        // POST: CelulaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Celula celula)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_celulas"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(celula.Titulo))
                    {
                        DataContext context = new();
                        context.Celula.Update(celula);
                        context.Entry(celula).State = EntityState.Modified;
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

        // POST: CelulaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_celulas"))
            {
                try
                {
                    DataContext context = new();
                    Celula celula = context.Celula.Find(id);
                    if (celula != null)
                    {
                        context.Celula.Remove(celula);
                        context.SaveChanges();
                        return Json(new { success = true });
                    }
                    return Json(new { success = false, msg = "Célula não encontrada" });
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
