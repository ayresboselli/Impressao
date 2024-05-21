using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Impressao.Migrations
{
    /// <inheritdoc />
    public partial class ProdutoMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produto_ProdutoHotFolder_ProdutoHotFolderId",
                table: "Produto");

            migrationBuilder.DropForeignKey(
                name: "FK_Produto_ProdutoSubstrato_ProdutoSubstratoId",
                table: "Produto");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produto_ProdutoHotFolder_ProdutoHotFolderId",
                table: "Produto");

            migrationBuilder.DropForeignKey(
                name: "FK_Produto_ProdutoSubstrato_ProdutoSubstratoId",
                table: "Produto");

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
    }
}
