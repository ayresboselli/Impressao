using Automacao.Models;

using Microsoft.EntityFrameworkCore;
using NetBarcode;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SixLabors.Fonts;

namespace Automacao
{
    class Imposicao
    {
        private static Config config;

        private static List<ProdutoInformacao> Informacoes(PedidoItem item, PedidoItemArquivoAlbum arqAlbum, int cnt, int pag)
        {
            DataContext context = new();
            List<ProdutoInformacao> informacoes = context.ProdutoInformacao.Where(i => i.ProdutoId == item.ProdutoId).ToList();
            for (int i = 0; i < informacoes.Count; i++)
            {
                if (informacoes[i].Pagina == pag)
                {
                    switch (informacoes[i].Texto)
                    {
                        case "{pedido}": informacoes[i].Texto = item.PedidoId.ToString().PadLeft(5, '0'); break;
                        case "{op}": informacoes[i].Texto = item.Id.ToString().PadLeft(5, '0'); break;
                        case "{id_album}": informacoes[i].Texto = arqAlbum.Id.ToString().PadLeft(5, '0'); break;
                        case "{nome_album}": informacoes[i].Texto = arqAlbum.Album; break;
                        case "{nome_pdf}": informacoes[i].Texto = arqAlbum.NomeArquivo; break;
                        case "{seq_foto}": informacoes[i].Texto = (cnt + 1).ToString().PadLeft(5, '0'); break;
                    }
                }
            }

            return informacoes;
        }

        private static PdfDocument Frente(PedidoItem item, PedidoItemArquivoAlbum arqAlbum)
        {

            //Create PDF Document
            PdfDocument document = new PdfDocument();

            int cnt = 0;
            foreach (var arq in item.PedidoItemArquivos)
            {
                if (arq.Album == arqAlbum.Album)
                {
                    double deslX = config.Densidade((double)item.Produto.DeslocamentoFrenteX);
                    double deslY = config.Densidade((double)item.Produto.DeslocamentoFrenteY);


                    // Atualiza informações
                    List<ProdutoInformacao> informacoes = Informacoes(item, arqAlbum, cnt, 0);


                    //You will have to add Page in PDF Document
                    PdfPage page = document.AddPage();
                    page.Width = config.Densidade(item.Produto.LarguraMidia);
                    page.Height = config.Densidade(item.Produto.AlturaMidia);


                    // Imagem
                    string imageFile = config.PathFotos + item.PedidoId.ToString() + "/" + item.Id.ToString() + "/" + arq.Path;
                    XImage foto = XImage.FromFile(imageFile);
                    //For drawing in PDF Page you will nedd XGraphics Object
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    gfx.DrawImage(foto, deslX, deslY, foto.Width, foto.Height);


                    // Informações
                    XFont font = new XFont("Arial", 32, XFontStyle.Regular);
                    foreach (var info in informacoes)
                    {
                        if (info.Pagina == 0)
                        {
                            XSize size = gfx.MeasureString(info.Texto, font);
                            gfx.RotateAtTransform(360 - info.Orientacao, new XPoint(config.Densidade(info.PosX), (config.Densidade(info.PosY)) - (size.Height / 2)));

                            if (info.Texto == "{nome_arquivo}")
                            {
                                info.Texto = arq.NomeArquivo;
                            }

                            if (info.Tipo == "barras")
                            {
                                string fileBarcode = AppDomain.CurrentDomain.BaseDirectory + "temp/barcode_" + info.Texto + ".jpg";
                                if (!File.Exists(fileBarcode))
                                {
                                    FontCollection collection = new();
                                    SixLabors.Fonts.FontFamily family = collection.Add(AppDomain.CurrentDomain.BaseDirectory + "OpenSans-Regular.ttf");
                                    SixLabors.Fonts.Font BCFont = family.CreateFont(8, SixLabors.Fonts.FontStyle.Regular);

                                    var barcode = new Barcode(info.Texto, NetBarcode.Type.Code128, true, 200, 100, BCFont);
                                    barcode.SaveImageFile(fileBarcode);
                                }

                                XImage XBarcode = XImage.FromFile(fileBarcode);
                                double deslBarX = config.Densidade(info.PosX) - (XBarcode.Width / 2);
                                double deslBarY = config.Densidade(info.PosY) - (XBarcode.Height / 2);
                                gfx.DrawImage(XBarcode, deslBarX, deslBarY, XBarcode.Width, XBarcode.Height);
                            }
                            else
                            {
                                gfx.DrawString(info.Texto, font, XBrushes.Black, config.Densidade(info.PosX), config.Densidade(info.PosY));
                            }
                            gfx.RotateAtTransform(info.Orientacao, new XPoint(config.Densidade(info.PosX), config.Densidade(info.PosY) - (size.Height / 2)));
                        }
                    }


                    foto.Dispose();
                    gfx.Dispose();
                    cnt++;
                }
            }

            return document;
        }

