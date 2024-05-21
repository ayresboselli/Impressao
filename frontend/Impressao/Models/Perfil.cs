namespace Impressao.Models
{
    public class Perfil
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Titulo { get; set; }

        public ICollection<UsuarioPerfil> UsuarioPerfis { get; set; }
        public ICollection<PerfilFuncao> PerfilFuncoes { get; set; }
    }
}
