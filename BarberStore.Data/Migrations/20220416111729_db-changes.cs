using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberStore.Infrastructure.Migrations
{
    public partial class dbchanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_AspNetUsers_PublishUserId",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_PublishUserId",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "PublishUserId",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Announcements");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Announcements",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublishUserId",
                table: "Announcements",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Announcements",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_PublishUserId",
                table: "Announcements",
                column: "PublishUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_AspNetUsers_PublishUserId",
                table: "Announcements",
                column: "PublishUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
