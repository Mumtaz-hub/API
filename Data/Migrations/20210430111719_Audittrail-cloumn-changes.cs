using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class Audittrailcloumnchanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangedColumns",
                table: "AuditTrail");

            migrationBuilder.RenameColumn(
                name: "AuditUser",
                table: "AuditTrail",
                newName: "UserId");

            migrationBuilder.AddColumn<string>(
                name: "AffectedColumns",
                table: "AuditTrail",
                type: "varchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AffectedColumns",
                table: "AuditTrail");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AuditTrail",
                newName: "AuditUser");

            migrationBuilder.AddColumn<string>(
                name: "ChangedColumns",
                table: "AuditTrail",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
