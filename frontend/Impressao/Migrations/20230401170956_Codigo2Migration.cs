using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Impressao.Migrations
{
    /// <inheritdoc />
    public partial class Codigo2Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "ProdutoGrupo");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Produto");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "ClienteGrupo");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Celula");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "ProdutoGrupo",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Produto",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "ClienteGrupo",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Cliente",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Celula",
                type: "text",
                nullable: true);
        }
    }
}
