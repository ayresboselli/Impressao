using System.ComponentModel.DataAnnotations;

namespace Impressao.Models
{
    public class TabelaPrecoProduto
    {
        public int Id { get; set; }
        public int TabelaId { get; set; }
        public int ProdutoId { get; set; }
        public double Valor { get; set; }

        public TabelaPreco TabelaPreco { get; set; }
        public Produto Produto { get; set; }
    }
}
