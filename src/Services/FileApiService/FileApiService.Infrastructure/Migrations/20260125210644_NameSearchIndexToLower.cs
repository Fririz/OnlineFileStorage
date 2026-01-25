using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileApiService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NameSearchIndexToLower : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Item_Name",
                table: "Item");

            migrationBuilder.AddColumn<string>(
                name: "NameToLower",
                table: "Item",
                type: "text",
                nullable: true,
                computedColumnSql: "lower(\"Name\")",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_NameToLower",
                table: "Item",
                column: "NameToLower")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Item_NameToLower",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "NameToLower",
                table: "Item");

            migrationBuilder.CreateIndex(
                name: "IX_Item_Name",
                table: "Item",
                column: "Name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }
    }
}
