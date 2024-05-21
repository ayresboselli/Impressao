using System.ComponentModel.DataAnnotations;

namespace Impressao.Models
{
    public class TabelaPreco
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public bool Padrao { get; set; }
        public bool Ativo { get; set; }

        public ICollection<Pedido>? Pedidos { get; set; }
        public ICollection<TabelaPrecoCliente>? TabelaPrecoClientes { get; set; }
        public ICollection<TabelaPrecoProduto>? TabelaPrecoProdutos { get; set; }
    }
}
