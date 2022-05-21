using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendance_System___ITI.Data.Migrations
{
    public partial class Dateanno : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Announcements");
        }
    }
}
