namespace Impressao.Models
{
    public class PerfilFuncao
    {
        public int Id { get; set; }
        public int PerfilId { get; set; }
        public int FuncaoId { get; set; }

        public Perfil Perfil { get; set; }
        public Funcao Funcao { get; set; }
    }
}
