using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Impressao.Migrations
{
    /// <inheritdoc />
    public partial class PedidoItemArquivoAlbumMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PedidoItemArquivoAlbumId",
                table: "PedidoItemArquivo",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PedidoItemArquivoAlbum",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PedidoItemId = table.Column<int>(type: "integer", nullable: false),
                    Album = table.Column<string>(type: "text", nullable: false),
                    NomeArquivo = table.Column<string>(type: "text", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataProcessamento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItemArquivoAlbum", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PIAAlbum_PedidoItem_PedidoItemId",
                        column: x => x.PedidoItemId,
                        principalTable: "PedidoItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItemArquivo_PedidoItemArquivoAlbumId",
                table: "PedidoItemArquivo",
                column: "PedidoItemArquivoAlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItemArquivoAlbum_PedidoItemId",
                table: "PedidoItemArquivoAlbum",
                column: "PedidoItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_PIAAlbum_PedidoItem_PedidoItemId",
                table: "PedidoItemArquivo",
                column: "PedidoItemArquivoAlbumId",
                principalTable: "PedidoItemArquivoAlbum",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PIAAlbum_PedidoItem_PedidoItemId",
                table: "PedidoItemArquivo");

            migrationBuilder.DropTable(
                name: "PedidoItemArquivoAlbum");

            migrationBuilder.DropIndex(
                name: "IX_PedidoItemArquivo_PedidoItemArquivoAlbumId",
                table: "PedidoItemArquivo");

            migrationBuilder.DropColumn(
                name: "PedidoItemArquivoAlbumId",
                table: "PedidoItemArquivo");
        }
    }
}
