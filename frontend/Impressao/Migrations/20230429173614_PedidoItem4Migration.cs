using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Impressao.Migrations
{
    /// <inheritdoc />
    public partial class PedidoItem4Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataFechamento",
                table: "PedidoItem",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataFechamento",
                table: "PedidoItem");
        }
    }
}
