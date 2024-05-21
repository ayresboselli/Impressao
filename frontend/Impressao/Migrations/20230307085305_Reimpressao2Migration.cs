using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Impressao.Migrations
{
    /// <inheritdoc />
    public partial class Reimpressao2Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoReimpressaoArquivo_PedidoItemArquivo_ArquivoVersoId",
                table: "PedidoReimpressaoArquivo");

            migrationBuilder.AlterColumn<int>(
                name: "ArquivoVersoId",
                table: "PedidoReimpressaoArquivo",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "ApontamentoFrente",
                table: "PedidoReimpressaoArquivo",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoReimpressaoArquivo_PedidoItemArquivo_ArquivoVersoId",
                table: "PedidoReimpressaoArquivo",
                column: "ArquivoVersoId",
                principalTable: "PedidoItemArquivo",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoReimpressaoArquivo_PedidoItemArquivo_ArquivoVersoId",
                table: "PedidoReimpressaoArquivo");

            migrationBuilder.DropColumn(
                name: "ApontamentoFrente",
                table: "PedidoReimpressaoArquivo");

            migrationBuilder.AlterColumn<int>(
                name: "ArquivoVersoId",
                table: "PedidoReimpressaoArquivo",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoReimpressaoArquivo_PedidoItemArquivo_ArquivoVersoId",
                table: "PedidoReimpressaoArquivo",
                column: "ArquivoVersoId",
                principalTable: "PedidoItemArquivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