        private static PdfDocument FrenteVerso(PedidoItem item, PedidoItemArquivoAlbum arqAlbum)
        {

            //Create PDF Document
            PdfDocument document = new PdfDocument();

            int cnt = 0;
            foreach (var arq in item.PedidoItemArquivos)
            {
                if (arq.Album == arqAlbum.Album)
                {
                    double deslX = 0;
                    double deslY = 0;
                    int pag = 0;

                    if (cnt % 2 == 0)
                    {
                        deslX = config.Densidade((double)item.Produto.DeslocamentoFrenteX);
                        deslY = config.Densidade((double)item.Produto.DeslocamentoFrenteY);
                    }
                    else
                    {
                        pag = 1;
                        deslX = config.Densidade((double)item.Produto.DeslocamentoVersoX);
                        deslY = config.Densidade((double)item.Produto.DeslocamentoVersoY);
                    }


                    // Atualiza informações
                    List<ProdutoInformacao> informacoes = Informacoes(item, arqAlbum, cnt, pag);


                    //You will have to add Page in PDF Document
                    PdfPage page = document.AddPage();
                    page.Width = new XUnit(item.Produto.LarguraMidia, XGraphicsUnit.Millimeter);
                    page.Height = new XUnit(item.Produto.AlturaMidia, XGraphicsUnit.Millimeter);
                    //page.Width = config.Densidade(item.Produto.LarguraMidia);
                    //page.Height = config.Densidade(item.Produto.AlturaMidia);


                    // Imagem
                    string imageFile = config.PathFotos + item.PedidoId.ToString() + "/" + item.Id.ToString() + "/" + arq.Path;
                    XImage foto = XImage.FromFile(imageFile);

                    //For drawing in PDF Page you will nedd XGraphics Object
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    gfx.DrawImage(foto, deslX, deslY, foto.Width, foto.Height);


                    // Informações
                    XFont font = new XFont("Arial", 32, XFontStyle.Regular);
                    foreach (var info in informacoes)
                    {
                        if (info.Pagina == pag)
                        {
                            XSize size = gfx.MeasureString(info.Texto, font);
                            gfx.RotateAtTransform(360 - info.Orientacao, new XPoint(config.Densidade(info.PosX), config.Densidade(info.PosY) - (size.Height / 2)));

                            if(info.Texto == "{nome_arquivo}")
                            {
                                info.Texto = arq.NomeArquivo;
                            }

                            if (info.Tipo == "barras")
                            {
                                string fileBarcode = AppDomain.CurrentDomain.BaseDirectory + "temp/barcode_" + info.Texto + ".jpg";
                                if (!File.Exists(fileBarcode))
                                {
                                    FontCollection collection = new();
                                    SixLabors.Fonts.FontFamily family = collection.Add("OpenSans-Regular.ttf");
                                    SixLabors.Fonts.Font BCFont = family.CreateFont(8, SixLabors.Fonts.FontStyle.Regular);

                                    var barcode = new Barcode(info.Texto, NetBarcode.Type.Code128, true, 200, 100, BCFont);
                                    barcode.SaveImageFile(fileBarcode);
                                }

                                XImage XBarcode = XImage.FromFile(fileBarcode);
                                double deslBarX = config.Densidade(info.PosX) - (XBarcode.Width / 2);
                                double deslBarY = config.Densidade(info.PosY) - (XBarcode.Height / 2);
                                gfx.DrawImage(XBarcode, deslBarX, deslBarY, XBarcode.Width, XBarcode.Height);
                            }
                            else
                            {
                                gfx.DrawString(info.Texto, font, XBrushes.Black, config.Densidade(info.PosX), config.Densidade(info.PosY));
                            }
                            gfx.RotateAtTransform(info.Orientacao, new XPoint(config.Densidade(info.PosX), config.Densidade(info.PosY) - (size.Height / 2)));
                        }
                    }

                    foto.Dispose();
                    gfx.Dispose();
                    cnt++;
                }
            }

            return document;
        }

