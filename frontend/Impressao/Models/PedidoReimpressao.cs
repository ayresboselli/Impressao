namespace Impressao.Models
{
    public class PedidoReimpressao
    {
        public int Id { get; set; }
        public int PedidoItemId { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataImposicionamento { get; set; }

        public PedidoItem PedidoItem { get; set; }
        public ICollection<PedidoReimpressaoArquivo> Arquivos { get; set; }
    }
}
