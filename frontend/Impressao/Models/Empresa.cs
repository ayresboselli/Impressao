using System.ComponentModel.DataAnnotations;

namespace Impressao.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        public int CidadeId { get; set; }
        public string RazaoSocial { get; set; }
        public string? NomeFantasia { get; set; }
        public string Cnpj { get; set; }
        public string? InscricaoEstadual { get; set; }
        public DateTime DataCriacao { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string? Telefone2 { get; set; }
        public string Logradouro { get; set; }
        public int? Numero { get; set; }
        public string? Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public int? Pais { get; set; }
        public string? Logo { get; set; }
    }
}