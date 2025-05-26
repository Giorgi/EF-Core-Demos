using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Json.Migrations
{
    /// <inheritdoc />
    public partial class Dates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImportantDates",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportantDates",
                table: "Employees");
        }
    }
}
