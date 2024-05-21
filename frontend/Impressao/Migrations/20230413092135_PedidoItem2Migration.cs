using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Impressao.Migrations
{
    /// <inheritdoc />
    public partial class PedidoItem2Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataProcessamento",
                table: "PedidoItem",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataUpload",
                table: "PedidoItem",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Index",
                table: "PedidoItem",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataProcessamento",
                table: "PedidoItem");

            migrationBuilder.DropColumn(
                name: "DataUpload",
                table: "PedidoItem");

            migrationBuilder.DropColumn(
                name: "Index",
                table: "PedidoItem");
        }
    }
}
