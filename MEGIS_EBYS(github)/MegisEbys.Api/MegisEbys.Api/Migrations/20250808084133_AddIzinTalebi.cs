using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MegisEbys.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIzinTalebi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IzinTalepleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    TalepTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TalepEdenKullaniciId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IzinTalepleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IzinTalepleri_Kullanicilar_TalepEdenKullaniciId",
                        column: x => x.TalepEdenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IzinTalepleri_TalepEdenKullaniciId",
                table: "IzinTalepleri",
                column: "TalepEdenKullaniciId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IzinTalepleri");
        }
    }
}
