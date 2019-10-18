using Microsoft.EntityFrameworkCore.Migrations;

namespace DTShop.OrderService.Data.Migrations
{
    public partial class FixUsername : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Orders",
                newName: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Orders",
                newName: "UserName");
        }
    }
}
