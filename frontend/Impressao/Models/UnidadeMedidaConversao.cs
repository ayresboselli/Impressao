namespace Impressao.Models
{
    public class UnidadeMedidaConversao
    {
        public int Id { get; set; }
        public int IdUnidadeOriginal { get; set; }
        public int IdUnidadeConvertida { get; set; }
        public double Proporcao { get; set; }

        public UnidadeMedida? Original { get; set; }
        public UnidadeMedida? Convertida { get; set; }
    }
}
