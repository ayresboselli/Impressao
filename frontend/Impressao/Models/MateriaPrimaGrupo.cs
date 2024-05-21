namespace Impressao.Models
{
    public class MateriaPrimaGrupo
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string? Descricao { get; set; }

        public ICollection<MateriaPrima> MateriasPrima { get; set; }
    }
}
