using Impressao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Impressao.Controllers
{
    public class ConfigController : Controller
    {
        private string wwwPath;
        public ConfigController(Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment)
        {
            wwwPath = _environment.WebRootPath;
        }

        public ActionResult Index()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("config_parametros"))
            {
                DataContext context = new();
                Config config = context.Config.Find(1);

                if (TempData["success"] != null)
                {
                    ViewBag.Success = TempData["success"];
                    TempData["success"] = null;
                }

                return View(config);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Salvar(Config config)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("config_parametros"))
            {
                DataContext context = new();

                config.WwwPath = wwwPath;

                if (config.Id == 0)
                {
                    config.Id = 1;
                    context.Config.Add(config);
                    context.Entry(config).State = EntityState.Added;
                }
                else
                {
                    context.Config.Update(config);
                    context.Entry(config).State = EntityState.Modified;
                }

                context.SaveChanges();

                TempData["success"] = "Parâmetros salvos com sucesso!";
            }
            
            return RedirectToAction("Index");
        }
    }
}
