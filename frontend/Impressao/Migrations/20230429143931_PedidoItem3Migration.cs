using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Impressao.Migrations
{
    /// <inheritdoc />
    public partial class PedidoItem3Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataProcessamento",
                table: "PedidoItem",
                newName: "DataProcessamentoUpload");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataProcessamentoIndex",
                table: "PedidoItem",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataProcessamentoPreparacao",
                table: "PedidoItem",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataProcessamentoIndex",
                table: "PedidoItem");

            migrationBuilder.DropColumn(
                name: "DataProcessamentoPreparacao",
                table: "PedidoItem");

            migrationBuilder.RenameColumn(
                name: "DataProcessamentoUpload",
                table: "PedidoItem",
                newName: "DataProcessamento");
        }
    }
}
