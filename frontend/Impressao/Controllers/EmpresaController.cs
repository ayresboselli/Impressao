using Impressao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Impressao.Controllers
{
    public class EmpresaController : Controller
    {
        private string wwwPath;

        public EmpresaController(Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment)
        {
            wwwPath = _environment.WebRootPath;
        }

        public ActionResult Index()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("config_dados_empresa"))
            {
                DataContext context = new();
                Empresa empresa = context.Empresa.Find(1);

                if (TempData["success"] != null)
                {
                    ViewBag.Success = TempData["success"];
                    TempData["success"] = null;
                }

                return View(empresa);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Salvar(Empresa empresa, IFormFile logo)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("config_dados_empresa"))
            {
                DataContext context = new();

                if (logo != null)
                {
                    using (var stream = new FileStream(wwwPath + "\\logo.jpg", FileMode.Create))
                    {
                        await logo.CopyToAsync(stream);
                    }
                }

                if (empresa.Id == 0)
                {
                    empresa.Id = 1;
                    context.Empresa.Add(empresa);
                    context.Entry(empresa).State = EntityState.Added;
                }
                else
                {
                    context.Empresa.Update(empresa);
                    context.Entry(empresa).State = EntityState.Modified;
                }

                context.SaveChanges();

                TempData["success"] = "Empresa salva com sucesso!";
            }

            return RedirectToAction("Index");
        }
    }
}
