using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Impressao.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string Login { get; set; }
        public string? Senha { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }

        public ICollection<UsuarioPerfil> UsuarioPerfis { get; set; }
    }
}
