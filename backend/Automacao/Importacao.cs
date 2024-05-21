using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using Automacao.Models;
using System.Threading;
using SixLabors.ImageSharp.Memory;

namespace Automacao
{
    class Importacao
    {
        private static Config config;
        static List<PedidoItemArquivo> arquivos = new();
        static List<PedidoItemUpload> processadas = new();
        static int vCPU;
        static int threadCount = 0;

        static void ThreadProcessaImagemSimples(PedidoItem item, PedidoItemUpload foto, string fileOrigem, PedidoItemArquivo arq)
        {
            try
            {
                using (Image img = Image.FromFile(fileOrigem))
                {
                    SalvarImagem(item, img, arq.Path);
                    processadas.Add(foto);
                    img.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                threadCount--;
            }
        }

        static void ThreadProcessaImagemPanoramicas(PedidoItem item, PedidoItemUpload foto, string fileOrigem, PedidoItemArquivo arqA, PedidoItemArquivo arqB)
        {
            try
            {
                using (Image img = Image.FromFile(fileOrigem))
                {
                    // recorte de panoramica
                    using (Bitmap target = new Bitmap(img.Width / 2, img.Height))
                    {
                        using (Graphics g = Graphics.FromImage(target))
                        {
                            g.DrawImage(img, new Rectangle(0, 0, target.Width, target.Height),
                                new Rectangle(0, 0, target.Width, target.Height),
                                GraphicsUnit.Pixel);

                            SalvarImagem(item, target, arqA.Path);
                            g.Dispose();

                            processadas.Add(foto);
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

                            SalvarImagem(item, target, arqB.Path);
                            g.Dispose();

                            processadas.Add(foto);
                        }
                        target.Dispose();
                    }

                    img.Dispose();
                }

                File.Delete(fileOrigem);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                threadCount--;
            }
        }




        private static void SalvarImagem(PedidoItem item, Image img, string filename)
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

                using (Bitmap newBitmap = new Bitmap(img))
                {
                    newBitmap.SetResolution((float)config.DPI, (float)config.DPI);
                    newBitmap.Save(config.PathFotos + item.PedidoId.ToString() + @"/" + item.Id.ToString() + @"/" + filename, ImageFormat.Jpeg);
                    newBitmap.Dispose();
                }

                int thumbLarg = 600 * 100;
                int thumbAlt = Convert.ToInt32(thumbLarg / img.Size.Width * img.Size.Height);
                Image thumb = img.GetThumbnailImage(thumbLarg / 100, thumbAlt / 100, () => false, IntPtr.Zero);

                thumb.Save(config.WwwPath + @"/img/thumbs/" + filename, ImageFormat.Jpeg);

                thumb.Dispose();
                thumb = null;
            }
            catch (Exception ex)
            {
                //throw ex;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                img.Dispose();
                img = null;
            }
        }

