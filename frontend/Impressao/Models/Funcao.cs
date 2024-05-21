namespace Impressao.Models
{
    public class Funcao
    {
        public int Id { get; set; }
        public string Chave { get; set; }
        public string Titulo { get; set; }

        public ICollection<PerfilFuncao> PerfilFuncoes { get; set; }
    }
}
