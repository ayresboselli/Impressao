using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Impressao.Migrations
{
    /// <inheritdoc />
    public partial class Pedido2Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "Pedido",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "Pedido");
        }
    }
}
