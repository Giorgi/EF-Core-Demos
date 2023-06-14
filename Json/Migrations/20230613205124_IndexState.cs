using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Json.Migrations
{
    /// <inheritdoc />
    public partial class IndexState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: false,
                computedColumnSql: "JSON_VALUE([BillingAddress], '$.State')");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_State",
                table: "Employees",
                column: "State");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_State",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Employees");
        }
    }
}
