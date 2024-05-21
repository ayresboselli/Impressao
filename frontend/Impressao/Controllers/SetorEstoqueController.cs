using Impressao.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Impressao.Controllers
{
    public class SetorEstoqueController : Controller
    {
        // GET: SetorEstoqueController
        public ActionResult Index()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("ver_setores"))
            {
                DataContext context = new();
                List<SetorEstoque> setores = context.SetorEstoque.ToList();
                for (int i = 0; i < setores.Count; i++)
                {
                    setores[i].MateriasPrima = context.MateriaPrima.Where(t => t.IdSetorEstoque == setores[i].Id).ToList();
                }

                if (TempData["success"] != null)
                {
                    ViewBag.Success = TempData["success"];
                    TempData["success"] = null;
                }

                return View(setores);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public JsonResult ValidaId(int id)
        {
            DataContext context = new();
            SetorEstoque setor = context.SetorEstoque.Find(id);

            return Json(new { valid = setor == null });
        }

        // GET: SetorEstoqueController/Create
        public ActionResult Create()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_setores"))
            {
                return View(new SetorEstoque { Ativo = true });
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: SetorEstoqueController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SetorEstoque setor)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_setores"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(setor.Titulo))
                    {
                        DataContext context = new();

                        context.SetorEstoque.Add(setor);
                        context.Entry(setor).State = EntityState.Added;
                        context.SaveChanges();

                        TempData["success"] = "Setor salvo com sucesso!";

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

        // GET: SetorEstoqueController/Edit/5
        public ActionResult Edit(int? id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_setores"))
            {
                if (id != null)
                {
                    DataContext context = new();
                    var setor = context.SetorEstoque.Find(id);

                    return View(setor);
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

        // POST: SetorEstoqueController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SetorEstoque setor)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_setores"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(setor.Titulo))
                    {
                        DataContext context = new();
                        context.SetorEstoque.Update(setor);
                        context.Entry(setor).State = EntityState.Modified;
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

        // POST: SetorEstoque/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_setores"))
            {
                try
                {
                    DataContext context = new();
                    SetorEstoque setor = context.SetorEstoque.Find(id);
                    if (setor != null)
                    {
                        context.SetorEstoque.Remove(setor);
                        context.SaveChanges();
                        return Json(new { success = true });
                    }
                    return Json(new { success = false, msg = "Setor não encontrado" });
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
