namespace Impressao.Models
{
    public class ProdutoEngenharia
    {
        public int Id { get; set; }
        public int IdRoteiro { get; set; }
        public int IdMateriaPrima { get; set; }
        public int IdUnidadeMedida { get; set; }
        public decimal Quantidade { get; set; }

        public ProdutoRoteiro? ProdutoRoteiro { get; set; }
        public MateriaPrima? MateriaPrima { get; set; }
        public UnidadeMedida? UnidadeMedida { get; set; }
    }
}
