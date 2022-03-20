using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberStore.Infrastructure.Migrations
{
    public partial class CartProductAddedColumnOrdered : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ordered",
                table: "CartProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ordered",
                table: "CartProducts");
        }
    }
}
