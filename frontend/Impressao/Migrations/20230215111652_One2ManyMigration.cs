using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Impressao.Migrations
{
    /// <inheritdoc />
    public partial class One2ManyMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produto_ProdutoGrupo_ProdutoGrupoId",
                table: "Produto");

            migrationBuilder.DropForeignKey(
                name: "FK_Produto_ProdutoHotFolder_ProdutoHotFolderId",
                table: "Produto");

            migrationBuilder.DropForeignKey(
                name: "FK_Produto_ProdutoSubstrato_ProdutoSubstratoId",
                table: "Produto");

            migrationBuilder.DropTable(
                name: "TabelaPreco");

            migrationBuilder.DropTable(
                name: "TabelaPrecoCliente");

            migrationBuilder.DropTable(
                name: "TabelaPrecoProduto");

            migrationBuilder.DropIndex(
                name: "IX_Produto_ProdutoGrupoId",
                table: "Produto");

            migrationBuilder.DropIndex(
                name: "IX_Produto_ProdutoHotFolderId",
                table: "Produto");

            migrationBuilder.DropIndex(
                name: "IX_Produto_ProdutoSubstratoId",
                table: "Produto");

            migrationBuilder.DropIndex(
                name: "IX_PedidoItemArquivo_PedidoItemId",
                table: "PedidoItemArquivo");

            migrationBuilder.DropIndex(
                name: "IX_PedidoItemApontamento_CelulaId",
                table: "PedidoItemApontamento");

            migrationBuilder.DropIndex(
                name: "IX_PedidoItemApontamento_PedidoItemId",
                table: "PedidoItemApontamento");

            migrationBuilder.DropIndex(
                name: "IX_PedidoItem_PedidoId",
                table: "PedidoItem");

            migrationBuilder.DropIndex(
                name: "IX_Cliente_ClienteGrupoId",
                table: "Cliente");

            migrationBuilder.AlterColumn<int>(
                name: "ProdutoSubstratoId",
                table: "Produto",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProdutoHotFolderId",
                table: "Produto",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProdutoGrupoId",
                table: "Produto",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produto_ProdutoGrupoId",
                table: "Produto",
                column: "ProdutoGrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_ProdutoHotFolderId",
                table: "Produto",
                column: "ProdutoHotFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_ProdutoSubstratoId",
                table: "Produto",
                column: "ProdutoSubstratoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItemArquivo_PedidoItemId",
                table: "PedidoItemArquivo",
                column: "PedidoItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItemApontamento_CelulaId",
                table: "PedidoItemApontamento",
                column: "CelulaId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItemApontamento_PedidoItemId",
                table: "PedidoItemApontamento",
                column: "PedidoItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItem_PedidoId",
                table: "PedidoItem",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_ClienteGrupoId",
                table: "Cliente",
                column: "ClienteGrupoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produto_ProdutoGrupo_ProdutoGrupoId",
                table: "Produto",
                column: "ProdutoGrupoId",
                principalTable: "ProdutoGrupo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produto_ProdutoHotFolder_ProdutoHotFolderId",
                table: "Produto",
                column: "ProdutoHotFolderId",
                principalTable: "ProdutoHotFolder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produto_ProdutoSubstrato_ProdutoSubstratoId",
                table: "Produto",
                column: "ProdutoSubstratoId",
                principalTable: "ProdutoSubstrato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produto_ProdutoGrupo_ProdutoGrupoId",
                table: "Produto");

            migrationBuilder.DropForeignKey(
                name: "FK_Produto_ProdutoHotFolder_ProdutoHotFolderId",
                table: "Produto");

            migrationBuilder.DropForeignKey(
                name: "FK_Produto_ProdutoSubstrato_ProdutoSubstratoId",
                table: "Produto");

            migrationBuilder.DropIndex(
                name: "IX_Produto_ProdutoGrupoId",
                table: "Produto");

            migrationBuilder.DropIndex(
                name: "IX_Produto_ProdutoHotFolderId",
                table: "Produto");

            migrationBuilder.DropIndex(
                name: "IX_Produto_ProdutoSubstratoId",
                table: "Produto");

            migrationBuilder.DropIndex(
                name: "IX_PedidoItemArquivo_PedidoItemId",
                table: "PedidoItemArquivo");

            migrationBuilder.DropIndex(
                name: "IX_PedidoItemApontamento_CelulaId",
                table: "PedidoItemApontamento");

            migrationBuilder.DropIndex(
                name: "IX_PedidoItemApontamento_PedidoItemId",
                table: "PedidoItemApontamento");

            migrationBuilder.DropIndex(
                name: "IX_PedidoItem_PedidoId",
                table: "PedidoItem");

            migrationBuilder.DropIndex(
                name: "IX_Cliente_ClienteGrupoId",
                table: "Cliente");

            migrationBuilder.AlterColumn<int>(
                name: "ProdutoSubstratoId",
                table: "Produto",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ProdutoHotFolderId",
                table: "Produto",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ProdutoGrupoId",
                table: "Produto",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "TabelaPrecoCliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    Padrao = table.Column<bool>(type: "boolean", nullable: false),
                    TabelaId = table.Column<int>(type: "integer", nullable: false)
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
                name: "TabelaPrecoProduto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    TabelaId = table.Column<int>(type: "integer", nullable: false),
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
                    TabelaPrecoClienteId = table.Column<int>(type: "integer", nullable: true),
                    TabelaPrecoProdutoId = table.Column<int>(type: "integer", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Padrao = table.Column<bool>(type: "boolean", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false)
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
                name: "IX_PedidoItemArquivo_PedidoItemId",
                table: "PedidoItemArquivo",
                column: "PedidoItemId",
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
                name: "IX_PedidoItem_PedidoId",
                table: "PedidoItem",
                column: "PedidoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_ClienteGrupoId",
                table: "Cliente",
                column: "ClienteGrupoId",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Produto_ProdutoGrupo_ProdutoGrupoId",
                table: "Produto",
                column: "ProdutoGrupoId",
                principalTable: "ProdutoGrupo",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Produto_ProdutoHotFolder_ProdutoHotFolderId",
                table: "Produto",
                column: "ProdutoHotFolderId",
                principalTable: "ProdutoHotFolder",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Produto_ProdutoSubstrato_ProdutoSubstratoId",
                table: "Produto",
                column: "ProdutoSubstratoId",
                principalTable: "ProdutoSubstrato",
                principalColumn: "Id");
        }
    }
}
