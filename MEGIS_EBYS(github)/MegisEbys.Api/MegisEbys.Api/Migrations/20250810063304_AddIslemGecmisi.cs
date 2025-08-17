using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MegisEbys.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIslemGecmisi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Islemler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EvrakId = table.Column<int>(type: "int", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Islemler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Islemler_Evraklar_EvrakId",
                        column: x => x.EvrakId,
                        principalTable: "Evraklar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Islemler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Islemler_EvrakId",
                table: "Islemler",
                column: "EvrakId");

            migrationBuilder.CreateIndex(
                name: "IX_Islemler_KullaniciId",
                table: "Islemler",
                column: "KullaniciId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Islemler");
        }
    }
}
