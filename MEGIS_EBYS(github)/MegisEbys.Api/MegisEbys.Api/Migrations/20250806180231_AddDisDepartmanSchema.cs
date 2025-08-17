using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MegisEbys.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDisDepartmanSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisKullanicilar_Kurumlar_KurumId",
                table: "DisKullanicilar");

            migrationBuilder.RenameColumn(
                name: "KurumId",
                table: "DisKullanicilar",
                newName: "DisDepartmanId");

            migrationBuilder.RenameIndex(
                name: "IX_DisKullanicilar_KurumId",
                table: "DisKullanicilar",
                newName: "IX_DisKullanicilar_DisDepartmanId");

            migrationBuilder.CreateTable(
                name: "DisDepartmanlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KurumId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisDepartmanlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisDepartmanlar_Kurumlar_KurumId",
                        column: x => x.KurumId,
                        principalTable: "Kurumlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisDepartmanlar_KurumId",
                table: "DisDepartmanlar",
                column: "KurumId");

            migrationBuilder.AddForeignKey(
                name: "FK_DisKullanicilar_DisDepartmanlar_DisDepartmanId",
                table: "DisKullanicilar",
                column: "DisDepartmanId",
                principalTable: "DisDepartmanlar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisKullanicilar_DisDepartmanlar_DisDepartmanId",
                table: "DisKullanicilar");

            migrationBuilder.DropTable(
                name: "DisDepartmanlar");

            migrationBuilder.RenameColumn(
                name: "DisDepartmanId",
                table: "DisKullanicilar",
                newName: "KurumId");

            migrationBuilder.RenameIndex(
                name: "IX_DisKullanicilar_DisDepartmanId",
                table: "DisKullanicilar",
                newName: "IX_DisKullanicilar_KurumId");

            migrationBuilder.AddForeignKey(
                name: "FK_DisKullanicilar_Kurumlar_KurumId",
                table: "DisKullanicilar",
                column: "KurumId",
                principalTable: "Kurumlar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
