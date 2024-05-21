using System.ComponentModel.DataAnnotations;

namespace Impressao.Models
{
    public class ClienteGrupo
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        public ICollection<Cliente> Clientes { get; set; }
    }
}
