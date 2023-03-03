using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Migrations
{
    public partial class person7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seminar_Person_Personid",
                table: "Seminar");

            migrationBuilder.DropIndex(
                name: "IX_Seminar_Personid",
                table: "Seminar");

            migrationBuilder.DropColumn(
                name: "Personid",
                table: "Seminar");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "Person",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "Person");

            migrationBuilder.AddColumn<int>(
                name: "Personid",
                table: "Seminar",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seminar_Personid",
                table: "Seminar",
                column: "Personid");

            migrationBuilder.AddForeignKey(
                name: "FK_Seminar_Person_Personid",
                table: "Seminar",
                column: "Personid",
                principalTable: "Person",
                principalColumn: "id");
        }
    }
}
