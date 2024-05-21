namespace Impressao.Models
{
    public class Cidade
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string UF { get; set; }
        public string Estado { get; set; }

        public ICollection<Cliente> Clientes { get; set; }
    }
}
