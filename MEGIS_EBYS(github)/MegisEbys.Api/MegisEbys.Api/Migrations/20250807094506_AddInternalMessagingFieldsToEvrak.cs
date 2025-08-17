using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MegisEbys.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddInternalMessagingFieldsToEvrak : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IlgiliKurum",
                table: "Evraklar",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "AliciBirimId",
                table: "Evraklar",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AliciKullaniciId",
                table: "Evraklar",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DahiliMi",
                table: "Evraklar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "GonderenKullaniciId",
                table: "Evraklar",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evraklar_AliciBirimId",
                table: "Evraklar",
                column: "AliciBirimId");

            migrationBuilder.CreateIndex(
                name: "IX_Evraklar_AliciKullaniciId",
                table: "Evraklar",
                column: "AliciKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Evraklar_GonderenKullaniciId",
                table: "Evraklar",
                column: "GonderenKullaniciId");

            migrationBuilder.AddForeignKey(
                name: "FK_Evraklar_Birimler_AliciBirimId",
                table: "Evraklar",
                column: "AliciBirimId",
                principalTable: "Birimler",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Evraklar_Kullanicilar_AliciKullaniciId",
                table: "Evraklar",
                column: "AliciKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Evraklar_Kullanicilar_GonderenKullaniciId",
                table: "Evraklar",
                column: "GonderenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evraklar_Birimler_AliciBirimId",
                table: "Evraklar");

            migrationBuilder.DropForeignKey(
                name: "FK_Evraklar_Kullanicilar_AliciKullaniciId",
                table: "Evraklar");

            migrationBuilder.DropForeignKey(
                name: "FK_Evraklar_Kullanicilar_GonderenKullaniciId",
                table: "Evraklar");

            migrationBuilder.DropIndex(
                name: "IX_Evraklar_AliciBirimId",
                table: "Evraklar");

            migrationBuilder.DropIndex(
                name: "IX_Evraklar_AliciKullaniciId",
                table: "Evraklar");

            migrationBuilder.DropIndex(
                name: "IX_Evraklar_GonderenKullaniciId",
                table: "Evraklar");

            migrationBuilder.DropColumn(
                name: "AliciBirimId",
                table: "Evraklar");

            migrationBuilder.DropColumn(
                name: "AliciKullaniciId",
                table: "Evraklar");

            migrationBuilder.DropColumn(
                name: "DahiliMi",
                table: "Evraklar");

            migrationBuilder.DropColumn(
                name: "GonderenKullaniciId",
                table: "Evraklar");

            migrationBuilder.AlterColumn<string>(
                name: "IlgiliKurum",
                table: "Evraklar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
