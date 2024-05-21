using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Impressao.Migrations
{
    /// <inheritdoc />
    public partial class PedidoItemMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdProduto",
                table: "PedidoItem",
                newName: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItem_ProdutoId",
                table: "PedidoItem",
                column: "ProdutoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoItem_Produto_ProdutoId",
                table: "PedidoItem",
                column: "ProdutoId",
                principalTable: "Produto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoItem_Produto_ProdutoId",
                table: "PedidoItem");

            migrationBuilder.DropIndex(
                name: "IX_PedidoItem_ProdutoId",
                table: "PedidoItem");

            migrationBuilder.RenameColumn(
                name: "ProdutoId",
                table: "PedidoItem",
                newName: "IdProduto");
        }
    }
}
