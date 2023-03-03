using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Migrations
{
    public partial class registration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Registration",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    attendeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    seminarId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registration", x => x.id);
                    table.ForeignKey(
                        name: "FK_Registration_Person_attendeeId",
                        column: x => x.attendeeId,
                        principalTable: "Person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Registration_Seminar_seminarId",
                        column: x => x.seminarId,
                        principalTable: "Seminar",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Registration_attendeeId",
                table: "Registration",
                column: "attendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_seminarId",
                table: "Registration",
                column: "seminarId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Registration");
        }
    }
}