        private static PdfDocument FrenteInfoVerso(PedidoItem item, PedidoItemArquivoAlbum arqAlbum)
        {

            //Create PDF Document
            PdfDocument document = new PdfDocument();

            int cnt = 0;
            foreach (var arq in item.PedidoItemArquivos)
            {
                if (arq.Album == arqAlbum.Album)
                {
                    double deslX = config.Densidade((double)item.Produto.DeslocamentoFrenteX);
                    double deslY = config.Densidade((double)item.Produto.DeslocamentoFrenteY);


                    // FRENTE


                    //Cria página da fente
                    PdfPage pageF = document.AddPage();
                    pageF.Width = config.Densidade(item.Produto.LarguraMidia);
                    pageF.Height = config.Densidade(item.Produto.AlturaMidia);


                    List<ProdutoInformacao> informacoesFrente = Informacoes(item, arqAlbum, cnt, 1);

                    // Imagem
                    string imageFile = config.PathFotos + item.PedidoId.ToString() + "/" + item.Id.ToString() + "/" + arq.Path;
                    XImage foto = XImage.FromFile(imageFile);
                    //For drawing in PDF Page you will nedd XGraphics Object
                    XGraphics gfxF = XGraphics.FromPdfPage(pageF);
                    gfxF.DrawImage(foto, deslX, deslY, foto.Width, foto.Height);


                    // Informações
                    XFont font = new XFont("Arial", 32, XFontStyle.Regular);
                    foreach (var info in informacoesFrente)
                    {
                        if (info.Pagina == 0)
                        {
                            XSize size = gfxF.MeasureString(info.Texto, font);
                            gfxF.RotateAtTransform(360 - info.Orientacao, new XPoint(config.Densidade(info.PosX), config.Densidade(info.PosY) - (size.Height / 2)));

                            if (info.Texto == "{nome_arquivo}")
                            {
                                info.Texto = arq.NomeArquivo;
                            }

                            if (info.Tipo == "barras")
                            {
                                string fileBarcode = AppDomain.CurrentDomain.BaseDirectory + "temp/barcode_" + info.Texto + ".jpg";
                                if (!System.IO.File.Exists(fileBarcode))
                                {
                                    FontCollection collection = new();
                                    SixLabors.Fonts.FontFamily family = collection.Add(AppDomain.CurrentDomain.BaseDirectory + "OpenSans-Regular.ttf");
                                    SixLabors.Fonts.Font BCFont = family.CreateFont(8, SixLabors.Fonts.FontStyle.Regular);

                                    var barcode = new Barcode(info.Texto, NetBarcode.Type.Code128, true, 200, 100, BCFont);
                                    barcode.SaveImageFile(fileBarcode);
                                }

                                XImage XBarcode = XImage.FromFile(fileBarcode);
                                double deslBarX = config.Densidade(info.PosX) - (XBarcode.Width / 2);
                                double deslBarY = config.Densidade(info.PosY) - (XBarcode.Height / 2);
                                gfxF.DrawImage(XBarcode, deslBarX, deslBarY, XBarcode.Width, XBarcode.Height);
                            }
                            else
                            {
                                gfxF.DrawString(info.Texto, font, XBrushes.Black, config.Densidade(info.PosX), config.Densidade(info.PosY));
                            }
                            gfxF.RotateAtTransform(info.Orientacao, new XPoint(config.Densidade(info.PosX), config.Densidade(info.PosY) - (size.Height / 2)));
                        }
                    }


                    foto.Dispose();
                    gfxF.Dispose();


                    // VERSO


                    //Cria página do verso
                    PdfPage pageV = document.AddPage();
                    pageV.Width = config.Densidade(item.Produto.LarguraMidia);
                    pageV.Height = config.Densidade(item.Produto.AlturaMidia);


                    List<ProdutoInformacao> informacoesVerso = Informacoes(item, arqAlbum, cnt, 1);


                    XGraphics gfxV = XGraphics.FromPdfPage(pageV);

                    // Informações
                    foreach (var info in informacoesFrente)
                    {
                        XSize size = gfxV.MeasureString(info.Texto, font);
                        gfxV.RotateAtTransform(360 - info.Orientacao, new XPoint(config.Densidade(info.PosX), config.Densidade(info.PosY) - (size.Height / 2)));

                        if (info.Tipo == "barras")
                        {
                            string fileBarcode = AppDomain.CurrentDomain.BaseDirectory + "temp/barcode_" + info.Texto + ".jpg";
                            if (!System.IO.File.Exists(fileBarcode))
                            {
                                FontCollection collection = new();
                                SixLabors.Fonts.FontFamily family = collection.Add("OpenSans-Regular.ttf");
                                SixLabors.Fonts.Font BCFont = family.CreateFont(8, SixLabors.Fonts.FontStyle.Regular);

                                var barcode = new Barcode(info.Texto, NetBarcode.Type.Code128, true, 200, 100, BCFont);
                                barcode.SaveImageFile(fileBarcode);
                            }

                            XImage XBarcode = XImage.FromFile(fileBarcode);
                            double deslBarX = config.Densidade(info.PosX) - (XBarcode.Width / 2);
                            double deslBarY = config.Densidade(info.PosY) - (XBarcode.Height / 2);
                            gfxV.DrawImage(XBarcode, deslBarX, deslBarY, XBarcode.Width, XBarcode.Height);
                        }
                        else
                        {
                            gfxV.DrawString(info.Texto, font, XBrushes.Black, config.Densidade(info.PosX), config.Densidade(info.PosY));
                        }   
                        gfxV.RotateAtTransform(info.Orientacao, new XPoint(config.Densidade(info.PosX), config.Densidade(info.PosY) - (size.Height / 2)));
                    }

                    gfxV.Dispose();

                    cnt++;
                }
            }

            return document;
        }

