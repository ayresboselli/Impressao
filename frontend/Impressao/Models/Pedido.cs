using System.ComponentModel.DataAnnotations;

namespace Impressao.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public int ClienteId { get; set; }
        public int TabelaId { get; set; }
        public string? Contrato { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime? PrevisaoEntrega { get; set; }
        public DateTime? DataFinalizado { get; set; }
        public string? Observacoes { get; set; }

        public Cliente Cliente { get; set; }
        public TabelaPreco TabelaPreco { get; set; }
        public ICollection<PedidoItem> PedidoItens { get; set; }
    }
}
