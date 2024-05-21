using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Impressao.Migrations
{
    /// <inheritdoc />
    public partial class ImpressaoMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Celula",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Celula", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClienteGrupo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClienteGrupo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCliente = table.Column<int>(type: "integer", nullable: false),
                    Contrato = table.Column<string>(type: "text", nullable: true),
                    DataEntrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PrevisaoEntrega = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataFinalizado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProdutoGrupo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoGrupo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProdutoHotFolder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoHotFolder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProdutoSubstrato",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoSubstrato", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Login = table.Column<string>(type: "text", nullable: false),
                    Senha = table.Column<string>(type: "text", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClienteGrupoId = table.Column<int>(type: "integer", nullable: false),
                    FisicaJuridica = table.Column<char>(type: "character(1)", nullable: false),
                    RazaoSocial = table.Column<string>(type: "text", nullable: false),
                    NomeFantasia = table.Column<string>(type: "text", nullable: true),
                    CpfCnpj = table.Column<string>(type: "text", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "text", nullable: false),
                    Telefone2 = table.Column<string>(type: "text", nullable: true),
                    Logradouro = table.Column<string>(type: "text", nullable: false),
                    Numero = table.Column<int>(type: "integer", nullable: true),
                    Complemento = table.Column<string>(type: "text", nullable: false),
                    Bairro = table.Column<string>(type: "text", nullable: false),
                    Cep = table.Column<string>(type: "text", nullable: false),
                    CidadeIbge = table.Column<int>(type: "integer", nullable: false),
                    Pais = table.Column<int>(type: "integer", nullable: false),
                    Observacoes = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cliente_ClienteGrupo_ClienteGrupoId",
                        column: x => x.ClienteGrupoId,
                        principalTable: "ClienteGrupo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PedidoId = table.Column<int>(type: "integer", nullable: false),
                    IdProduto = table.Column<int>(type: "integer", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    DataAprovacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataImposicao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoItem_Pedido_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    ProdutoGrupoId = table.Column<int>(type: "integer", nullable: true),
                    ProdutoSubstratoId = table.Column<int>(type: "integer", nullable: true),
                    ProdutoHotFolderId = table.Column<int>(type: "integer", nullable: true),
                    RenomearArquivo = table.Column<bool>(type: "boolean", nullable: false),
                    Largura = table.Column<int>(type: "integer", nullable: false),
                    Altura = table.Column<int>(type: "integer", nullable: false),
                    LarguraMidia = table.Column<int>(type: "integer", nullable: false),
                    AlturaMidia = table.Column<int>(type: "integer", nullable: false),
                    DisposicaoImpressao = table.Column<char>(type: "character(1)", nullable: false),
                    DisposicaoImagem = table.Column<char>(type: "character(1)", nullable: false),
                    DeslocamentoFrenteX = table.Column<int>(type: "integer", nullable: true),
                    DeslocamentoFrenteY = table.Column<int>(type: "integer", nullable: true),
                    DeslocamentoVersoX = table.Column<int>(type: "integer", nullable: true),
                    DeslocamentoVersoY = table.Column<int>(type: "integer", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produto_ProdutoGrupo_ProdutoGrupoId",
                        column: x => x.ProdutoGrupoId,
                        principalTable: "ProdutoGrupo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Produto_ProdutoHotFolder_ProdutoHotFolderId",
                        column: x => x.ProdutoHotFolderId,
                        principalTable: "ProdutoHotFolder",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Produto_ProdutoSubstrato_ProdutoSubstratoId",
                        column: x => x.ProdutoSubstratoId,
                        principalTable: "ProdutoSubstrato",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TabelaPrecoCliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TabelaId = table.Column<int>(type: "integer", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    Padrao = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TabelaPrecoCliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TabelaPrecoCliente_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItemApontamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PedidoItemId = table.Column<int>(type: "integer", nullable: false),
                    CelulaId = table.Column<int>(type: "integer", nullable: false),
                    Sequencia = table.Column<int>(type: "integer", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataTermino = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItemApontamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoItemApontamento_Celula_CelulaId",
                        column: x => x.CelulaId,
                        principalTable: "Celula",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoItemApontamento_PedidoItem_PedidoItemId",
                        column: x => x.PedidoItemId,
                        principalTable: "PedidoItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItemArquivo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PedidoItemId = table.Column<int>(type: "integer", nullable: false),
                    Sequencia = table.Column<float>(type: "real", nullable: false),
                    Album = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    NomeArquivo = table.Column<string>(type: "text", nullable: false),
                    DataUpload = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItemArquivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoItemArquivo_PedidoItem_PedidoItemId",
                        column: x => x.PedidoItemId,
                        principalTable: "PedidoItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TabelaPrecoProduto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TabelaId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    Valor = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TabelaPrecoProduto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TabelaPrecoProduto_Produto_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TabelaPreco",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Padrao = table.Column<bool>(type: "boolean", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    TabelaPrecoClienteId = table.Column<int>(type: "integer", nullable: true),
                    TabelaPrecoProdutoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TabelaPreco", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TabelaPreco_TabelaPrecoCliente_TabelaPrecoClienteId",
                        column: x => x.TabelaPrecoClienteId,
                        principalTable: "TabelaPrecoCliente",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TabelaPreco_TabelaPrecoProduto_TabelaPrecoProdutoId",
                        column: x => x.TabelaPrecoProdutoId,
                        principalTable: "TabelaPrecoProduto",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_ClienteGrupoId",
                table: "Cliente",
                column: "ClienteGrupoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItem_PedidoId",
                table: "PedidoItem",
                column: "PedidoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItemApontamento_CelulaId",
                table: "PedidoItemApontamento",
                column: "CelulaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItemApontamento_PedidoItemId",
                table: "PedidoItemApontamento",
                column: "PedidoItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItemArquivo_PedidoItemId",
                table: "PedidoItemArquivo",
                column: "PedidoItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produto_ProdutoGrupoId",
                table: "Produto",
                column: "ProdutoGrupoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produto_ProdutoHotFolderId",
                table: "Produto",
                column: "ProdutoHotFolderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produto_ProdutoSubstratoId",
                table: "Produto",
                column: "ProdutoSubstratoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TabelaPreco_TabelaPrecoClienteId",
                table: "TabelaPreco",
                column: "TabelaPrecoClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_TabelaPreco_TabelaPrecoProdutoId",
                table: "TabelaPreco",
                column: "TabelaPrecoProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_TabelaPrecoCliente_ClienteId",
                table: "TabelaPrecoCliente",
                column: "ClienteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TabelaPrecoProduto_ProdutoId",
                table: "TabelaPrecoProduto",
                column: "ProdutoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PedidoItemApontamento");

            migrationBuilder.DropTable(
                name: "PedidoItemArquivo");

            migrationBuilder.DropTable(
                name: "TabelaPreco");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Celula");

            migrationBuilder.DropTable(
                name: "PedidoItem");

            migrationBuilder.DropTable(
                name: "TabelaPrecoCliente");

            migrationBuilder.DropTable(
                name: "TabelaPrecoProduto");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Produto");

            migrationBuilder.DropTable(
                name: "ClienteGrupo");

            migrationBuilder.DropTable(
                name: "ProdutoGrupo");

            migrationBuilder.DropTable(
                name: "ProdutoHotFolder");

            migrationBuilder.DropTable(
                name: "ProdutoSubstrato");
        }
    }
}
