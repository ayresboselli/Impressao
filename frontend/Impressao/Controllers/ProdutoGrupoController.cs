using Impressao.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Impressao.Controllers
{
    public class ProdutoGrupoController : Controller
    {
        // GET: ProdutoGrupoController
        public ActionResult Index()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_produto_grupos"))
            {
                DataContext context = new();
                List<ProdutoGrupo> grupos = context.ProdutoGrupo.ToList();
                for (int i = 0; i < grupos.Count; i++)
                {
                    grupos[i].Produtos = context.Produto.Where(c => c.ProdutoGrupoId == grupos[i].Id).ToList();
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
            ProdutoGrupo grupo = context.ProdutoGrupo.Find(id);

            return Json(new { valid = grupo == null });
        }

        // GET: ProdutoGrupoController/Create
        public ActionResult Create()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_produto_grupos"))
            {
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ProdutoGrupoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProdutoGrupo grupo)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_produto_grupos"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(grupo.Titulo))
                    {
                        DataContext context = new();

                        context.ProdutoGrupo.Add(grupo);
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

        // GET: ProdutoGrupoController/Edit/5
        public ActionResult Edit(int? id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_produto_grupos"))
            {
                if (id != null)
                {
                    DataContext context = new();
                    var grupo = context.ProdutoGrupo.Find(id);

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

        // POST: ProdutoGrupoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProdutoGrupo grupo)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_produto_grupos"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(grupo.Titulo))
                    {
                        DataContext context = new();

                        context.ProdutoGrupo.Add(grupo);
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

        // POST: ProdutoGrupoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_produto_grupos"))
            {
                try
                {
                    DataContext context = new();
                    ProdutoGrupo grupo = context.ProdutoGrupo.Find(id);
                    if (grupo != null)
                    {
                        context.ProdutoGrupo.Remove(grupo);
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
