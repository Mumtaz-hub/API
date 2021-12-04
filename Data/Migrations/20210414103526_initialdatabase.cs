using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class initialdatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditTrail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuditDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuditType = table.Column<byte>(type: "tinyint", nullable: false),
                    AuditUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedColumns = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommandAudit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoggedOnUserId = table.Column<long>(type: "bigint", nullable: false),
                    UtcTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    ExceptionMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Milliseconds = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandAudit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsAccountLocked = table.Column<bool>(type: "bit", nullable: false),
                    IsSystemUser = table.Column<bool>(type: "bit", nullable: false),
                    RoleType = table.Column<byte>(type: "tinyint", nullable: false),
                    CreationTs = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastChangeTs = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangeUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_EmailAddress",
                table: "User",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_MobileNumber",
                table: "User",
                column: "MobileNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTrail");

            migrationBuilder.DropTable(
                name: "CommandAudit");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
