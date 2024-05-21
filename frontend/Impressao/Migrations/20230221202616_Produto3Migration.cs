using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Impressao.Migrations
{
    /// <inheritdoc />
    public partial class Produto3Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Orientacao",
                table: "Produto",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Orientacao",
                table: "Produto");
        }
    }
}
