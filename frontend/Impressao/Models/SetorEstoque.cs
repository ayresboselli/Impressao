namespace Impressao.Models
{
    public class SetorEstoque
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        public bool Ativo { get; set; }

        public ICollection<MateriaPrima> MateriasPrima { get; set; }
    }
}
