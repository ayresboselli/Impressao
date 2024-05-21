using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Impressao.Models
{
    public class TabelaPrecoCliente
    {
        public int Id { get; set; }
        public int TabelaId { get; set; }
        public int ClienteId { get; set; }
        public bool Padrao { get; set; }

        public TabelaPreco TabelaPreco { get; set; }
        public Cliente Cliente { get; set; }
    }
}
