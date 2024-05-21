using Microsoft.EntityFrameworkCore;

namespace Automacao.Models
{
    public class DataContext : DbContext
    {
        //public DbSet<Celula> Celula { get; set; }
        //public DbSet<Empresa> Empresa { get; set; }
        public DbSet<Config> Config { get; set; }

        //public DbSet<ClienteGrupo> ClienteGrupo { get; set; }
        //public DbSet<Cidade> Cidade { get; set; }
        //public DbSet<Cliente> Cliente { get; set; }

        public DbSet<Produto> Produto { get; set; }
        public DbSet<ProdutoInformacao> ProdutoInformacao { get; set; }
        //public DbSet<ProdutoGrupo> ProdutoGrupo { get; set; }
        //public DbSet<ProdutoHotFolder> ProdutoHotFolder { get; set; }
        //public DbSet<ProdutoSubstrato> ProdutoSubstrato { get; set; }

        //public DbSet<Pedido> Pedido { get; set; }
        public DbSet<PedidoItem> PedidoItem { get; set; }
        public DbSet<PedidoItemArquivo> PedidoItemArquivo { get; set; }
        public DbSet<PedidoItemArquivoAlbum> PedidoItemArquivoAlbum { get; set; }
        //public DbSet<PedidoItemApontamento> PedidoItemApontamento { get; set; }
        public DbSet<PedidoItemUpload> PedidoItemUpload { get; set; }

        //public DbSet<PedidoReimpressao> PedidoReimpressao { get; set; }
        //public DbSet<PedidoReimpressaoArquivo> PedidoReimpressaoArquivo { get; set; }
        /*
        public DbSet<TabelaPreco> TabelaPreco { get; set; }
        public DbSet<TabelaPrecoCliente> TabelaPrecoCliente { get; set; }
        public DbSet<TabelaPrecoProduto> TabelaPrecoProduto { get; set; }
        */

        //public DbSet<Usuario> Usuario { get; set; }


        public DataContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to postgres with connection string from app settings
            options.UseNpgsql("Host=localhost; Database=impressao; Username=postgres; Password=1234");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //// Cliente
            //modelBuilder.Entity<Cliente>()
            //    .HasOne<ClienteGrupo>(s => s.ClienteGrupo)
            //    .WithMany(g => g.Clientes)
            //    .HasForeignKey(s => s.ClienteGrupoId);

            //modelBuilder.Entity<Cliente>()
            //    .HasOne<Cidade>(s => s.Cidade)
            //    .WithMany(g => g.Clientes)
            //    .HasForeignKey(s => s.CidadeId);

            //// Produto
            //modelBuilder.Entity<Produto>()
            //    .HasOne<ProdutoGrupo>(s => s.ProdutoGrupo)
            //    .WithMany(g => g.Produtos)
            //    .HasForeignKey(s => s.ProdutoGrupoId);

            //modelBuilder.Entity<Produto>()
            //    .HasOne<ProdutoSubstrato>(s => s.ProdutoSubstrato)
            //    .WithMany(g => g.Produtos)
            //    .HasForeignKey(s => s.ProdutoSubstratoId);

            //modelBuilder.Entity<Produto>()
            //    .HasOne<ProdutoHotFolder>(s => s.ProdutoHotFolder)
            //    .WithMany(g => g.Produtos)
            //    .HasForeignKey(s => s.ProdutoHotFolderId);

            modelBuilder.Entity<ProdutoInformacao>()
                .HasOne<Produto>(s => s.Produto)
                .WithMany(g => g.Informacoes)
                .HasForeignKey(s => s.ProdutoId);


            // Pedido
            //modelBuilder.Entity<Pedido>()
            //    .HasOne<Cliente>(s => s.Cliente)
            //    .WithMany(g => g.Pedidos)
            //    .HasForeignKey(s => s.ClienteId);

            //modelBuilder.Entity<PedidoItem>()
            //    .HasOne<Pedido>(s => s.Pedido)
            //    .WithMany(g => g.PedidoItens)
            //    .HasForeignKey(s => s.PedidoId);

            modelBuilder.Entity<PedidoItem>()
                .HasOne<Produto>(s => s.Produto)
                .WithMany(g => g.PedidoItens)
                .HasForeignKey(s => s.ProdutoId);

            modelBuilder.Entity<PedidoItemArquivo>()
                .HasOne<PedidoItem>(s => s.PedidoItem)
                .WithMany(g => g.PedidoItemArquivos)
                .HasForeignKey(s => s.PedidoItemId);

            modelBuilder.Entity<PedidoItemArquivo>()
                .HasOne<PedidoItemArquivoAlbum>(s => s.PedidoItemArquivoAlbum)
                .WithMany(g => g.PedidoItemArquivos)
                .HasForeignKey(s => s.PedidoItemArquivoAlbumId)
                .IsRequired(false);

            modelBuilder.Entity<PedidoItemArquivoAlbum>()
                .HasOne<PedidoItem>(s => s.PedidoItem)
                .WithMany(g => g.PedidoItemArquivoAlbuns)
                .HasForeignKey(s => s.PedidoItemId);

            //modelBuilder.Entity<PedidoItemApontamento>()
            //    .HasOne<PedidoItem>(s => s.PedidoItem)
            //    .WithMany(g => g.PedidoItemApontamentos)
            //    .HasForeignKey(s => s.PedidoItemId);

            //modelBuilder.Entity<PedidoItemApontamento>()
            //    .HasOne<Celula>(s => s.Celula)
            //    .WithMany(g => g.PedidoItemApontamentos)
            //    .HasForeignKey(s => s.CelulaId);

            modelBuilder.Entity<PedidoItemUpload>()
                .HasOne<PedidoItem>(s => s.PedidoItem)
                .WithMany(g => g.PedidoItemUploads)
                .HasForeignKey(s => s.PedidoItemId);

            //// Pedido Reimpressão
            //modelBuilder.Entity<PedidoReimpressao>()
            //    .HasOne<PedidoItem>(s => s.PedidoItem)
            //    .WithMany(g => g.PedidoReimpressoes)
            //    .HasForeignKey(s => s.PedidoItemId);

            //modelBuilder.Entity<PedidoReimpressaoArquivo>()
            //    .HasOne<PedidoReimpressao>(s => s.PedidoReimpressao)
            //    .WithMany(g => g.Arquivos)
            //    .HasForeignKey(s => s.PedidoReimpressaoId);

            //modelBuilder.Entity<PedidoReimpressaoArquivo>()
            //    .HasOne<PedidoItemArquivo>(s => s.ArquivoFrente)
            //    .WithMany(g => g.PedidoReimpressaoArquivosFrente)
            //    .HasForeignKey(s => s.ArquivoFrenteId);

            //modelBuilder.Entity<PedidoReimpressaoArquivo>()
            //    .HasOne<PedidoItemArquivo>(s => s.ArquivoVerso)
            //    .WithMany(g => g.PedidoReimpressaoArquivosVerso)
            //    .HasForeignKey(s => s.ArquivoVersoId);
        }
    }
}
