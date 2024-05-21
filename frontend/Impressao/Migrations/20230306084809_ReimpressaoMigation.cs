using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Impressao.Migrations
{
    /// <inheritdoc />
    public partial class ReimpressaoMigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PedidoReimpressao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PedidoItemId = table.Column<int>(type: "integer", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataImposicionamento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoReimpressao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoReimpressao_PedidoItem_PedidoItemId",
                        column: x => x.PedidoItemId,
                        principalTable: "PedidoItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoReimpressaoArquivo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PedidoReimpressaoId = table.Column<int>(type: "integer", nullable: false),
                    ArquivoFrenteId = table.Column<int>(type: "integer", nullable: false),
                    ArquivoVersoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoReimpressaoArquivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoReimpressaoArquivo_PedidoItemArquivo_ArquivoFrenteId",
                        column: x => x.ArquivoFrenteId,
                        principalTable: "PedidoItemArquivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoReimpressaoArquivo_PedidoItemArquivo_ArquivoVersoId",
                        column: x => x.ArquivoVersoId,
                        principalTable: "PedidoItemArquivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoReimpressaoArquivo_PedidoReimpressao_PedidoReimpressa~",
                        column: x => x.PedidoReimpressaoId,
                        principalTable: "PedidoReimpressao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PedidoReimpressao_PedidoItemId",
                table: "PedidoReimpressao",
                column: "PedidoItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoReimpressaoArquivo_ArquivoFrenteId",
                table: "PedidoReimpressaoArquivo",
                column: "ArquivoFrenteId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoReimpressaoArquivo_ArquivoVersoId",
                table: "PedidoReimpressaoArquivo",
                column: "ArquivoVersoId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoReimpressaoArquivo_PedidoReimpressaoId",
                table: "PedidoReimpressaoArquivo",
                column: "PedidoReimpressaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PedidoReimpressaoArquivo");

            migrationBuilder.DropTable(
                name: "PedidoReimpressao");
        }
    }
}
