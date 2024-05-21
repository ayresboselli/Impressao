using System.ComponentModel.DataAnnotations;

namespace Impressao.Models
{
    public class Produto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public string Titulo { get; set; }
        public int? ProdutoGrupoId { get; set; }
        public int? ProdutoSubstratoId { get; set; }
        public int? ProdutoHotFolderId { get; set; }
        public bool ArquivosJPEG { get; set; }
        public bool ArquivosPDF { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public int QuantidadeArquivosProduto { get; set; }
        public bool RenomearArquivo { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public int Largura { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public int Altura { get; set; }
        public int Orientacao { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public int LarguraMidia { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório.")]
        public int AlturaMidia { get; set; }
        public bool Giro180 { get; set; }
        public char DisposicaoImpressao { get; set; }
        public char DisposicaoImagem { get; set; }
        public int? DeslocamentoFrenteX { get; set; }
        public int? DeslocamentoFrenteY { get; set; }
        public int? DeslocamentoVersoX { get; set; }
        public int? DeslocamentoVersoY { get; set; }
        public bool Ativo { get; set; }

        public ProdutoGrupo ProdutoGrupo { get; set; }
        public ProdutoSubstrato? ProdutoSubstrato { get; set; }
        public ProdutoHotFolder? ProdutoHotFolder { get; set; }
        public ICollection<PedidoItem> PedidoItens { get; set; }
        public ICollection<ProdutoInformacao> Informacoes { get; set; }
        public ICollection<TabelaPrecoProduto>? TabelaPrecoProdutos { get; set; }
        public ICollection<ProdutoRoteiro> ProdutosRoteiro { get; set; }
    }
}
