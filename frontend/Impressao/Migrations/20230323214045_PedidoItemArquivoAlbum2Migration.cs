using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Impressao.Migrations
{
    /// <inheritdoc />
    public partial class PedidoItemArquivoAlbum2Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PIAAlbum_PedidoItem_PedidoItemId",
                table: "PedidoItemArquivo");

            migrationBuilder.AlterColumn<int>(
                name: "PedidoItemArquivoAlbumId",
                table: "PedidoItemArquivo",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_PIAAlbum_PedidoItem_PedidoItemId",
                table: "PedidoItemArquivo",
                column: "PedidoItemArquivoAlbumId",
                principalTable: "PedidoItemArquivoAlbum",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PIAAlbum_PedidoItem_PedidoItemId",
                table: "PedidoItemArquivo");

            migrationBuilder.AlterColumn<int>(
                name: "PedidoItemArquivoAlbumId",
                table: "PedidoItemArquivo",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PIAAlbum_PedidoItem_PedidoItemId",
                table: "PedidoItemArquivo",
                column: "PedidoItemArquivoAlbumId",
                principalTable: "PedidoItemArquivoAlbum",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
