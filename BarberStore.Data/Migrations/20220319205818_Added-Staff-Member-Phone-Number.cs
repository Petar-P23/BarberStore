using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberStore.Infrastructure.Migrations
{
    public partial class AddedStaffMemberPhoneNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "StaffMembers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "StaffMembers");
        }
    }
}
