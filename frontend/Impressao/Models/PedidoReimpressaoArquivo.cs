namespace Impressao.Models
{
    public class PedidoReimpressaoArquivo
    {
        public int Id { get; set; }
        public int PedidoReimpressaoId { get; set;}
        public bool ApontamentoFrente { get; set; }
        public int ArquivoFrenteId { get; set;}
        public int? ArquivoVersoId { get; set; }

        public PedidoReimpressao PedidoReimpressao { get; set; }
        public PedidoItemArquivo ArquivoFrente { get; set; }
        public PedidoItemArquivo? ArquivoVerso { get; set; }
    }
}
