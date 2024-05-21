using Impressao.Models;
using Impressao.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Impressao.Controllers
{
    public class MateriaPrimaController : Controller
    {
        // GET: MateriaPrimaController
        public ActionResult Index()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("ver_materias_prima"))
            {
                DataContext context = new();
                List<MateriaPrima> materias = context.MateriaPrima.ToList();
                for (int i = 0; i < materias.Count; i++)
                {
                    materias[i].Grupo = context.MateriaPrimaGrupo.Find(materias[i].IdGrupo);
                    materias[i].UnidadeMedida = context.UnidadeMedida.Find(materias[i].IdUnidadeMedida);
                    materias[i].SetorEstoque = context.SetorEstoque.Find(materias[i].IdSetorEstoque);

                    materias[i].ProdutosEngenharia = context.ProdutoEngenharia.Where(t => t.IdMateriaPrima == materias[i].Id).ToList();
                }

                if (TempData["success"] != null)
                {
                    ViewBag.Success = TempData["success"];
                    TempData["success"] = null;
                }

                return View(materias);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public JsonResult ValidaId(int id)
        {
            DataContext context = new();
            MateriaPrima materia = context.MateriaPrima.Find(id);

            return Json(new { valid = materia == null });
        }

        // GET: MateriaPrimaController/Create
        public ActionResult Create()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_materias_prima"))
            {
                DataContext context = new();
                
                var materiaVM = new MateriaPrimaViewModel();
                materiaVM.Materia = new MateriaPrima{ Ativo = true };

                materiaVM.Grupos = context.MateriaPrimaGrupo.ToList();
                materiaVM.UnidadesMedida = context.UnidadeMedida.ToList();
                materiaVM.SetoresEstoque = context.SetorEstoque.ToList();


                return View(materiaVM);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: MateriaPrimaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MateriaPrima materia)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_materias_prima"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(materia.Titulo))
                    {
                        DataContext context = new();

                        context.MateriaPrima.Add(materia);
                        context.Entry(materia).State = EntityState.Added;
                        context.SaveChanges();

                        TempData["success"] = "Matéria prima salva com sucesso!";

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

        // GET: MateriaPrimaController/Edit/5
        public ActionResult Edit(int? id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_materias_prima"))
            {
                if (id != null)
                {
                    DataContext context = new();

                    var materiaVM = new MateriaPrimaViewModel();
                    materiaVM.Materia = context.MateriaPrima.Find(id);

                    materiaVM.Grupos = context.MateriaPrimaGrupo.ToList();
                    materiaVM.UnidadesMedida = context.UnidadeMedida.ToList();
                    materiaVM.SetoresEstoque = context.SetorEstoque.ToList();

                    return View(materiaVM);
                }
                else
                {
                    return RedirectToAction(nameof(Create));
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: MateriaPrimaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MateriaPrima materia)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_materias_prima"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(materia.Titulo))
                    {
                        DataContext context = new();
                        context.MateriaPrima.Update(materia);
                        context.Entry(materia).State = EntityState.Modified;
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

        // POST: MateriaPrimaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_materias_prima"))
            {
                try
                {
                    DataContext context = new();
                    MateriaPrima materia = context.MateriaPrima.Find(id);
                    if (materia != null)
                    {
                        context.MateriaPrima.Remove(materia);
                        context.SaveChanges();
                        return Json(new { success = true });
                    }
                    return Json(new { success = false, msg = "Matéria prima não encontrada" });
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
