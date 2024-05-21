using Impressao.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.Fonts.Tables.AdvancedTypographic;
using System.ComponentModel;
using System.Security.Cryptography.Xml;

namespace Impressao.Controllers
{
    class ProdutoTabela
    {
        public int ProdutoId { get; set; }
        public int? Id { get; set; }
        public string Produto { get; set;}
        public double? Valor { get; set;}
    }

    public class TabelaPrecoController : Controller
    {
        // GET: TabelaPrecoController
        public ActionResult Index()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_tab_precos"))
            {
                DataContext context = new();
                List<TabelaPreco> tabelasPreco = context.TabelaPreco.ToList();
                for (int i = 0; i < tabelasPreco.Count; i++)
                {
                    tabelasPreco[i].TabelaPrecoClientes = context.TabelaPrecoCliente.Where(t => t.TabelaId == tabelasPreco[i].Id).ToList();
                    tabelasPreco[i].TabelaPrecoProdutos = context.TabelaPrecoProduto.Where(t => t.TabelaId == tabelasPreco[i].Id).ToList();
                }

                if (TempData["success"] != null)
                {
                    ViewBag.Success = TempData["success"];
                    TempData["success"] = null;
                }

                return View(tabelasPreco);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public JsonResult ValidaId(int id)
        {
            DataContext context = new();
            TabelaPreco tab = context.TabelaPreco.Find(id);

            return Json(new { valid = tab == null });
        }

        // GET: TabelaPrecoController/Produtos/5
        public ActionResult Produtos(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_tab_precos"))
            {
                if (id != null)
                {
                    DataContext context = new();
                    var tabelaPreco = context.TabelaPreco.Find(id);
                    tabelaPreco.TabelaPrecoProdutos = context.TabelaPrecoProduto.Where(t => t.TabelaId == tabelaPreco.Id).ToList();

                    foreach (var item in tabelaPreco.TabelaPrecoProdutos)
                    {
                        item.Produto = context.Produto.Find(item.ProdutoId);
                    }

                    return View(tabelaPreco);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ListaProdutos()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_tab_precos"))
            {
                try
                {
                    if (Request.Form["tabela"] != "")
                    {
                        int tabela = Convert.ToInt32(Request.Form["tabela"]);
                        DataContext context = new();

                        var produtos = context.Produto.Where(p => p.Ativo).ToList();
                        var tabelaProduto = context.TabelaPrecoProduto.Where(t => t.TabelaId == tabela).ToList();

                        List<ProdutoTabela> lista = new();
                        foreach (var item in produtos)
                        {
                            var tabList = tabelaProduto.Where(t => t.ProdutoId == item.Id).ToList();
                            if (tabList.Count > 0)
                            {
                                foreach (var tab in tabList)
                                {
                                    lista.Add(new ProdutoTabela
                                    {
                                        ProdutoId = item.Id,
                                        Produto = item.Titulo,
                                        Valor = tab.Valor,
                                        Id = tab.Id
                                    });
                                    break;
                                }
                            }
                            else
                            {
                                lista.Add(new ProdutoTabela
                                {
                                    ProdutoId = item.Id,
                                    Produto = item.Titulo
                                });
                            }
                        }

                        return Json(lista.OrderBy(l => l.ProdutoId));
                    }
                    else
                    {
                        return Json(new { success = false, msg = "Tabela não encontrada" });
                    }
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
        public JsonResult SalvarProduto()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_tab_precos"))
            {
                try
                {
                    TabelaPrecoProduto tab;

                    int tabela = Convert.ToInt32(Request.Form["tabela"]);
                    int produto = Convert.ToInt32(Request.Form["produto"]);
                    double valor = Convert.ToDouble(Request.Form["valor"]);

                    DataContext context = new();
                    if (Request.Form["id"] != "")
                    {
                        // update
                        int id = Convert.ToInt32(Request.Form["id"]);
                        tab = context.TabelaPrecoProduto.Find(id);
                        if (tab != null)
                        {
                            if (valor > 0)
                            {
                                tab.ProdutoId = produto;
                                tab.Valor = valor;

                                context.TabelaPrecoProduto.Update(tab);
                                context.Entry(tab).State = EntityState.Modified;
                            }
                            else
                            {
                                context.TabelaPrecoProduto.Remove(tab);
                                context.Entry(tab).State = EntityState.Deleted;
                            }

                        }
                        else
                        {
                            return Json(new { success = false, msg = "Tabela de produtos não encontrada" });
                        }
                    }
                    else
                    {
                        // create
                        tab = new TabelaPrecoProduto
                        {
                            TabelaId = tabela,
                            ProdutoId = produto,
                            Valor = valor
                        };

                        context.TabelaPrecoProduto.Add(tab);
                        context.Entry(tab).State = EntityState.Added;
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


        // GET: TabelaPrecoController/Clientes/5
        public ActionResult Clientes(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_tab_precos"))
            {
                if (id != null)
                {
                    DataContext context = new();
                    var tabelaPreco = context.TabelaPreco.Find(id);
                    tabelaPreco.TabelaPrecoClientes = context.TabelaPrecoCliente.Where(t => t.TabelaId == tabelaPreco.Id).ToList();

                    foreach (var item in tabelaPreco.TabelaPrecoClientes)
                    {
                        item.Cliente = context.Cliente.Find(item.ClienteId);
                    }

                    return View(tabelaPreco);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SalvarCliente()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_tab_precos"))
            {
                try
                {
                    TabelaPrecoCliente tab;

                    int tabela = Convert.ToInt32(Request.Form["tabela"]);
                    int cliente = Convert.ToInt32(Request.Form["cliente"]);
                    bool padrao = Convert.ToBoolean(Request.Form["padrao"]);

                    DataContext context = new();
                    if (Request.Form["id"] != "")
                    {
                        // update
                        int id = Convert.ToInt32(Request.Form["id"]);
                        tab = context.TabelaPrecoCliente.Find(id);
                        if (tab != null)
                        {
                            tab.ClienteId = cliente;
                            tab.Padrao = padrao;

                            context.TabelaPrecoCliente.Update(tab);
                            context.Entry(tab).State = EntityState.Modified;
                        }
                        else
                        {
                            return Json(new { success = false, msg = "Tabela de cliente não encontrada" });
                        }
                    }
                    else
                    {
                        // create
                        tab = new TabelaPrecoCliente
                        {
                            TabelaId = tabela,
                            ClienteId = cliente,
                            Padrao = padrao
                        };

                        context.TabelaPrecoCliente.Add(tab);
                        context.Entry(tab).State = EntityState.Added;
                    }

                    context.SaveChanges();

                    TabelaPadraoCliente(tab);

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
        public JsonResult DeletarCliente(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_tab_precos"))
            {
                try
                {
                    DataContext context = new();
                    var cliente = context.TabelaPrecoCliente.Find(id);
                    if (cliente != null)
                    {
                        context.TabelaPrecoCliente.Remove(cliente);
                        context.SaveChanges();
                        return Json(new { success = true });
                    }
                    return Json(new { success = false, msg = "Tabela de cliente não encontrado" });
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

        // GET: TabelaPrecoController/Create
        public ActionResult Create()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_tab_precos"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: TabelaPrecoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TabelaPreco tabelaPreco)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_tab_precos"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(tabelaPreco.Titulo))
                    {
                        DataContext context = new();

                        context.TabelaPreco.Add(tabelaPreco);
                        context.Entry(tabelaPreco).State = EntityState.Added;
                        context.SaveChanges();

                        TempData["success"] = "Tabela de preços salva com sucesso!";

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

        // GET: TabelaPrecoController/Edit/5
        public ActionResult Edit(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_tab_precos"))
            {
                if (id != null)
                {
                    DataContext context = new();
                    var tabelaPreco = context.TabelaPreco.Find(id);

                    return View(tabelaPreco);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: TabelaPrecoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TabelaPreco tabelaPreco)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_tab_precos"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(tabelaPreco.Titulo))
                    {
                        DataContext context = new();
                        context.TabelaPreco.Update(tabelaPreco);
                        context.Entry(tabelaPreco).State = EntityState.Modified;
                        context.SaveChanges();

                        TabelaPadrao(tabelaPreco);

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

        private void TabelaPadrao(TabelaPreco tabelaPreco)
        {
            DataContext context = new();
            
            if (tabelaPreco.Padrao)
            {
                var tabs = context.TabelaPreco.Where(t => t.Id != tabelaPreco.Id && t.Padrao).ToList();
                for (int i = 0; i < tabs.Count; i++)
                {
                    tabs[i].Padrao = false;

                    context.TabelaPreco.Update(tabs[i]);
                    context.Entry(tabs[i]).State = EntityState.Modified;
                }
            }
            else
            {
                var tabs = context.TabelaPreco.Where(t => t.Padrao && t.Ativo).ToList();
                if(tabs.Count == 0)
                {
                    var tab = context.TabelaPreco.Where(t => t.Ativo).First();
                    tab.Padrao = true;

                    context.TabelaPreco.Update(tab);
                    context.Entry(tab).State = EntityState.Modified;
                }
            }

            context.SaveChanges();

        }

        private void TabelaPadraoCliente(TabelaPrecoCliente tabelaPreco)
        {
            DataContext context = new();

            if (tabelaPreco.Padrao)
            {
                var tabs = context.TabelaPrecoCliente.Where(t => t.Id != tabelaPreco.Id && t.ClienteId == tabelaPreco.ClienteId && t.Padrao).ToList();
                for (int i = 0; i < tabs.Count; i++)
                {
                    tabs[i].Padrao = false;

                    context.TabelaPrecoCliente.Update(tabs[i]);
                    context.Entry(tabs[i]).State = EntityState.Modified;
                }
            }
            else
            {
                var tabs = context.TabelaPrecoCliente.Where(t => t.Padrao && t.ClienteId == tabelaPreco.ClienteId).ToList();
                if (tabs.Count == 0)
                {
                    var tab = context.TabelaPrecoCliente.First();
                    tab.Padrao = true;

                    context.TabelaPrecoCliente.Update(tab);
                    context.Entry(tab).State = EntityState.Modified;
                }
            }

            context.SaveChanges();
        }

        // POST: TabelaPrecoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_tab_precos"))
            {
                try
                {
                    DataContext context = new();
                    TabelaPreco tabela = context.TabelaPreco.Find(id);
                    if (tabela != null)
                    {
                        context.TabelaPreco.Remove(tabela);
                        context.SaveChanges();
                        return Json(new { success = true });
                    }
                    return Json(new { success = false, msg = "Tabela de preços não encontrada" });
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