        public static void Imposicionar()
        {
            DataContext context = new();

            config = context.Config.Find(1);

            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "temp"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "temp");
            }

            if (!Directory.Exists(config.PathPdf))
            {
                Directory.CreateDirectory(config.PathPdf);
            }

            var itens = context.PedidoItem.Where(p => p.DataAprovacao != null && p.DataImposicao == null).ToList();

            foreach (var item in itens)
            {
                try
                {
                    item.Produto = context.Produto.Find(item.ProdutoId);
                    //item.Produto.Informacoes = context.ProdutoInformacao.Where(i => i.ProdutoId == item.ProdutoId).ToList();
                    item.PedidoItemArquivos = context.PedidoItemArquivo.Where(a => a.PedidoItemId == item.Id).OrderBy(a => a.NomeArquivo).ToList();


                    List<string> albuns = new();
                    foreach (var arq in item.PedidoItemArquivos)
                    {
                        if (!albuns.Contains(arq.Album))
                        {
                            albuns.Add(arq.Album);
                        }
                    }

                    // album por album
                    foreach (var alb in albuns)
                    {
                        //Specify file name of the PDF file
                        string filename = item.PedidoId.ToString() + "_" + item.Id.ToString() + "_" + alb + ".pdf";


                        // Cria registro do Álbum
                        PedidoItemArquivoAlbum arqAlbum;
                        var arqAlbumList = context.PedidoItemArquivoAlbum.Where(a => a.PedidoItemId == item.Id && a.Album == alb).ToList();
                        if (arqAlbumList.Count == 0)
                        {
                            arqAlbum = new()
                            {
                                PedidoItemId = item.Id,
                                Album = alb,
                                NomeArquivo = filename.Split('.')[0],
                                DataCadastro = DateTime.Now
                            };

                            context.PedidoItemArquivoAlbum.Add(arqAlbum);
                            context.Entry(arqAlbum).State = EntityState.Added;
                            context.SaveChanges();
                        }
                        else
                        {
                            arqAlbum = arqAlbumList.First();
                        }


                        PdfDocument document = null;
                        if (item.Produto.DisposicaoImpressao == 'D')
                        {
                            if (item.Produto.DisposicaoImagem == 'D')
                            {
                                document = FrenteVerso(item, arqAlbum);
                            }
                            else
                            {
                                document = FrenteInfoVerso(item, arqAlbum);
                            }
                        }
                        else
                        {
                            document = Frente(item, arqAlbum);
                        }


                        document.Save(config.PathPdf + filename);
                        document.Dispose();

                        arqAlbum.DataProcessamento = DateTime.Now;
                        context.PedidoItemArquivoAlbum.Update(arqAlbum);
                        context.Entry(arqAlbum).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    item.DataImposicao = DateTime.Now;
                    context.PedidoItem.Update(item);
                    context.Entry(item).State = EntityState.Modified;
                    context.SaveChanges();
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }

        }
    }
}
