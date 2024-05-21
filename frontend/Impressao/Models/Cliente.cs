using System.ComponentModel.DataAnnotations;

namespace Impressao.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public int ClienteGrupoId { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public int CidadeId { get; set; }
        public Char FisicaJuridica { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string RazaoSocial { get; set; }
        public string? NomeFantasia { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string CpfCnpj { get; set; }
        public string? InscricaoEstadual { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public DateTime DataNascimento { get; set; }
        [EmailAddress(ErrorMessage = "Este campo é obrigatório.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string Telefone { get; set; }
        public string? Telefone2 { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string Logradouro { get; set; }
        public int? Numero { get; set; }
        public string? Complemento { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string Bairro { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string Cep { get; set; }
        public int? Pais { get; set; }
        public string? Observacoes { get; set; }
        public bool Ativo { get; set; }

        public ClienteGrupo? ClienteGrupo { get; set; }
        public Cidade? Cidade { get; set; }
        public ICollection<Pedido>? Pedidos { get; set; }
        public ICollection<TabelaPrecoCliente>? TabelaPrecoClientes { get; set; }
    }
}
