using Impressao.Models;
using Impressao.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Impressao.Controllers
{
    public class UnidadeMedidaConversaoController : Controller
    {
        // GET: UnidadeMedidaConversaoController
        public ActionResult Index()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("ver_unid_med_conv"))
            {
                DataContext context = new();
                List<UnidadeMedidaConversao> unidades = context.UnidadeMedidaConversao.ToList();

                for (int i = 0; i < unidades.Count; i++)
                {
                    unidades[i].Original = context.UnidadeMedida.Where(c => c.Id == unidades[i].IdUnidadeOriginal).First();
                    unidades[i].Convertida = context.UnidadeMedida.Where(c => c.Id == unidades[i].IdUnidadeConvertida).First();
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
            UnidadeMedidaConversao unidade = context.UnidadeMedidaConversao.Find(id);

            return Json(new { valid = unidade == null });
        }

        // GET: UnidadeMedidaConversaoController/Create
        public ActionResult Create()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_unid_med_conv"))
            {
                DataContext context = new();
                var unidade = new UnidadeMedidaConversaoViewModel
                {
                    UnidadesMedida = context.UnidadeMedida.ToList()
                };

                return View(unidade);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: UnidadeMedidaConversaoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UnidadeMedidaConversaoViewModel unidadeVM)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_unid_med_conv"))
            {
                try
                {
                    var unidade = unidadeVM.Conversao;

                    if (unidade.IdUnidadeOriginal != null && unidade.IdUnidadeConvertida != null)
                    {
                        DataContext context = new();

                        context.UnidadeMedidaConversao.Add(unidade);
                        context.Entry(unidade).State = EntityState.Added;
                        context.SaveChanges();

                        TempData["success"] = "Conversão salva com sucesso!";

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

        // GET: UnidadeMedidaConversaoController/Edit/5
        public ActionResult Edit(int? id)
        {
            DataContext context = new();

            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_unid_med_conv"))
            {
                if (id != null)
                {
                    var unidade = new UnidadeMedidaConversaoViewModel
                    {
                        Conversao = context.UnidadeMedidaConversao.Find(id),
                        UnidadesMedida = context.UnidadeMedida.ToList()
                    };

                    return View(unidade);
                }
                else
                {
                    var unidade = new UnidadeMedidaConversaoViewModel
                    {
                        UnidadesMedida = context.UnidadeMedida.ToList()
                    };

                    return View();
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: UnidadeMedidaConversaoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UnidadeMedidaConversaoViewModel unidadeVM)
        {
            var unidade = unidadeVM.Conversao;

            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_unid_med_conv"))
            {
                try
                {
                    if (unidade.IdUnidadeOriginal != null && unidade.IdUnidadeConvertida != null)
                    {
                        DataContext context = new();
                        context.UnidadeMedidaConversao.Update(unidade);
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

        // POST: UnidadeMedidaConversaoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_unid_med_conv"))
            {
                try
                {
                    DataContext context = new();
                    UnidadeMedidaConversao unidade = context.UnidadeMedidaConversao.Find(id);
                    if (unidade != null)
                    {
                        context.UnidadeMedidaConversao.Remove(unidade);
                        context.SaveChanges();
                        return Json(new { success = true });
                    }
                    return Json(new { success = false, msg = "Conversão não encontrada" });
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
