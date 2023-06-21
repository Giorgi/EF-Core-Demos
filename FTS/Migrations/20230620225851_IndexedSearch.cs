using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

#nullable disable

namespace FTS.Migrations
{
    /// <inheritdoc />
    public partial class IndexedSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SearchDocument",
                table: "Posts",
                type: "tsvector",
                nullable: false,
                computedColumnSql: "setweight(to_tsvector('english', \"Title\"), 'A') || \r\n                                     setweight(to_tsvector('english', \"Body\"), 'B')  || \r\n                                     setweight(to_tsvector('simple', \"Author\"), 'C') || \r\n                                     setweight(to_tsvector('simple', \"Tags\"), 'B') ", stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_SearchDocument",
                table: "Posts",
                column: "SearchDocument")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Posts_SearchDocument");
            migrationBuilder.DropColumn("SearchDocument", "Posts");
        }
    }
}
