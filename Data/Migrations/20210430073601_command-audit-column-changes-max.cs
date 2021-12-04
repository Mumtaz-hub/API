using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class commandauditcolumnchangesmax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ExceptionMessage",
                table: "CommandAudit",
                type: "varchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ExceptionMessage",
                table: "CommandAudit",
                type: "varchar",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldNullable: true);
        }
    }
}
