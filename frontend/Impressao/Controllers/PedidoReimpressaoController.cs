using Impressao.Models;
using Impressao.Views.Pedido;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Impressao.Controllers
{
    public class PedidoReimpressaoController : Controller
    {
        // GET: PedidoReimpressao
        public ActionResult Index()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_pedidos_reimpressao"))
            {
                DataContext context = new();
                var reimpressao = context.PedidoReimpressao.ToList();

                for (int i = 0; i < reimpressao.Count; i++)
                {
                    reimpressao[i].PedidoItem = context.PedidoItem.Find(reimpressao[i].PedidoItemId);
                    reimpressao[i].PedidoItem.Produto = context.Produto.Find(reimpressao[i].PedidoItem.ProdutoId);
                }

                if (TempData["success"] != null)
                {
                    ViewBag.Success = TempData["success"];
                    TempData["success"] = null;
                }

                return View(reimpressao);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public JsonResult Itens(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_pedidos_reimpressao"))
            {
                try
                {
                    DataContext context = new();
                    var reimpressao = context.PedidoReimpressao.Find(id);
                    if (reimpressao != null)
                    {
                        var arquivos = context.PedidoReimpressaoArquivo.Where(a => a.PedidoReimpressaoId == reimpressao.Id).ToList();
                        for (int i = 0; i < arquivos.Count; i++)
                        {
                            arquivos[i].ArquivoFrente = context.PedidoItemArquivo.Find(arquivos[i].ArquivoFrenteId);
                        }
                        return Json(new { success = true, arquivos });
                    }

                    return Json(new { success = false, msg = "Reimpressão não encontrada" });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, msg = e.Message });
                }
            }
            else
            {
                return Json(new { success = false, msg = "Acesso negado" });
            }
        }

        // GET: PedidoReimpressao/Create
        public ActionResult Create()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos_reimpressao"))
            {
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PedidoReimpressao/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PedidoReimpressao param)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos_reimpressao"))
            {
                try
                {
                    DataContext context = new();

                    // busca arquivo
                    var arquivo = context.PedidoItemArquivo.Find(Convert.ToInt32(Request.Form["arquivo"]));
                    if (arquivo != null)
                    {
                        PedidoReimpressao reimpressao = new()
                        {
                            DataCadastro = DateTime.Now,
                            PedidoItemId = arquivo.PedidoItemId
                        };
                        reimpressao.PedidoItem = context.PedidoItem.Find(reimpressao.PedidoItemId);
                        reimpressao.PedidoItem.Produto = context.Produto.Find(reimpressao.PedidoItem.ProdutoId);

                        var rempressaoArquivo = CarregaFotos(reimpressao, arquivo.Id);


                        context.PedidoReimpressao.Add(reimpressao);
                        context.Entry(reimpressao).State = EntityState.Added;

                        context.SaveChanges();

                        rempressaoArquivo.PedidoReimpressaoId = reimpressao.Id;

                        context.PedidoReimpressaoArquivo.Add(rempressaoArquivo);
                        context.Entry(rempressaoArquivo).State = EntityState.Added;

                        context.SaveChanges();

                        return RedirectToAction(nameof(Edit), new { id = reimpressao.Id });
                    }

                    return View();
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

        // GET: PedidoReimpressao/Edit/5
        public ActionResult Edit(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos_reimpressao"))
            {
                DataContext context = new();
                var reimpressao = context.PedidoReimpressao.Find(id);
                if (reimpressao != null)
                {
                    reimpressao.PedidoItem = context.PedidoItem.Find(reimpressao.PedidoItemId);
                    reimpressao.PedidoItem.Pedido = context.Pedido.Find(reimpressao.PedidoItem.PedidoId);
                    reimpressao.PedidoItem.Pedido.Cliente = context.Cliente.Find(reimpressao.PedidoItem.Pedido.ClienteId);
                    reimpressao.PedidoItem.Produto = context.Produto.Find(reimpressao.PedidoItem.ProdutoId);
                }

                return View(reimpressao);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PedidoReimpressao/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(PedidoReimpressaoArquivo param)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos_reimpressao"))
            {
                try
                {
                    // carrega a reimpressão
                    DataContext context = new();
                    var reimpressao = context.PedidoReimpressao.Find(param.PedidoReimpressaoId);
                    if (reimpressao != null)
                    {
                        reimpressao.PedidoItem = context.PedidoItem.Find(reimpressao.PedidoItemId);
                        reimpressao.PedidoItem.Produto = context.Produto.Find(reimpressao.PedidoItem.ProdutoId);

                        var rempressaoArquivo = CarregaFotos(reimpressao, param.ArquivoFrenteId);

                        // salva
                        context.PedidoReimpressaoArquivo.Add(rempressaoArquivo);
                        context.Entry(rempressaoArquivo).State = EntityState.Added;
                        context.SaveChanges();

                        return Json(new { success = true });
                    }

                    return Json(new { success = false, msg = "Reimpressão não encontrada" });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, msg = e.Message });
                }
            }
            else
            {
                return Json(new { success = false, msg = "Acesso negado" });
            }
        }

        private PedidoReimpressaoArquivo CarregaFotos(PedidoReimpressao reimpressao, int arquivoId)
        {
            // carrega a foto
            DataContext context = new();
            var arquivo = context.PedidoItemArquivo.Find(arquivoId);
            var arquivos = context.PedidoItemArquivo.Where(a => a.PedidoItemId == reimpressao.PedidoItemId).OrderBy(a => a.NomeArquivo).ToList();

            PedidoReimpressaoArquivo rempressaoArquivo = new()
            {
                PedidoReimpressaoId = reimpressao.Id
            };

            if (reimpressao.PedidoItem.Produto.DisposicaoImagem == 'D')
            {
                // verifica a paridade
                int cnt = 0;
                foreach (var item in arquivos)
                {
                    if (item.Id == arquivo.Id)
                    {
                        if (cnt % 2 == 0)
                        {
                            rempressaoArquivo.ApontamentoFrente = true;
                            rempressaoArquivo.ArquivoFrenteId = arquivo.Id;
                            if (cnt + 1 < arquivos.Count && arquivo.Album == arquivos[cnt + 1].Album)
                            {
                                rempressaoArquivo.ArquivoVersoId = arquivos[cnt + 1].Id;
                            }
                        }
                        else
                        {
                            rempressaoArquivo.ApontamentoFrente = false;
                            if (cnt > 0 && arquivo.Album == arquivos[cnt - 1].Album)
                            {
                                rempressaoArquivo.ArquivoFrenteId = arquivos[cnt - 1].Id;
                            }
                            rempressaoArquivo.ArquivoVersoId = arquivo.Id;
                        }

                        break;
                    }
                    cnt++;
                }
            }
            else
            {
                rempressaoArquivo.ApontamentoFrente = true;
                rempressaoArquivo.ArquivoFrenteId = arquivo.Id;
            }

            return rempressaoArquivo;
        }

        // POST: PedidoReimpressao/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBlade()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos_reimpressao"))
            {
                try
                {
                    DataContext context = new();
                    var lamina = context.PedidoReimpressaoArquivo.Find(Convert.ToInt32(Request.Form["id"]));
                    if (lamina != null)
                    {
                        context.Remove(lamina);
                        context.Entry(lamina).State = EntityState.Deleted;
                        context.SaveChanges();

                        return Json(new { success = true });
                    }

                    return Json(new { success = false, msg = "Lâmina não encontrada" });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, msg = e.Message });
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos_reimpressao"))
            {
                try
                {
                    DataContext context = new();
                    var reimpressao = context.PedidoReimpressao.Find(Convert.ToInt32(Request.Form["id"]));
                    if (reimpressao != null)
                    {
                        // Carrega arquivos
                        var arquivos = context.PedidoReimpressaoArquivo.Where(a => a.PedidoReimpressaoId == reimpressao.Id).ToList();
                        foreach (var foto in arquivos)
                        {
                            context.Remove(foto);
                            context.Entry(foto).State = EntityState.Deleted;
                            context.SaveChanges();
                        }

                        context.Remove(reimpressao);
                        context.Entry(reimpressao).State = EntityState.Deleted;
                        context.SaveChanges();

                        return Json(new { success = true });
                    }

                    return Json(new { success = false, msg = "Pedido não encontrado" });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, msg = e.Message });
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
