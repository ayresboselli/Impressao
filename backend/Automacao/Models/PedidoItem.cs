using System.ComponentModel.DataAnnotations;

namespace Automacao.Models
{
    public class PedidoItem
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public int PedidoId { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public int ProdutoId { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public int Copias { get; set; }
        public bool Index { get; set; }
        public DateTime? DataUpload { get; set; }
        public DateTime? DataProcessamentoIndex { get; set; }
        public DateTime? DataProcessamentoPreparacao { get; set; }
        public DateTime? DataProcessamentoUpload { get; set; }
        public DateTime? DataAprovacao { get; set; }
        public DateTime? DataImposicao { get; set; }

        //public Pedido Pedido { get; set; }
        public Produto Produto { get; set; }
        public ICollection<PedidoItemArquivo> PedidoItemArquivos { get; set; }
        //public ICollection<PedidoItemApontamento> PedidoItemApontamentos { get; set; }
        public ICollection<PedidoItemArquivoAlbum> PedidoItemArquivoAlbuns { get; set; }
        //public ICollection<PedidoReimpressao> PedidoReimpressoes { get; set; }
        public ICollection<PedidoItemUpload> PedidoItemUploads { get; set; }
    }
}
