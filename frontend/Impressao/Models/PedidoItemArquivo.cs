namespace Impressao.Models
{
    public class PedidoItemArquivo
    {
        public int Id { get; set; }
        public int PedidoItemId { get; set; }
        public int? PedidoItemArquivoAlbumId { get; set; }
        public float Sequencia { get; set; }
        public string Album { get; set; }
        public string Path { get; set; }
        public string NomeArquivo { get; set; }
        public DateTime DataUpload { get; set; }

        public PedidoItem PedidoItem { get; set; }
        public PedidoItemArquivoAlbum PedidoItemArquivoAlbum { get; set; }
        public ICollection<PedidoReimpressaoArquivo> PedidoReimpressaoArquivosFrente { get; set; }
        public ICollection<PedidoReimpressaoArquivo> PedidoReimpressaoArquivosVerso { get; set; }
    }
}
