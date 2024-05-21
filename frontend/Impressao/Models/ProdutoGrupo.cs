using System.ComponentModel.DataAnnotations;

namespace Impressao.Models
{
    public class ProdutoGrupo
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        public ICollection<Produto> Produtos { get; set; }
    }
}
