using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Impressao.Migrations
{
    /// <inheritdoc />
    public partial class ConfigMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DPI = table.Column<double>(type: "double precision", nullable: false),
                    UnidadeMedida = table.Column<string>(type: "text", nullable: false),
                    PathFotos = table.Column<string>(type: "text", nullable: false),
                    PathPdf = table.Column<string>(type: "text", nullable: false),
                    MargemMidia = table.Column<int>(type: "integer", nullable: false),
                    MargemThumb = table.Column<int>(type: "integer", nullable: false),
                    LarguraThumb = table.Column<int>(type: "integer", nullable: false),
                    AlturaThumb = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Config");
        }
    }
}
