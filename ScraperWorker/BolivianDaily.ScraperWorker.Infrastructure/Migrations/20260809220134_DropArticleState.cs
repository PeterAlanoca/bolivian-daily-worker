using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolivianDaily.ScraperWorker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropArticleState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "state",
                table: "articles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "articles",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "A");
        }
    }
}
