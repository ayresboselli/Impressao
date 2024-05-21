namespace Impressao.Models
{
    public class ProdutoInformacao
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int Pagina { get; set; }
        public string Tipo { get; set; }
        public string Texto { get; set; }
        public int PosY { get; set; }
        public int PosX { get; set; }
        public int Orientacao { get; set; }

        public Produto Produto { get; set; }
    }
}
