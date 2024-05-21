using Impressao.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Impressao.Controllers
{
    public class OrdemCelulas
    {
        public int ProdutoId { get; set; }
        public int[] Ordem { get; set; }
    }

    public class ProdutoController : Controller
    {
        // GET: ProdutoController
        public ActionResult Index()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("ver_produtos"))
            {
                DataContext context = new();
                List<Produto> produtos = context.Produto.ToList();

                for (int i = 0; i < produtos.Count; i++)
                {
                    produtos[i].ProdutoGrupo = context.ProdutoGrupo.Find(produtos[i].ProdutoGrupoId);
                    produtos[i].PedidoItens = context.PedidoItem.Where(c => c.ProdutoId == produtos[i].Id).ToList();
                    produtos[i].Informacoes = context.ProdutoInformacao.Where(c => c.ProdutoId == produtos[i].Id).ToList();
                }

                if (TempData["success"] != null)
                {
                    ViewBag.Success = TempData["success"];
                    TempData["success"] = null;
                }

                return View(produtos);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public JsonResult ValidaId(int id)
        {
            DataContext context = new();
            Produto produto = context.Produto.Find(id);
            
            return Json(new { valid = produto==null });
        }
        public ActionResult List()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("ver_produtos"))
            {
                DataContext context = new();
                List<Produto> produtos = context.Produto.Where(p => p.Ativo).ToList();

                return Json(produtos);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public JsonResult ListInfo(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("ver_produtos"))
            {
                DataContext context = new();
                List<ProdutoInformacao> informacoes = context.ProdutoInformacao.Where(p => p.ProdutoId == id).ToList();

                return Json(informacoes);
            }
            else
            {
                return Json("");
            }
        }
        public JsonResult Info(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("ver_produtos"))
            {
                DataContext context = new();
                ProdutoInformacao informacao = context.ProdutoInformacao.Find(id);

                return Json(informacao);
            }
            else
            {
                return Json("");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Info(ProdutoInformacao informacao)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_produtos"))
            {
                try
                {
                    DataContext context = new();

                    if (informacao.Id == 0)
                    {
                        context.ProdutoInformacao.Add(informacao);
                        context.Entry(informacao).State = EntityState.Added;
                    }
                    else
                    {
                        context.ProdutoInformacao.Update(informacao);
                        context.Entry(informacao).State = EntityState.Modified;
                    }

                    context.SaveChanges();

                    return Json(new { success = true });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult InfoDelete(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_produtos"))
            {
                try
                {
                    DataContext context = new();
                    ProdutoInformacao informacao = context.ProdutoInformacao.Find(id);
                    if (informacao != null)
                    {
                        context.ProdutoInformacao.Remove(informacao);
                        context.SaveChanges();
                        return Json(new { success = true });
                    }

                    return Json(new { success = false, msg = "Informação não encontrada" });
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

        public JsonResult Grupos()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("ver_produtos"))
            {
                DataContext context = new();
                List<ProdutoGrupo> grupo = context.ProdutoGrupo.ToList();
                return Json(grupo);
            }
            else
            {
                return Json("");
            }
        }

        // GET: ProdutoController/Create
        public ActionResult Create()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_produtos"))
            {
                Produto produto = new()
                {
                    ArquivosJPEG = true,
                    ArquivosPDF = true,
                    Ativo = true,
                    RenomearArquivo = true,
                    QuantidadeArquivosProduto = 1,
                    Largura = 0,
                    Altura = 0,
                    LarguraMidia = 0,
                    AlturaMidia = 0,
                    DeslocamentoFrenteX = 0,
                    DeslocamentoFrenteY = 0,
                    DeslocamentoVersoX = 0,
                    DeslocamentoVersoY = 0,
                };

                return View(produto);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: ProdutoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Produto produto)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_produtos"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(produto.Titulo))
                    {
                        DataContext context = new();

                        if (produto.DisposicaoImpressao == 'S')
                        {
                            produto.DisposicaoImagem = 'S';
                        }

                        context.Produto.Add(produto);
                        context.Entry(produto).State = EntityState.Added;
                        context.SaveChanges();
                        return RedirectToAction(nameof(Edit), new { id = produto.Id });
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
                return RedirectToAction("Index");
            }
        }

        // GET: ProdutoController/Edit/5
        public ActionResult Edit(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_produtos"))
            {
                DataContext context = new();
                Produto produto = context.Produto.Find(id);

                return View(produto);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ProdutoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Produto produto)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_produtos"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(produto.Titulo))
                    {
                        DataContext context = new();

                        if (produto.DisposicaoImpressao == 'S')
                        {
                            produto.DisposicaoImagem = 'S';
                        }

                        context.Produto.Update(produto);
                        context.Entry(produto).State = EntityState.Modified;
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
                return RedirectToAction("Index");
            }
        }


        public ActionResult Duplicate(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_produtos"))
            {
                DataContext context = new();
                Produto produto = context.Produto.Find(id);
                if (produto != null)
                {
                    produto.Informacoes = context.ProdutoInformacao.ToList();

                    Produto newProd = new()
                    {
                        Titulo = produto.Titulo,
                        ProdutoGrupoId = produto.ProdutoGrupoId,
                        ProdutoSubstratoId = produto.ProdutoSubstratoId,
                        ProdutoHotFolderId = produto.ProdutoHotFolderId,
                        ArquivosJPEG = produto.ArquivosJPEG,
                        ArquivosPDF = produto.ArquivosPDF,
                        QuantidadeArquivosProduto = produto.QuantidadeArquivosProduto,
                        RenomearArquivo = produto.RenomearArquivo,
                        Largura = produto.Largura,
                        Altura = produto.Altura,
                        Orientacao = produto.Orientacao,
                        LarguraMidia = produto.LarguraMidia,
                        AlturaMidia = produto.AlturaMidia,
                        DisposicaoImpressao = produto.DisposicaoImpressao,
                        DisposicaoImagem = produto.DisposicaoImagem,
                        DeslocamentoFrenteX = produto.DeslocamentoFrenteX,
                        DeslocamentoFrenteY = produto.DeslocamentoFrenteY,
                        DeslocamentoVersoX = produto.DeslocamentoVersoX,
                        DeslocamentoVersoY = produto.DeslocamentoVersoY,
                        Ativo = produto.Ativo,
                    };

                    context.Produto.Add(newProd);
                    context.Entry(newProd).State = EntityState.Added;
                    context.SaveChanges();
                    context.Database.CloseConnection();
                    context.Dispose();

                    context = new DataContext();
                    foreach (var info in produto.Informacoes)
                    {
                        ProdutoInformacao newInfo = new()
                        {
                            ProdutoId = newProd.Id,
                            Pagina = info.Pagina,
                            Tipo = info.Tipo,
                            Texto = info.Texto,
                            PosX = info.PosX,
                            PosY = info.PosY,
                            Orientacao = info.Orientacao
                        };

                        context.ProdutoInformacao.Add(newInfo);
                        context.Entry(newInfo).State = EntityState.Added;
                    }

                    context.SaveChanges();

                    return RedirectToAction("Edit", new { id = newProd.Id });
                }
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: ProdutoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("cad_produtos"))
            {
                try
                {
                    DataContext context = new();
                    Produto produto = context.Produto.Find(id);
                    if (produto != null)
                    {
                        // Deletar informação
                        produto.Informacoes = context.ProdutoInformacao.Where(c => c.ProdutoId == produto.Id).ToList();

                        foreach (ProdutoInformacao info in produto.Informacoes)
                        {
                            context.ProdutoInformacao.Remove(info);
                        }

                        context.Produto.Remove(produto);
                        context.SaveChanges();
                        return Json(new { success = true });
                    }
                    return Json(new { success = false, msg = "Produto não encontrado" });
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

        /* Engenharia */
        public JsonResult ListarCelulas(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("ver_produtos"))
            {
                DataContext context = new();
                List<ProdutoRoteiro> roteiro = context.ProdutoRoteiro.Where(i => i.IdProduto == id).OrderBy(i => i.Sequencia).ToList();

                for(var i = 0; i < roteiro.Count; i++)
                {
                    roteiro[i].Celula = context.Celula.Find(roteiro[i].IdCelula);
                }

                return Json(roteiro);
            }
            else
            {
                return Json("");
            }
        }

        public JsonResult ListarCelulasAdd(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions") != null ? HttpContext.Session.GetString("Functions").Split(",") : null;
            if (listaFuncoes != null && listaFuncoes.Contains("ver_celulas"))
            {
                DataContext context = new();
                Produto produto = context.Produto.Find(id);
                produto.ProdutosRoteiro = context.ProdutoRoteiro.Where(i => i.IdProduto == produto.Id).ToList();
                List<Celula> celulas = context.Celula.ToList();

                foreach (var roteiro in produto.ProdutosRoteiro)
                {
                    var i = celulas.Where(i => i.Id == roteiro.IdCelula).First();
                    if(i != null)
                    {
                        celulas.Remove(i);
                    }
                }

                return Json(celulas);
            }
            else
            {
                return Json("");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CelulasAdd(ProdutoRoteiro roteiro)
        {
            DataContext context = new();
            Produto produto = context.Produto.Find(roteiro.IdProduto);
            Celula celula = context.Celula.Find(roteiro.IdCelula);
            int seq = 0;

            if (produto != null && celula != null)
            {
                produto.ProdutosRoteiro = context.ProdutoRoteiro.Where(i => i.IdProduto == produto.Id).OrderBy(i => i.Sequencia).ToList();
                
                if (produto.ProdutosRoteiro.Count > 0)
                {
                    seq = produto.ProdutosRoteiro.Last().Sequencia + 1;
                }

                var novo = new ProdutoRoteiro
                {
                    IdProduto = produto.Id,
                    IdCelula = celula.Id,
                    Sequencia = seq
                };

                context.ProdutoRoteiro.Add(novo);
                context.Entry(novo).State = EntityState.Added;
                context.SaveChanges();

                return Json(novo);
            }
            else
            {
                return Json("");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RoteiroSequencia(OrdemCelulas ordens)
        {
            DataContext context = new();
            Produto produto = context.Produto.Find(ordens.ProdutoId);
            produto.ProdutosRoteiro = context.ProdutoRoteiro.Where(i => i.IdProduto == produto.Id).OrderBy(i => i.Sequencia).ToList();

            for (int i = 0; i < ordens.Ordem.Length; i++)
            {
                var item = produto.ProdutosRoteiro.Where(r => r.Id == ordens.Ordem[i]).First();
                if(item != null && item.Sequencia != i)
                {
                    item.Sequencia = i;

                    context.ProdutoRoteiro.Update(item);
                    context.Entry(item).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }

            return Json("");
        }
    }
}
