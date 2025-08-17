using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MegisEbys.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddKullaniciAndEvrakModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Evraklar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Yonu = table.Column<int>(type: "int", nullable: false),
                    EvrakNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Konu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IlgiliKurum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    EvrakTuruId = table.Column<int>(type: "int", nullable: false),
                    SorumluBirimId = table.Column<int>(type: "int", nullable: true),
                    DosyaYolu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evraklar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evraklar_Birimler_SorumluBirimId",
                        column: x => x.SorumluBirimId,
                        principalTable: "Birimler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Evraklar_EvrakTurleri_EvrakTuruId",
                        column: x => x.EvrakTuruId,
                        principalTable: "EvrakTurleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdSoyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Eposta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Yetki = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    BirimId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kullanicilar_Birimler_BirimId",
                        column: x => x.BirimId,
                        principalTable: "Birimler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Evraklar_EvrakTuruId",
                table: "Evraklar",
                column: "EvrakTuruId");

            migrationBuilder.CreateIndex(
                name: "IX_Evraklar_SorumluBirimId",
                table: "Evraklar",
                column: "SorumluBirimId");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_BirimId",
                table: "Kullanicilar",
                column: "BirimId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evraklar");

            migrationBuilder.DropTable(
                name: "Kullanicilar");
        }
    }
}
