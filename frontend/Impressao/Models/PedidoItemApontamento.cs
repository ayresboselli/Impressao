namespace Impressao.Models
{
    public class PedidoItemApontamento
    {
        public int Id { get; set; }
        public int PedidoItemId { get; set; }
        public int CelulaId { get; set; }
        public int Sequencia { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataTermino { get; set; }

        public PedidoItem PedidoItem { get; set; }
        public Celula Celula { get; set; }
    }
}
