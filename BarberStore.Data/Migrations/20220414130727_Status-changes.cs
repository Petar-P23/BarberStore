using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberStore.Infrastructure.Migrations
{
    public partial class Statuschanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentService");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentService",
                columns: table => new
                {
                    AppointmentsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServicesToDoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentService", x => new { x.AppointmentsId, x.ServicesToDoId });
                    table.ForeignKey(
                        name: "FK_AppointmentService_Appointments_AppointmentsId",
                        column: x => x.AppointmentsId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentService_Services_ServicesToDoId",
                        column: x => x.ServicesToDoId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentService_ServicesToDoId",
                table: "AppointmentService",
                column: "ServicesToDoId");
        }
    }
}
