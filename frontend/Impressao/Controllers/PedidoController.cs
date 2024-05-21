using Impressao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using PdfSharp.Drawing;
using TheArtOfDev.HtmlRenderer.Adapters;
using ClosedXML.Excel;
using Impressao.Views.Pedido;
using System.IO;
using DocumentFormat.OpenXml.Bibliography;
using ClosedXML.Report;

namespace Impressao.Controllers
{
    class Agrupado
    {
        public int Count { get; set; }
        public int Largura { get; set; }
        public int Altura { get; set; }
    }

    public class PedidoController : Controller
    {
        private string wwwPath;
        private string contentPath;
        private Config config;

        public PedidoController(Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment)
        {
            wwwPath = _environment.WebRootPath;
            contentPath = _environment.ContentRootPath;

            DataContext context = new();
            config = context.Config.Find(1);
        }

        // GET: PedidoController
        public ActionResult Index()
        {
            if (HttpContext.Session.GetString("Functions") != null)
            {
                string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
                if (listaFuncoes.Contains("ver_pedidos_ativos"))
                {
                    DataContext context = new();
                    List<Pedido> pedidos = context.Pedido.Where(p => p.DataFinalizado == null).OrderByDescending(p => p.Id).ToList();

                    for (int i = 0; i < pedidos.Count; i++)
                    {
                        pedidos[i].Cliente = context.Cliente.Find(pedidos[i].ClienteId);
                        pedidos[i].TabelaPreco = context.TabelaPreco.Find(pedidos[i].TabelaId);
                    }

                    if (TempData["success"] != null)
                    {
                        ViewBag.Success = TempData["success"];
                        TempData["success"] = null;
                    }

                    return View(pedidos);
                }
            }
            
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Finalizados()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_pedidos_finalizados"))
            {
                DataContext context = new();
                List<Pedido> pedidos = context.Pedido.Where(p => p.DataFinalizado != null).OrderByDescending(p => p.Id).ToList();

                for (int i = 0; i < pedidos.Count; i++)
                {
                    pedidos[i].Cliente = context.Cliente.Find(pedidos[i].ClienteId);
                }

                if (TempData["success"] != null)
                {
                    ViewBag.Success = TempData["success"];
                    TempData["success"] = null;
                }

                return View(pedidos);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public JsonResult Clientes()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_pedidos_ativos"))
            {
                DataContext context = new();
                List<Cliente> clientes = context.Cliente.Where(c => c.Ativo).ToList();

                return Json(clientes);
            }
            else
            {
                return Json(new { success = false, msg = "Acesso negado" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult TabelasPrecos()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_pedidos_ativos"))
            {
                DataContext context = new();
                List<TabelaPreco> tabelas = new();

                if (Request.Form["cliente"] != "")
                {
                    int cliente = Convert.ToInt32(Request.Form["cliente"]);
                    var tabCliente = context.TabelaPrecoCliente.Where(t => t.ClienteId == cliente).OrderByDescending(t => t.Padrao).ToList();
                    if (tabCliente.Count > 0)
                    {
                        foreach (var item in tabCliente)
                        {
                            tabelas.Add(
                                context.TabelaPreco.Where(t => t.Id == item.TabelaId).First()
                            );
                        }
                    }
                }

                TabelaPreco tabPadrao = context.TabelaPreco.Where(t => t.Ativo && t.Padrao).First();
                if (!tabelas.Contains(tabPadrao))
                {
                    tabelas.Add(tabPadrao);
                }

                return Json(tabelas);
            }
            else
            {
                return Json(new { success = false, msg = "Acesso negado" });
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PrecoProduto()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_pedidos_ativ "))
            {
                int tabela = Convert.ToInt32(Request.Form["tabela"]);
                int produto = Convert.ToInt32(Request.Form["produto"]);

                DataContext context = new();
                var result = context.TabelaPrecoProduto.Where(t => t.TabelaId == tabela && t.ProdutoId == produto);
                if (result != null)
                {
                    return Json(result.First());
                }
                else
                {
                    var tabPrec = context.TabelaPreco.Where(t => t.Ativo && t.Padrao).First();
                    var tabProd = context.TabelaPrecoProduto.Where(t => t.TabelaId == tabPrec.Id && t.ProdutoId == produto);
                    if (tabProd != null)
                    {
                        return Json(tabProd.First());
                    }
                }

                return Json("");
            }
            else
            {
                return Json(new { success = false, msg = "Acesso negado" });
            }
            
        }

        public JsonResult Itens(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_pedidos_ativos"))
            {
                List<PedidoItem> itens = new() { };
                DataContext context = new();
                Pedido pedido = context.Pedido.Find(id);

                if (pedido != null)
                {
                    itens = context.PedidoItem.Where(c => c.PedidoId == pedido.Id).OrderBy(p => p.Id).ToList();

                    for (int i = 0; i < itens.Count; i++)
                    {
                        itens[i].Produto = context.Produto.Find(itens[i].ProdutoId);
                        itens[i].PedidoItemArquivos = context.PedidoItemArquivo.Where(a => a.PedidoItemId == itens[i].Id).ToList();
                        itens[i].PedidoItemUploads = context.PedidoItemUpload.Where(a => a.PedidoItemId == itens[i].Id).ToList();
                    }
                }

                return Json(itens);
            }
            else
            {
                return Json(new { success = false, msg = "Acesso negado" });
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Itens(PedidoItem item)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos"))
            {
                try
                {
                    DataContext context = new();
                    if (item.Id == 0)
                    {
                        context.PedidoItem.Add(item);
                        context.Entry(item).State = EntityState.Added;
                    }
                    else
                    {
                        context.PedidoItem.Update(item);
                        context.Entry(item).State = EntityState.Modified;
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

        // GET: PedidoController/Create
        public ActionResult Create()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos"))
            {
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PedidoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pedido pedido)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos"))
            {
                try
                {
                    if (pedido.ClienteId != 0)
                    {
                        pedido.DataEntrada = DateTime.Now;

                        DataContext context = new();
                        context.Pedido.Add(pedido);
                        context.Entry(pedido).State = EntityState.Added;
                        context.SaveChanges();

                        return RedirectToAction(nameof(Edit), new { id = pedido.Id });
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

        // GET: PedidoController/View/5
        public ActionResult View(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_pedidos_ativos") || listaFuncoes.Contains("ver_pedidos_finalizados"))
            {
                DataContext context = new();
                var pedido = context.Pedido.Find(id);
                pedido.Cliente = context.Cliente.Find(pedido.ClienteId);
                pedido.TabelaPreco = context.TabelaPreco.Find(pedido.TabelaId);

                return View(pedido);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PedidoController/Edit/5
        public ActionResult Edit(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos"))
            {
                DataContext context = new();
                //var pedido = context.Pedido.Where(c => c.Id == id).FirstOrDefault();
                var pedido = context.Pedido.Find(id);

                return View(pedido);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PedidoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pedido pedido)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos"))
            {
                try
                {
                    if (pedido.ClienteId != 0)
                    {
                        DataContext context = new();
                        context.Pedido.Update(pedido);
                        context.Entry(pedido).State = EntityState.Modified;
                        context.SaveChanges();

                        return RedirectToAction(nameof(Edit));
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
        
        public ActionResult Arquivos(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos"))
            {
                DataContext context = new();
                PedidoItem item = context.PedidoItem.Find(id);
                if (item != null)
                {
                    item.Pedido = context.Pedido.Find(item.PedidoId);
                    item.Pedido.Cliente = context.Cliente.Find(item.Pedido.ClienteId);
                    item.Pedido.TabelaPreco = context.TabelaPreco.Find(item.Pedido.TabelaId);
                    item.Produto = context.Produto.Find(item.ProdutoId);
                }

                return View(item);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ListaAlbuns()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos"))
            {
                List<string> albuns = new List<string>();

                DataContext context = new();
                PedidoItem item = context.PedidoItem.Find(Convert.ToInt32(Request.Form["id"]));
                if (item != null)
                {
                    item.PedidoItemArquivos = context.PedidoItemArquivo.Where(a => a.PedidoItemId == item.Id).OrderBy(a => a.Album).ToList();
                    foreach (PedidoItemArquivo foto in item.PedidoItemArquivos)
                    {
                        if (!albuns.Contains(foto.Album))
                        {
                            albuns.Add(foto.Album);
                        }
                    }

                    return Json(new { success = true, albuns });
                }

                return Json(new { success = false, msg = "Item não encontrado" });
            }
            else
            {
                return Json(new { success = false, msg = "Acesso negado" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ListaArquivos()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos"))
            {
                List<PedidoItemArquivo> arquivos = new();

                DataContext context = new();
                PedidoItem item = context.PedidoItem.Find(Convert.ToInt32(Request.Form["id"]));
                if (item != null)
                {
                    item.PedidoItemArquivos = context.PedidoItemArquivo.Where(a => a.PedidoItemId == item.Id && a.Album == Request.Form["album"].ToString()).OrderBy(a => a.NomeArquivo).ToList();
                    foreach (PedidoItemArquivo foto in item.PedidoItemArquivos)
                    {
                        arquivos.Add(foto);
                    }

                    return Json(new { success = true, arquivos });
                }

                return Json(new { success = false, msg = "Item não encontrado" });
            }
            else
            {
                return Json(new { success = false, msg = "Acesso negado" });
            }
        }

        public ActionResult UploadJpgFile(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos"))
            {
                DataContext context = new();
                PedidoItem item = context.PedidoItem.Find(id);
                if (item != null)
                {
                    item.Pedido = context.Pedido.Find(item.PedidoId);
                    item.Pedido.Cliente = context.Cliente.Find(item.Pedido.ClienteId);
                    item.Produto = context.Produto.Find(item.ProdutoId);

                    string output = config.PathFotos + @"\" + item.Pedido.Id.ToString() + @"\" + item.Id.ToString() + @"\";
                    if (!Directory.Exists(output))
                    {
                        Directory.CreateDirectory(output);
                    }

                    return View(item);
                }
                else
                {
                    return Redirect("Edit/" + id.ToString());
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult UploadJpgFolder(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos"))
            {
                DataContext context = new();
                PedidoItem item = context.PedidoItem.Find(id);
                if (item != null)
                {
                    item.Pedido = context.Pedido.Find(item.PedidoId);
                    item.Pedido.Cliente = context.Cliente.Find(item.Pedido.ClienteId);
                    item.PedidoItemUploads = context.PedidoItemUpload.Where(a => a.PedidoItemId == item.Id).ToList();
                    item.Produto = context.Produto.Find(item.ProdutoId);

                    string output = config.PathFotos + item.Pedido.Id.ToString() + @"\" + item.Id.ToString() + @"\";
                    if (!Directory.Exists(output))
                    {
                        Directory.CreateDirectory(output);
                    }

                    return View(item);
                }
                else
                {
                    return Redirect("Edit/" + id.ToString());
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Upload(object sender)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos"))
            {
                try
                {
                    IFormFileCollection files = Request.Form.Files;
                    int id_item = Convert.ToInt32(Request.Form["id_item"]);
                    string path = Request.Form["path"];

                    if (id_item != 0)
                    {
                        DataContext context = new();
                        PedidoItem item = context.PedidoItem.Find(id_item);
                        if (item != null)
                        {
                            string album = "001";
                            if (!string.IsNullOrEmpty(path))
                            {
                                string[] strct = path.Split('/');
                                if (strct.Length >= 2) { album = strct[strct.Length - 2]; }
                            }

                            item.Pedido = context.Pedido.Find(item.PedidoId);
                            //item.Produto = context.Produto.Find(item.ProdutoId);
                            //item.PedidoItemArquivos = context.PedidoItemArquivo.Where(a => a.PedidoItemId == item.Id).ToList();
                            item.PedidoItemUploads = context.PedidoItemUpload.Where(u => u.PedidoItemId == item.Id).ToList();

                            string output = config.PathFotos + item.Pedido.Id.ToString() + @"\" + item.Id.ToString() + @"\" + album + @"\";

                            if (!Directory.Exists(output))
                            {
                                Directory.CreateDirectory(output);
                            }

                            // Verificar se o arquivo já foi cadastrado
                            bool found = false;
                            foreach (var arq in item.PedidoItemUploads)
                            {
                                if (arq.PedidoItemId == item.Id && arq.NomeArquivo == files[0].FileName && arq.Album == album)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                // salvar o arquivo
                                string filename = item.PedidoId.ToString() + "_" + item.Id.ToString() + "_" + Guid.NewGuid().ToString() + ".jpg";
                                MemoryStream memoryStream = new MemoryStream();
                                files[0].CopyTo(memoryStream);
                                using (Image image = Image.FromStream(memoryStream))
                                {
                                    Image img = image;
                                    // Orientação da imagem
                                    try
                                    {
                                        img = ExifRotate(image);
                                    }
                                    catch (Exception ex)
                                    { }


                                    img.Save(output + filename, ImageFormat.Jpeg);

                                    // salvar registros
                                    PedidoItemUpload upload = new()
                                    {
                                        PedidoItemId = item.Id,
                                        Path = filename,
                                        NomeArquivo = files[0].FileName,
                                        Album = album,
                                        Largura = img.Size.Width,
                                        Altura = img.Size.Height,
                                        DataCadstro = DateTime.Now
                                    };

                                    context.PedidoItemUpload.Add(upload);
                                    context.Entry(upload).State = EntityState.Added;
                                    context.SaveChanges();

                                    img.Dispose();
                                    image.Dispose();
                                }
                                memoryStream.Dispose();
                            }
                        }
                    }

                    return Json(new { success = true });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult FinalizarUpload(object sender)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes != null && listaFuncoes.Contains("cad_pedidos"))
            {
                int id = Convert.ToInt32(Request.Form["id"]);
                DataContext context = new();
                PedidoItem item = context.PedidoItem.Find(id);
                if (item != null)
                {
                    item.DataUpload = DateTime.Now;
                    context.Update(item);
                    context.Entry(item).State = EntityState.Modified;
                    context.SaveChanges();

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, msg = "Item não encontrado" });
                }
            }
            else
            {
                return Json(new { success = false, msg = "Acesso negado" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AprovarPedido(object sender)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos"))
            {
                int id = Convert.ToInt32(Request.Form["id"]);
                DataContext context = new();
                PedidoItem item = context.PedidoItem.Find(id);
                if (item != null)
                {
                    item.DataAprovacao = DateTime.Now;
                    context.Update(item);
                    context.Entry(item).State = EntityState.Modified;
                    context.SaveChanges();

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, msg = "Item não encontrado" });
                }
            }
            else
            {
                return Json(new { success = false, msg = "Acesso negado" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UploadJpg(object sender)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos"))
            {
                try
                {
                    IFormFileCollection files = Request.Form.Files;
                    int id_item = Convert.ToInt32(Request.Form["id_item"]);
                    string path = Request.Form["path"];

                    if (id_item != 0)
                    {
                        DataContext context = new();
                        PedidoItem item = context.PedidoItem.Find(id_item);
                        if (item != null)
                        {
                            string album = "001";
                            if (!string.IsNullOrEmpty(path))
                            {
                                string[] strct = path.Split('/');
                                if (strct.Length >= 2) { album = strct[strct.Length - 2]; }
                            }

                            item.Pedido = context.Pedido.Find(item.PedidoId);
                            item.Produto = context.Produto.Find(item.ProdutoId);
                            item.PedidoItemArquivos = context.PedidoItemArquivo.Where(a => a.PedidoItemId == item.Id).ToList();

                            string output = config.PathFotos + item.Pedido.Id.ToString() + @"\" + item.Id.ToString() + @"\";

                            // Verificar se o arquivo já foi cadastrado
                            bool found = false;
                            foreach (var arq in item.PedidoItemArquivos)
                            {
                                if (arq.NomeArquivo == files[0].FileName && arq.Album == album)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                int prodLargura = Convert.ToInt32(item.Produto.Largura * config.Densidade());
                                int prodAltura = Convert.ToInt32(item.Produto.Altura * config.Densidade());
                                bool prodVertical = item.Produto.Largura < item.Produto.Altura;

                                MemoryStream memoryStream = new MemoryStream();
                                files[0].CopyTo(memoryStream);
                                using (Image image = Image.FromStream(memoryStream))
                                {
                                    Image img = image;

                                    // Orientação da imagem
                                    try
                                    {
                                        img = ExifRotate(image);
                                    }
                                    catch (Exception ex)
                                    { }


                                    // Orientação do arquivo
                                    int largura = img.Size.Width;
                                    int altura = img.Size.Height;
                                    bool imgVertical = largura < altura;

                                    // Verifica se é panorâmica
                                    float propProd = prodLargura / prodAltura;
                                    float propImg = largura / altura;

                                    if (propImg >= propProd * 2 && largura >= prodLargura * 2 && altura >= prodAltura && 1 != 1)
                                    {
                                        // recortar a imagem
                                        using (Bitmap target = new Bitmap(img.Width / 2, img.Height))
                                        {
                                            using (Graphics g = Graphics.FromImage(target))
                                            {
                                                g.DrawImage(img, new Rectangle(0, 0, target.Width, target.Height),
                                                    new Rectangle(0, 0, target.Width, target.Height),
                                                    GraphicsUnit.Pixel);

                                                SalvarImagem(item, target, files[0].FileName, album, "a");
                                                g.Dispose();
                                            }
                                            target.Dispose();
                                        }

                                        using (Bitmap target = new Bitmap(img.Width / 2, img.Height))
                                        {
                                            using (Graphics g = Graphics.FromImage(target))
                                            {
                                                g.DrawImage(img, new Rectangle(0, 0, target.Width, target.Height),
                                                    new Rectangle(target.Width, 0, target.Width, target.Height),
                                                    GraphicsUnit.Pixel);

                                                SalvarImagem(item, target, files[0].FileName, album, "b");
                                                g.Dispose();
                                            }
                                            target.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        SalvarImagem(item, img, files[0].FileName, album);
                                    }

                                    img.Dispose();
                                    img = null;
                                }
                            }
                            else
                            {
                                return Json(new { success = true, msg = "Arquivo já cadastrado" });
                            }
                        }
                    }

                    return Json(new { success = true });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ResetItem()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_pedidos"))
            {
                // Carrega item
                DataContext context = new();
                PedidoItem item = context.PedidoItem.Find(Convert.ToInt32(Request.Form["id"]));
                if (item != null)
                {
                    List<PedidoItemArquivo> arquivos_alb = context.PedidoItemArquivo.Where(a => a.PedidoItemId == item.Id).ToList();
                    List<string> albuns = new List<string>();
                    foreach (var arq in arquivos_alb)
                    {
                        if (!albuns.Contains(arq.Album))
                        {
                            albuns.Add(arq.Album);
                        }
                    }

                    foreach (var album in albuns)
                    {
                        string filename = config.PathFotos + "\\" + item.PedidoId + "\\" + item.Id + "\\" + album;
                        if (System.IO.Directory.Exists(filename))
                        {
                            System.IO.Directory.Delete(filename,true);
                        }
                    }

                    // Carrega arquivos
                    var arquivos = context.PedidoItemArquivo.Where(a => a.PedidoItemId == item.Id).ToList();
                    foreach (PedidoItemArquivo foto in arquivos)
                    {
                        // Deleta arquivos na pasta, incluindo os thumbs
                        string filename = config.PathFotos + "\\" + item.PedidoId + "\\" + item.Id + "\\" + foto.Album + "\\" + foto.Path;
                        if (System.IO.File.Exists(filename))
                        {
                            System.IO.File.Delete(filename);
                        }
                        if (System.IO.File.Exists(wwwPath + @"/img/thumbs/" + foto.Path))
                        {
                            System.IO.File.Delete(wwwPath + @"/img/thumbs/" + foto.Path);
                        }

                        // deleta todos os registros de arquivos
                        context.Remove(foto);
                        context.Entry(foto).State = EntityState.Deleted;
                        context.SaveChanges();
                    }



                    var uploads = context.PedidoItemUpload.Where(a => a.PedidoItemId == item.Id).ToList();
                    foreach (var foto in uploads)
                    {
                        // Deleta arquivos na pasta, incluindo os thumbs
                        string filename = config.PathFotos + "\\" + item.PedidoId + "\\" + item.Id + "\\" + foto.Album + "\\" + foto.Path;
                        if (System.IO.File.Exists(filename))
                        {
                            System.IO.File.Delete(filename);
                        }
                        if (System.IO.File.Exists(wwwPath + @"/img/thumbs/" + foto.Path))
                        {
                            System.IO.File.Delete(wwwPath + @"/img/thumbs/" + foto.Path);
                        }

                        // deleta todos os registros de arquivos
                        context.Remove(foto);
                        context.Entry(foto).State = EntityState.Deleted;
                        context.SaveChanges();
                    }


                    string pasta = config.PathFotos + "\\" + item.PedidoId + "\\" + item.Id;
                    if (System.IO.Directory.Exists(pasta))
                    {
                        System.IO.Directory.Delete(pasta,true);
                    }


                    item.DataUpload = null;
                    item.DataProcessamentoIndex = null;
                    item.DataProcessamentoPreparacao = null;
                    item.DataProcessamentoUpload = null;
                    item.DataAprovacao = null;
                    item.DataImposicao = null;

                    context.PedidoItem.Update(item);
                    context.Entry(item).State = EntityState.Modified;
                    context.SaveChanges();

                    return Json(new { success = true });
                }

                return Json(new { success = false, msg = "Item não encontrado" });
            }
            else
            {
                return Json(new { success = false, msg = "Acesso negado" });
            }
        }

        // POST: PedidoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Finalizar(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("fin_pedidos"))
            {
                try
                {
                    DataContext context = new();
                    Pedido pedido = context.Pedido.Find(id);
                    if (pedido != null)
                    {
                        pedido.DataFinalizado = DateTime.Now;

                        context.Pedido.Update(pedido);
                        context.Entry(pedido).State = EntityState.Modified;
                        context.SaveChanges();

                        return Json(new { success = true, msg = "Pedido finalizado com sucesso!" });
                    }
                    else
                    {
                        return Json(new { success = false, msg = "Pedido não encontrado!" });
                    }
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

        public ActionResult ReportOrder(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_rel_pedidos"))
            {
                DataContext context = new();
                var pedido = context.Pedido.Find(id);
                if (pedido != null)
                {
                    pedido.Cliente = context.Cliente.Find(pedido.ClienteId);
                    pedido.Cliente.Cidade = context.Cidade.Find(pedido.Cliente.CidadeId);

                    List<PedidoItem> itens = context.PedidoItem.Where(c => c.PedidoId == pedido.Id).OrderBy(p => p.Id).ToList();

                    for (int i = 0; i < itens.Count; i++)
                    {
                        itens[i].Produto = context.Produto.Find(itens[i].ProdutoId);
                        itens[i].PedidoItemArquivos = context.PedidoItemArquivo.Where(a => a.PedidoItemId == itens[i].Id).ToList();
                    }

                    ViewBag.DataEntrada = pedido.DataEntrada.ToString().Split(" ")[0];
                    ViewBag.PrevisaoEntrega = pedido.PrevisaoEntrega.ToString().Split(" ")[0];
                    ViewBag.Itens = itens;

                }

                var empresa = context.Empresa.Find(1);
                ViewBag.Empresa = empresa;
                ViewBag.Cidade = context.Cidade.Find(empresa.CidadeId);

                return View(pedido);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult ReportSpreadsheet(int id)
        {
            if (HttpContext.Session.GetString("Functions") != null)
            {
                string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
                if (listaFuncoes.Contains("ver_rel_pedidos"))
                {
                    DataContext context = new();
                    var item = context.PedidoItem.Find(id);
                    if (item != null)
                    {
                        item.Pedido = context.Pedido.Find(item.PedidoId);
                        item.Produto = context.Produto.Find(item.ProdutoId);
                        item.Pedido.Cliente = context.Cliente.Find(item.Pedido.ClienteId);
                        item.Pedido.Cliente.Cidade = context.Cidade.Find(item.Pedido.Cliente.CidadeId);
                        item.PedidoItemArquivoAlbuns = context.PedidoItemArquivoAlbum.Where(a => a.PedidoItemId == item.Id).OrderBy(a => a.Album).ToList();

                        var listaArquivos = context.PedidoItemArquivo.Where(a => a.PedidoItemId == item.Id).OrderBy(a => a.Album).ToList();
                        List<string> albuns = new();
                        foreach (var lArq in listaArquivos)
                        {
                            if (!albuns.Contains(lArq.Album))
                            {
                                albuns.Add(lArq.Album);
                            }
                        }

                        var workbook = new XLWorkbook(@"C:/Piovelli/Templates/Report.xlsx");
                        var worksheet = workbook.Worksheets.First();

                        worksheet.Cell("R2").Value = item.PedidoId.ToString()+"/"+ item.Pedido.Contrato;
                        worksheet.Cell("R3").Value = item.Pedido.Cliente.Id.ToString() + " - " + item.Pedido.Cliente.RazaoSocial;
                        worksheet.Cell("R5").Value = item.Produto.Id.ToString() + " - " + item.Produto.Titulo;
                        
                        

                        int total_album = 0;
                        int total_fotos = 0;
                        List<object> lista1 = new();
                        List<object> lista2 = new();
                        List<object> lista3 = new();
                        int cnt = 0;
                        int cntLine = 10;
                        string colAlb = "B";
                        string colVal = "D";
                        
                        foreach (var albItem in albuns)
                        {
                            if (cnt <= albuns.Count / 3)
                            {
                                colAlb = "B";
                                colVal = "D";
                                worksheet.Range("B" + cntLine.ToString() + ":C" + cntLine.ToString()).Merge();
                            }
                            else if (cnt <= albuns.Count / 3 * 2 + 1)
                            {
                                if (colAlb != "J")
                                {
                                    cntLine = 10;
                                }

                                colAlb = "J";
                                colVal = "L";
                                worksheet.Range("J" + cntLine.ToString() + ":K" + cntLine.ToString()).Merge();
                            }
                            else
                            {
                                if (colAlb != "R")
                                {
                                    cntLine = 10;
                                }

                                colAlb = "R";
                                colVal = "T";
                                worksheet.Range("R" + cntLine.ToString() + ":S" + cntLine.ToString()).Merge();
                            }
                            
                            var arquivos = listaArquivos.Where(a => a.Album == albItem).ToList();
                            worksheet.Cell(colAlb + cntLine.ToString()).Value = albItem;
                            worksheet.Cell(colVal + cntLine.ToString()).Value = arquivos.Count.ToString();

                            worksheet.Cell(colAlb + cntLine.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                            worksheet.Cell(colAlb + cntLine.ToString()).Style.Border.OutsideBorderColor = XLColor.Black;

                            worksheet.Cell(colAlb + cntLine.ToString()).Style.Fill.BackgroundColor = XLColor.DarkGray;
                            cntLine++;

                            total_album++;
                            total_fotos += item.Copias * arquivos.Count;// / item.Produto.QuantidadeArquivosProduto;
                            cnt++;
                        }

                        cntLine+=2;

                        worksheet.Cell("B" + cntLine.ToString()).Value = "Álbuns: ";
                        worksheet.Cell("B" + cntLine.ToString()).Style.Fill.BackgroundColor = XLColor.DarkGray;
                        worksheet.Cell("B" + cntLine.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Range("B" + cntLine.ToString() + ":D" + cntLine.ToString()).Merge();

                        worksheet.Cell("E" + cntLine.ToString()).Value = total_album.ToString();
                        worksheet.Range("E" + cntLine.ToString() + ":G" + cntLine.ToString()).Merge();
                        worksheet.Cell("E" + cntLine.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        worksheet.Cell("J" + cntLine.ToString()).Value = "Fotos: ";
                        worksheet.Cell("J" + cntLine.ToString()).Style.Fill.BackgroundColor = XLColor.DarkGray;
                        worksheet.Cell("J" + cntLine.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Range("J" + cntLine.ToString() + ":L" + cntLine.ToString()).Merge();

                        worksheet.Cell("M" + cntLine.ToString()).Value = total_fotos.ToString();
                        worksheet.Range("M" + cntLine.ToString() + ":O" + cntLine.ToString()).Merge();
                        worksheet.Cell("M" + cntLine.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        worksheet.Cell("R" + cntLine.ToString()).Value = "Média: ";
                        worksheet.Cell("R" + cntLine.ToString()).Style.Fill.BackgroundColor = XLColor.DarkGray;
                        worksheet.Range("R" + cntLine.ToString() + ":T" + cntLine.ToString()).Merge();
                        worksheet.Cell("R" + cntLine.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        worksheet.Cell("U" + cntLine.ToString()).Value = (total_album > 0 ? (total_fotos / total_album) : 0).ToString();
                        worksheet.Range("U" + cntLine.ToString() + ":W" + cntLine.ToString()).Merge();
                        worksheet.Cell("U" + cntLine.ToString()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        string path = config.PathFotos + item.Pedido.Id.ToString() + @"\" + item.Id.ToString() + @"\Report.xlsx";
                        workbook.SaveAs(path);

                        FileStream fileStream;

                        try
                        {
                            fileStream = System.IO.File.OpenRead(path);
                        }
                        catch (DirectoryNotFoundException)
                        {
                            return new EmptyResult();
                        }

                        return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Report.xlsx");
                    }

                    return View(item);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private void SalvarImagem(PedidoItem item, Image img, string fName, string album, string complemento = "")
        {
            try
            {
                int prodLargura = Convert.ToInt32(item.Produto.Largura * config.Densidade());
                int prodAltura = Convert.ToInt32(item.Produto.Altura * config.Densidade());
                bool prodVertical = item.Produto.Largura < item.Produto.Altura;

                // Orientação do arquivo
                bool imgVertical = img.Size.Width < img.Size.Height;

                if (prodVertical != imgVertical)
                {
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                }

                // Redimensionar
                img = ResizeImage(img, prodLargura, prodAltura);

                // Salvar
                string filename = item.PedidoId.ToString() + "_" + item.Id.ToString() + "_" + Guid.NewGuid().ToString() + ".jpg";
                string nomeArquivo = fName;
                if (!String.IsNullOrWhiteSpace(complemento))
                {
                    string[] strct = fName.Split('.');
                    strct[strct.Length - 2] += complemento;
                    nomeArquivo = string.Join(".", strct);
                }

                // Salva registros no banco
                PedidoItemArquivo arq = new()
                {
                    PedidoItemId = item.Id,
                    Sequencia = 0,
                    Album = album,
                    Path = filename,
                    NomeArquivo = nomeArquivo,
                    DataUpload = DateTime.Now
                };

                DataContext context = new();
                context.PedidoItemArquivo.Add(arq);
                context.Entry(arq).State = EntityState.Added;
                context.SaveChanges();

                using (Bitmap newBitmap = new Bitmap(img))
                {
                    newBitmap.SetResolution((float)config.DPI, (float)config.DPI);
                    newBitmap.Save(config.PathFotos + item.PedidoId.ToString() + @"/" + item.Id.ToString() + @"/" + filename, ImageFormat.Jpeg);
                    newBitmap.Dispose();
                }

                int thumbLarg = 600 * 100;
                int thumbAlt = Convert.ToInt32(thumbLarg / img.Size.Width * img.Size.Height);
                Image thumb = img.GetThumbnailImage(thumbLarg / 100, thumbAlt / 100, () => false, IntPtr.Zero);

                thumb.Save(wwwPath + @"/img/thumbs/" + filename, ImageFormat.Jpeg);

                thumb.Dispose();

                img.Dispose();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                img.Dispose();
            }
        }

        public static Image ExifRotate(Image img)
        {
            const int exifOrientationID = 0x112; //274

            //if (!img.PropertyIdList.Contains(exifOrientationID))
            //    return;

            var prop = img.GetPropertyItem(exifOrientationID);
            int val = BitConverter.ToUInt16(prop.Value, 0);
            var rot = RotateFlipType.RotateNoneFlipNone;

            if (val == 3 || val == 4)
                rot = RotateFlipType.Rotate180FlipNone;
            else if (val == 5 || val == 6)
                rot = RotateFlipType.Rotate90FlipNone;
            else if (val == 7 || val == 8)
                rot = RotateFlipType.Rotate270FlipNone;

            if (val == 2 || val == 4 || val == 5 || val == 7)
                rot |= RotateFlipType.RotateNoneFlipX;

            if (rot != RotateFlipType.RotateNoneFlipNone)
            {
                img.RotateFlip(rot);
                img.RemovePropertyItem(exifOrientationID);
            }

            return img;
        }

        public static Image ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return (Image)destImage;
        }

    }
}
