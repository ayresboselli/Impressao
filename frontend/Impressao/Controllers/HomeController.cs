using DocumentFormat.OpenXml.Drawing.Charts;
using Impressao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Impressao.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PedidosAtivos()
        {
            DataContext context = new();
            List<Pedido> pedidos = context.Pedido.Where(p => p.DataFinalizado == null).ToList();

            return Json(new { PedidosAtivos = pedidos.Count });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PedidosFinalizados()
        {
            DataContext context = new();
            int finalizado = context.Pedido.Where(p => p.DataFinalizado != null).Count();

            return Json(new { PedidosFinalizados = finalizado });
        }
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PedidosMes()
        {
            DataContext context = new();
            var pedidos = context.Pedido.FromSqlRaw("SELECT count(\"Id\") \"Id\", \"DataEntrada\"::date, \"ClienteId\", \"TabelaId\" FROM public.\"Pedido\" GROUP BY \"DataEntrada\"::date").ToList();
            return Json(new { Pedidos = pedidos });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AlbunsMes()
        {
            DataContext context = new();
            var pedidos = context.Pedido.FromSqlRaw("SELECT count(\"Id\") \"Id\", \"DataCadastro\"::date FROM public.\"PedidoItemArquivoAlbum\" GROUP BY \"DataCadastro\"::date").ToList();
            return Json(new { Pedidos = pedidos });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult FotosMes()
        {
            DataContext context = new();
            var pedidos = context.Pedido.FromSqlRaw("SELECT count(\"Id\") \"Id\", \"DataUpload\"::date FROM public.\"PedidoItemArquivo\" GROUP BY \"DataUpload\"::date").ToList();
            return Json(new { Pedidos = pedidos });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ProdutosTop5Pedidos()
        {
            string sql = "SELECT \"prd\".\"Id\" || ' - ' || \"prd\".\"Titulo\" produto,\r\ncount(\"o\".\"Id\") quantidade" +
                "FROM public.\"Pedido\" AS \"o\"" +
                "JOIN public.\"PedidoItem\" AS \"i\" ON \"o\".\"Id\" = \"i\".\"PedidoId\"" +
                "JOIN public.\"Produto\" AS \"prd\" ON \"i\".\"ProdutoId\" = \"prd\".\"Id\"" +
                "WHERE \"o\".\"DataEntrada\" BETWEEN CURRENT_DATE -30 AND CURRENT_DATE" +
                "GROUP BY \"prd\".\"Id\" ORDER BY count(\"o\".\"Id\") DESC LIMIT 5";

            DataContext context = new();
            var produtos = context.Produto.FromSqlRaw(sql).ToList();
            return Json(new { Produtos = produtos });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ProdutosTop5Fotos()
        {
            string sql = "SELECT \"prd\".\"Id\" || ' - ' || \"prd\".\"Titulo\" produto, count(\"a\".\"Id\") quantidade" +
                "FROM public.\"PedidoItem\" AS \"i\"" +
                "JOIN public.\"Produto\" AS \"prd\" ON \"i\".\"ProdutoId\" = \"prd\".\"Id\"" +
                "JOIN public.\"PedidoItemArquivo\" AS \"a\" ON \"i\".\"Id\" = \"a\".\"PedidoItemId\"" +
                "WHERE \"a\".\"DataUpload\" BETWEEN CURRENT_DATE -30 AND CURRENT_DATE" +
                "GROUP BY \"prd\".\"Id\" ORDER BY count(\"a\".\"Id\") DESC LIMIT 5";
            DataContext context = new();
            var produtos = context.Produto.FromSqlRaw(sql).ToList();
            return Json(new { Produtos = produtos });
        }
        */
    }
}