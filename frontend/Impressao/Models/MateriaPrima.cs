using Impressao.Models;

namespace Impressao.Models
{
    public class MateriaPrima
    {
        public int Id { get; set; }
        public int IdGrupo { get; set; }
        public int IdSetorEstoque { get; set; }
        public int IdUnidadeMedida { get; set; }
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        public double UltimoValor { get; set; }
        public double ValorMedio { get; set; }
        public double Quantidade { get; set; }
        public bool Ativo { get; set; }

        public MateriaPrimaGrupo? Grupo { get; set; }
        public UnidadeMedida? UnidadeMedida { get; set; }
        public SetorEstoque? SetorEstoque { get; set; }

        public ICollection<ProdutoEngenharia> ProdutosEngenharia { get; set; }
    }
}
