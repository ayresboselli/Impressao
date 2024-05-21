using Impressao.Models;

namespace Impressao.ViewModel
{
    public class UnidadeMedidaConversaoViewModel
    {
        public UnidadeMedidaConversao Conversao { get; set; }
        public IEnumerable<UnidadeMedida> UnidadesMedida { get; set; }
    }
}
