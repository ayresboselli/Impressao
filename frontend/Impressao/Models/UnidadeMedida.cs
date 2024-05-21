namespace Impressao.Models
{
    public class UnidadeMedida
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Sigla { get; set; }

        public ICollection<ProdutoEngenharia> ProdutosEngenharia { get; set; }
        public ICollection<MateriaPrima> MateriasPrima { get; set; }
        public ICollection<UnidadeMedidaConversao> UnidadesOriginais { get; set; }
        public ICollection<UnidadeMedidaConversao> UnidadesConvertidas { get; set; }
    }
}
