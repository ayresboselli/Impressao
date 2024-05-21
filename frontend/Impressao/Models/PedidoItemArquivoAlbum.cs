namespace Impressao.Models
{
    public class PedidoItemArquivoAlbum
    {
        public int Id { get; set; }
        public int PedidoItemId { get; set; }
        public string Album { get; set; }
        public string NomeArquivo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataProcessamento { get; set; }

        public PedidoItem PedidoItem { get; set; }
        public ICollection<PedidoItemArquivo> PedidoItemArquivos { get; set; }
    }
}
