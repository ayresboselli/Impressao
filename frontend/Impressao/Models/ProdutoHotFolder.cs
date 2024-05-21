namespace Impressao.Models
{
    public class ProdutoHotFolder
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public ICollection<Produto> Produtos { get; set; }
    }
}
