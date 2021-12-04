using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class AddressesListremoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressList",
                table: "User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressList",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