        public static Image ResizeImage(Image image, int width, int height)
        {
            try
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return image;
            }
        }





        private static void Index(PedidoItem item)
        {
            DataContext context = new();

            try
            {
                item.Produto = context.Produto.Find(item.ProdutoId);
                item.PedidoItemUploads = context.PedidoItemUpload.Where(a => a.PedidoItemId == item.Id).ToList();


                List<string> albuns = new();
                foreach (var arq in item.PedidoItemUploads)
                {
                    if (!albuns.Contains(arq.Album))
                    {
                        albuns.Add(arq.Album);
                    }
                }


                // arquivos por página
                double largPag = item.Produto.LarguraMidia - config.MargemMidia;
                double altPag = item.Produto.AlturaMidia - config.MargemMidia;
                double largThumb = config.LarguraThumb + config.MargemThumb * 2;
                double altThumb = config.AlturaThumb + config.MargemThumb * 2;
                double thumbPorPag = Math.Floor((largPag / largThumb) * (altPag / (altThumb + config.TamanhoFonte)));

                //int posXMedia = Convert.ToInt32((largPag - ((largPag / largThumb) * largThumb)) / 2);

                foreach (var alb in albuns)
                {
                    // arquivos por album
                    int cntArq = 0;
                    int cntPag = 0;
                    var arqList = context.PedidoItemUpload.Where(a => a.PedidoItemId == item.Id && a.Album == alb).OrderBy(a => a.NomeArquivo).ToList();

                    Bitmap bitmap = null;
                    while (cntArq < arqList.Count)
                    {
                        // criar a página
                        bitmap = new Bitmap(Convert.ToInt32(config.Densidade(item.Produto.LarguraMidia)), Convert.ToInt32(config.Densidade(item.Produto.AlturaMidia)));
                        using (Graphics graph = Graphics.FromImage(bitmap))
                        {
                            Rectangle ImageSize = new Rectangle(0, 0, Convert.ToInt32(config.Densidade(item.Produto.LarguraMidia)), Convert.ToInt32(config.Densidade(item.Produto.AlturaMidia)));
                            graph.FillRectangle(Brushes.White, ImageSize);


                            // criar o thumb
                            int cntArqCol = 0;
                            int cntArqPag = 0;
                            int posX = Convert.ToInt32(config.Densidade(config.MargemMidia));
                            int posY = Convert.ToInt32(config.Densidade(config.MargemMidia));
                            while (cntArqPag < thumbPorPag && cntArq < arqList.Count)
                            //foreach (var arq in arqList)
                            {
                                Image img = Image.FromFile(config.PathFotos + item.PedidoId.ToString() + "/" + item.Id.ToString() + "/" + arqList[cntArq].Path);

                                var thumb = new Bitmap(Convert.ToInt32(config.Densidade(largThumb)), Convert.ToInt32(config.Densidade(altThumb)));
                                var thumbG = Graphics.FromImage(thumb);
                                Rectangle thumbSize = new Rectangle(0, 0, Convert.ToInt32(config.Densidade(largThumb)), Convert.ToInt32(config.Densidade(largThumb)));
                                thumbG.FillRectangle(Brushes.White, thumbSize);

                                int imgLarg = 0;
                                int imgAlt = 0;
                                int tPosX = 0;
                                int tPosY = 0;

                                if (img.Size.Width > img.Size.Height)
                                {
                                    imgLarg = Convert.ToInt32(config.Densidade(config.LarguraThumb));
                                    imgAlt = Convert.ToInt32(config.Densidade(config.LarguraThumb) / img.Size.Width * img.Size.Height);
                                }
                                else
                                {
                                    imgLarg = Convert.ToInt32(config.Densidade(config.AlturaThumb) / img.Size.Height * img.Size.Width);
                                    imgAlt = Convert.ToInt32(config.Densidade(config.AlturaThumb));
                                }


                                if (img.Size.Width > img.Size.Height)
                                {
                                    imgAlt = Convert.ToInt32(((double)img.Size.Height / (double)img.Size.Width) * config.Densidade(config.AlturaThumb));
                                }
                                else
                                {
                                    imgLarg = Convert.ToInt32(((double)img.Size.Width / (double)img.Size.Height) * config.Densidade(config.LarguraThumb));
                                }

                                tPosX = Convert.ToInt32((config.Densidade(largThumb) - (double)imgLarg) / 2);
                                tPosY = Convert.ToInt32((config.Densidade(altThumb) - (double)imgAlt) / 2);

                                Image thumbImg = img.GetThumbnailImage(imgLarg, imgAlt, () => false, IntPtr.Zero);

                                thumbG.DrawImage(thumbImg, tPosX, tPosY);

                                // Texto no thumbnail
                                SizeF stringSize = new();
                                stringSize = thumbG.MeasureString(arqList[cntArq].NomeArquivo, new Font("Arial", config.TamanhoFonte));
                                thumbG.DrawString(arqList[cntArq].NomeArquivo, new Font("Arial", config.TamanhoFonte), Brushes.Black, new PointF((float)(config.Densidade(largThumb) - stringSize.Width) / 2, (float)config.Densidade(altThumb) - 20));


                                // inserir o thumb na página
                                if (cntArqCol > 0)
                                {
                                    posX += Convert.ToInt32(config.Densidade(largThumb));
                                    //if(posX < posXMedia)
                                    //{
                                    //    posX = posXMedia;
                                    //}
                                }

                                if ((largPag / largThumb) < cntArqCol + 1)
                                {
                                    posX = Convert.ToInt32(config.Densidade(config.MargemMidia));
                                    posY += Convert.ToInt32(config.Densidade(altThumb));
                                    cntArqCol = 0;
                                }

                                graph.DrawImage(thumb, posX, posY);

                                img.Dispose();
                                thumb.Dispose();
                                thumbG.Dispose();

                                cntArqCol++;
                                cntArqPag++;
                                cntArq++;
                            }

                            graph.Dispose();
                        }

                        // salva o arquivo
                        string filename = item.PedidoId.ToString() + "_" + item.Id.ToString() + "_" + Guid.NewGuid().ToString() + ".jpg";
                        string nomeArquivo = "zzzzzz" + cntPag.ToString() + ".jpg";

                        bitmap.Save(config.PathFotos + item.PedidoId.ToString() + "/" + item.Id.ToString() + "/" + filename, ImageFormat.Jpeg);

                        // Add on upload
                        PedidoItemUpload upload = new()
                        {
                            PedidoItemId = item.Id,
                            Path = filename,
                            NomeArquivo = nomeArquivo,
                            Album = alb,
                            Largura = Convert.ToInt32(config.Densidade(item.Produto.LarguraMidia)),
                            Altura = Convert.ToInt32(config.Densidade(item.Produto.AlturaMidia)),
                            Rotacionar = false,
                            Panoramica = false,
                            DataCadstro = DateTime.Now
                        };

                        context.PedidoItemUpload.Add(upload);
                        context.Entry(upload).State = EntityState.Added;
                        context.SaveChanges();

                        cntPag++;
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

        private static void PreparaImagensUpload(int id)
        {
            DataContext context = new();

            try
            {
                PedidoItem item = context.PedidoItem.Find(id);
                if (item != null)
                {
                    item.Produto = context.Produto.Find(item.ProdutoId);
                    item.PedidoItemUploads = context.PedidoItemUpload.Where(a => a.PedidoItemId == item.Id).ToList();

                    //proporção do produto
                    float proporcao = (float)item.Produto.Largura / (float)item.Produto.Altura;

                    List<PedidoItemUpload> fotoProporcao = new List<PedidoItemUpload>();
                    List<PedidoItemUpload> fotoNaoProporcao = new List<PedidoItemUpload>();
                    List<object> dimensoes = new List<object>();

                    //agrupar as fotos por tamanhos, conforme a proporção, e considerar o maior grupo

                    List<Agrupado> agrupado = new();
                    foreach (var foto in item.PedidoItemUploads)
                    {
                        float fotoProp = (float)foto.Largura / (float)foto.Altura;
                        double fotoPropMin = fotoProp - fotoProp * 0.1;
                        double fotoPropMax = fotoProp * 1.1;
                        if (fotoPropMin <= proporcao && fotoPropMax >= proporcao)
                        {
                            fotoProporcao.Add(foto);

                            bool encontrou = false;
                            for (int i = 0; i < agrupado.Count; i++)
                            {
                                if (agrupado[i].Largura == foto.Largura && agrupado[i].Altura == foto.Altura)
                                {
                                    agrupado[i].Count++;
                                    encontrou = true;
                                }
                            }

                            if (!encontrou)
                            {
                                agrupado.Add(new Agrupado()
                                {
                                    Count = 1,
                                    Largura = foto.Largura,
                                    Altura = foto.Altura
                                });
                            }
                        }
                        else
                        {
                            fotoNaoProporcao.Add(foto);
                        }
                    }

                    Agrupado dimensoesPadrao = new() { Count = 0 };
                    for (int x = 0; x < agrupado.Count; x++)
                    {
                        if (agrupado[x].Count > dimensoesPadrao.Count)
                        {
                            dimensoesPadrao = agrupado[x];
                        }
                    }


                    foreach (var imagem in fotoNaoProporcao)
                    {
                        //se as alturas forem iguais e a largura da foto for 2x a largura do padrão, então é panoramica
                        if (
                            imagem.Altura == dimensoesPadrao.Altura &&
                            (float)imagem.Largura / 2 >= (float)dimensoesPadrao.Largura - ((float)dimensoesPadrao.Largura * 0.1) &&
                            (float)imagem.Largura / 2 <= (float)dimensoesPadrao.Largura * 1.1
                        )
                        {
                            imagem.Panoramica = true;
                            context.PedidoItemUpload.Update(imagem);
                            context.Entry(imagem).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                        else
                        {
                            //Para as demais, se a largura e altura invertidas da foto for igual a largura e altura padrão, então dar o tombo
                            if (
                                ((float)imagem.Largura / (float)imagem.Altura < 1 && proporcao > 1) ||
                                ((float)imagem.Largura / (float)imagem.Altura > 1 && proporcao < 1)
                            )
                            {
                                imagem.Rotacionar = true;
                                context.PedidoItemUpload.Update(imagem);
                                context.Entry(imagem).State = EntityState.Modified;
                                context.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                context.Database.CloseConnection();
                context.Dispose();
            }
        }

        private static void ProcessaImagensUpload(int id)
        {
            DataContext context = new();
            
            try
            {
                PedidoItem item = context.PedidoItem.Find(id);
                if (item != null)
                {
                    item.Produto = context.Produto.Find(item.ProdutoId);

                    List<PedidoItemUpload> fotos = context.PedidoItemUpload.Where(a => a.PedidoItemId == item.Id).OrderBy(a => a.NomeArquivo).ToList();
                    
                    foreach (var foto in fotos)
                    {
                        while (threadCount >= vCPU)
                        {
                            Thread.Sleep(1000);
                        }

                        string fileImage = config.PathFotos + item.PedidoId.ToString() + @"/" + item.Id.ToString() + @"/" + foto.Path;

                        if (foto.Panoramica)
                        {
                            string[] strct = foto.NomeArquivo.Split('.');
                            string ext = strct[^1];
                            strct[^1] = null;

                            string filenameA = item.PedidoId.ToString() + "_" + item.Id.ToString() + "_" + Guid.NewGuid().ToString() + ".jpg";
                            string filenameB = item.PedidoId.ToString() + "_" + item.Id.ToString() + "_" + Guid.NewGuid().ToString() + ".jpg";
                            string nomeArquivoA = string.Join('.', strct) + "a." + ext;
                            string nomeArquivoB = string.Join('.', strct) + "b." + ext;

                            PedidoItemArquivo arqA = new()
                            {
                                PedidoItemId = item.Id,
                                Sequencia = 0,
                                Album = foto.Album,
                                Path = filenameA,
                                NomeArquivo = nomeArquivoA,
                                DataUpload = DateTime.Now
                            };

                            PedidoItemArquivo arqB = new()
                            {
                                PedidoItemId = item.Id,
                                Sequencia = 0,
                                Album = foto.Album,
                                Path = filenameB,
                                NomeArquivo = nomeArquivoB,
                                DataUpload = DateTime.Now
                            };

                            arquivos.Add(arqA);
                            arquivos.Add(arqB);

                            // Thread
                            Thread t = new(() => ThreadProcessaImagemPanoramicas(item, foto, fileImage, arqA, arqB));
                            t.Start();

                            threadCount++;

                        }
                        else
                        {
                            string filename = item.PedidoId.ToString() + "_" + item.Id.ToString() + "_" + Guid.NewGuid().ToString() + ".jpg";
                            PedidoItemArquivo arq = new()
                            {
                                PedidoItemId = item.Id,
                                Sequencia = 0,
                                Album = foto.Album,
                                Path = filename,
                                NomeArquivo = foto.NomeArquivo,
                                DataUpload = DateTime.Now
                            };
                            arquivos.Add(arq);

                            // Thread
                            Thread t = new(() => ThreadProcessaImagemSimples(item, foto, fileImage, arq));
                            t.Start();

                            threadCount++;
                        }
                    }

                    int contador = 0;
                    while (threadCount > 0)
                    {
                        Thread.Sleep(1000);
                        contador++;
                        if(contador >= 60)
                        {
                            threadCount = 0;
                        }
                    }


                    context.PedidoItemArquivo.AddRange(arquivos);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                context.PedidoItemUpload.RemoveRange(processadas);
                context.SaveChanges();

                context.Database.CloseConnection();
                context.Dispose();
            }
        }

        private static void LimparStorage(int id)
        {
            DataContext context = new();

            try
            {
                PedidoItem item = context.PedidoItem.Find(id);
                if (item != null)
                {
                    // excluir arquivos que não estão cadastrados no banco de dados
                    string path = config.PathFotos + @"/" + item.PedidoId.ToString() + @"/" + item.Id.ToString() + @"/";
                    List<PedidoItemArquivo> arquivosLista = context.PedidoItemArquivo.Where(a => a.PedidoItemId == item.Id).ToList();
                    string[] filePaths = Directory.GetFiles(path);
                    foreach (var file in filePaths)
                    {
                        bool achou = false;
                        foreach (var arq in arquivosLista)
                        {
                            if (file == path + arq.Path)
                            {
                                achou = true;
                            }
                        }

                        if (!achou)
                        {
                            File.Delete(file);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        public static void Importar()
        {
            DataContext context = new();

            try
            {
                int cores = 0;
                foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
                {
                    cores += int.Parse(item["NumberOfCores"].ToString());
                }

                string temp = AppDomain.CurrentDomain.BaseDirectory;

                vCPU = cores * 10;

                config = context.Config.Find(1);

                var itens = context.PedidoItem.Where(p => p.DataUpload != null && p.DataProcessamentoUpload == null).ToList();

                foreach (var item in itens)
                {
                    if (item.Index && item.DataProcessamentoIndex == null)
                    {
                        Index(item);

                        item.DataProcessamentoIndex = DateTime.Now;
                        context.PedidoItem.Update(item);
                        context.Entry(item).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    if (item.DataProcessamentoPreparacao == null)
                    {
                        PreparaImagensUpload(item.Id);

                        item.DataProcessamentoPreparacao = DateTime.Now;
                        context.PedidoItem.Update(item);
                        context.Entry(item).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    if (item.DataProcessamentoUpload == null)
                    {
                        ProcessaImagensUpload(item.Id);

                        item.DataProcessamentoUpload = DateTime.Now;
                        context.PedidoItem.Update(item);
                        context.Entry(item).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    if (item.DataAprovacao == null && config.AprovarAutomaticamente)
                    {
                        item.DataAprovacao = DateTime.Now;
                        context.PedidoItem.Update(item);
                        context.Entry(item).State = EntityState.Modified;
                        context.SaveChanges();
                    }


                    LimparStorage(item.Id);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                context.Dispose();
            }
        }

    }
}
