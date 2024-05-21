using Impressao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Impressao.Controllers
{
    public class PerfilController : Controller
    {
        public ActionResult Index()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_perfis"))
            {
                DataContext context = new();
                List<Perfil> perfis = context.Perfil.OrderBy(p => p.Id).ToList();

                foreach (Perfil perf in perfis)
                {
                    perf.PerfilFuncoes = context.PerfilFuncao.Where(f => f.PerfilId == perf.Id).ToList();
                    perf.UsuarioPerfis = context.UsuarioPerfil.Where(f => f.PerfilId == perf.Id).ToList();
                }

                if (TempData["success"] != null)
                {
                    ViewBag.Success = TempData["success"];
                    TempData["success"] = null;
                }

                return View(perfis);
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }
        public JsonResult ValidaId(int id)
        {
            DataContext context = new();
            Perfil perfil = context.Perfil.Find(id);

            return Json(new { valid = perfil == null });
        }

        public ActionResult Create()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_perfis"))
            {
                Perfil perfil = new();
                CarregaFuncoes(perfil.Id);
                return View(perfil);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Perfil perfil)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_perfis"))
            {
                DataContext context = new();

                if (!String.IsNullOrEmpty(perfil.Titulo))
                {
                    context.Perfil.Add(perfil);
                    context.Entry(perfil).State = EntityState.Added;
                    context.SaveChanges();

                    SalvaFuncoes(perfil);
                }
                else
                {
                    return View();
                }

                TempData["success"] = "Perfil salvo com sucesso!";

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult Edit(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_perfis"))
            {
                DataContext context = new();
                Perfil perfil = context.Perfil.Find(id);
                if (perfil != null)
                {
                    perfil.PerfilFuncoes = context.PerfilFuncao.Where(f => f.PerfilId == perfil.Id).ToList();
                }
                else
                {
                    perfil = new();
                }

                CarregaFuncoes(perfil.Id);

                return View(perfil);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Perfil perfil)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_perfis"))
            {
                DataContext context = new();

                if (!String.IsNullOrEmpty(perfil.Titulo))
                {
                    context.Perfil.Update(perfil);
                    context.Entry(perfil).State = EntityState.Modified;
                    context.SaveChanges();

                    SalvaFuncoes(perfil);
                }
                else
                {
                    return View();
                }


                TempData["success"] = "Perfil salvo com sucesso!";

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        private void CarregaFuncoes(int id)
        {
            DataContext context = new();
            List<Funcao> funcoes = context.Funcao.OrderBy(f => f.Id).ToList();

            foreach (var funcao in funcoes)
            {
                var result = context.PerfilFuncao.Where(p => p.PerfilId == id && p.FuncaoId == funcao.Id).ToList();
                if (result.Count > 0)
                {
                    var perfilFuncao = result.First();
                    funcao.PerfilFuncoes.Add(perfilFuncao);
                }
            }

            ViewBag.funcoes = funcoes;

        }

        private void SalvaFuncoes(Perfil perfil)
        {
            DataContext context = new();

            string funcoes = Request.Form["perfilFuncoes"];
            string[] ids = funcoes.Split(',');
            if (funcoes != "")
            {
                foreach (string id in ids)
                {
                    var result = context.PerfilFuncao.Where(p => p.PerfilId == perfil.Id && p.FuncaoId == Convert.ToInt32(id)).ToList();
                    if (result.Count == 0)
                    {
                        var perfilFuncao = new PerfilFuncao
                        {
                            PerfilId = perfil.Id,
                            FuncaoId = Convert.ToInt32(id)
                        };

                        context.PerfilFuncao.Add(perfilFuncao);
                        context.Entry(perfilFuncao).State = EntityState.Added;
                        context.SaveChanges();
                    }
                }
            }

            var funcList = context.PerfilFuncao.Where(p => p.PerfilId == perfil.Id).ToList();
            foreach (var r in funcList)
            {
                if (!ids.Contains(r.FuncaoId.ToString()))
                {
                    context.PerfilFuncao.Remove(r);
                    context.Entry(r).State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
        }
    }
}
