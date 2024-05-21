using Impressao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Impressao.Controllers
{
    public class UnidadeMedidaController : Controller
    {
        public ActionResult Index()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions")!=null?HttpContext.Session.GetString("Functions").Split(","):null;
            if (listaFuncoes!=null && listaFuncoes.Contains("ver_unidades_medida"))
            {
                DataContext context = new();
                List<UnidadeMedida> unidades = context.UnidadeMedida.ToList();

                for (int i = 0; i < unidades.Count; i++)
                {
                    unidades[i].ProdutosEngenharia = context.ProdutoEngenharia.Where(c => c.IdUnidadeMedida == unidades[i].Id).ToList();
                    unidades[i].MateriasPrima = context.MateriaPrima.Where(c => c.IdUnidadeMedida == unidades[i].Id).ToList();
                    unidades[i].UnidadesOriginais = context.UnidadeMedidaConversao.Where(c => c.IdUnidadeOriginal == unidades[i].Id).ToList();
                    unidades[i].UnidadesConvertidas = context.UnidadeMedidaConversao.Where(c => c.IdUnidadeConvertida == unidades[i].Id).ToList();
                }

                if (TempData["success"] != null)
                {
                    ViewBag.Success = TempData["success"];
                    TempData["success"] = null;
                }

                return View(unidades);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public JsonResult ValidaId(int id)
        {
            DataContext context = new();
            UnidadeMedida unidade = context.UnidadeMedida.Find(id);

            return Json(new { valid = unidade == null });
        }

        // GET: UnidadeMedidaController/Create
        public ActionResult Create()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_unidades_medida"))
            {
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: UnidadeMedidaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UnidadeMedida unidade)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_unidades_medida"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(unidade.Titulo))
                    {
                        DataContext context = new();

                        context.UnidadeMedida.Add(unidade);
                        context.Entry(unidade).State = EntityState.Added;
                        context.SaveChanges();

                        TempData["success"] = "Unidade de medida salva com sucesso!";

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

        // GET: UnidadeMedidaController/Edit/5
        public ActionResult Edit(int? id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_unidades_medida"))
            {
                if (id != null)
                {
                    DataContext context = new();
                    var unidade = context.UnidadeMedida.Find(id);

                    return View(unidade);
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

        // POST: UnidadeMedidaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UnidadeMedida unidade)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_unidades_medida"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(unidade.Titulo))
                    {
                        DataContext context = new();
                        context.UnidadeMedida.Update(unidade);
                        context.Entry(unidade).State = EntityState.Modified;
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

        // POST: UnidadeMedidaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_unidades_medida"))
            {
                try
                {
                    DataContext context = new();
                    UnidadeMedida unidade = context.UnidadeMedida.Find(id);
                    if (unidade != null)
                    {
                        context.UnidadeMedida.Remove(unidade);
                        context.SaveChanges();
                        return Json(new { success = true });
                    }
                    return Json(new { success = false, msg = "Unidade de medida não encontrada" });
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
