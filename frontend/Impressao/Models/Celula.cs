using System.ComponentModel.DataAnnotations;

namespace Impressao.Models
{
    public class Celula
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string Titulo { get; set; }
        public string? Descricao { get; set; }

        public ICollection<PedidoItemApontamento> PedidoItemApontamentos { get; set; }
        public ICollection<ProdutoRoteiro> ProdutosRoteiro { get; set; }
    }
}
