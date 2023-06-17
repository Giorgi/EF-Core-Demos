using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.SqlServer.Types;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HierarchicalData.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateHierarchical : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<SqlHierarchyId>(type: "hierarchyid", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "Id", "Name", "Path" },
                values: new object[,]
                {
                    { -34, "Head of Reporting Department", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/4/2/") },
                    { -33, "Head of Tax Department", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/4/2/") },
                    { -32, "Finance Analyst", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/4/1/2/1/") },
                    { -31, "Head of Budgeting", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/4/1/2/") },
                    { -30, "Accountant", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/4/1/1/1/1/") },
                    { -29, "Accounting Manager", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/4/1/1/1/") },
                    { -28, "Chief Accountant", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/4/1/1/") },
                    { -27, "Corporate Vice President Finances", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/4/1/") },
                    { -26, "CFO", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/4/") },
                    { -25, "Marketing Brand Manager", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/3/3/1/") },
                    { -24, "Marketing Brand Head", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/3/3/") },
                    { -23, "Advertising Agency Specialist", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/3/2/1/1/") },
                    { -22, "Advertising Agency Director", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/3/2/1/") },
                    { -21, "Marketing Advertising Head", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/3/2/") },
                    { -20, "Marketing Communications Intern", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/3/1/1/1/") },
                    { -19, "Marketing Communications Jr. Officer", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/3/1/3/") },
                    { -18, "Marketing Social Communications Sr. Officer", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/3/1/2/") },
                    { -17, "Marketing Communications Sr. Officer", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/3/1/1/") },
                    { -16, "Marketing Communications Head", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/3/1/") },
                    { -15, "Chief Marketing Officer", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/3/") },
                    { -14, "Chief Web3 Developer", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/2/2/3/") },
                    { -13, "Chief Prompt Engineer", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/2/2/2/") },
                    { -12, "Chief AI Bot", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/2/2/1/") },
                    { -11, "Head of AI, ChatGPT, BlockChain and other buzzwords", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/2/2/") },
                    { -10, "Frontend Team Lead", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/2/1/2/") },
                    { -9, ".Net Junior Engineer", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/2/1/1/2/") },
                    { -8, ".Net Senior Engineer", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/2/1/1/1/") },
                    { -7, ".Net Team Lead", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/2/1/1/") },
                    { -6, "Head of Software Development", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/2/1/") },
                    { -5, "CTO", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/2/") },
                    { -4, "Enterprise Sales", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/1/2/") },
                    { -3, "Global Retail", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/1/1/") },
                    { -2, "Chief Commercial Officer", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/1/") },
                    { -1, "CEO", Microsoft.SqlServer.Types.SqlHierarchyId.Parse("/") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Positions");
        }
    }
}
