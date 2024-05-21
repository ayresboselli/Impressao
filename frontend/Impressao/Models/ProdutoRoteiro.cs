namespace Impressao.Models
{
    public class ProdutoRoteiro
    {
        public int Id { get; set; }
        public int IdProduto { get; set; }
        public int IdCelula { get; set; }
        public int Sequencia { get; set; }

        public Produto? Produto { get; set; }
        public Celula? Celula { get; set; }

        public ICollection<ProdutoEngenharia> ProdutosEngenharia { get; set; }
    }
}
