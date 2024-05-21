namespace Automacao.Models
{
    public class PedidoItemUpload
    {
        public int Id { get; set; }
        public int PedidoItemId { get; set; }
        public string Path { get; set; }
        public string NomeArquivo { get; set; }
        public string Album { get; set; }
        public int Largura { get; set; }
        public int Altura { get; set; }
        public bool Rotacionar { get; set; }
        public bool Panoramica { get; set; }
        public DateTime DataCadstro { get; set; }

        public PedidoItem PedidoItem { get; set; }
    }
}
