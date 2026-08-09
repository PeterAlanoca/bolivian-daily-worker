using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolivianDaily.ScraperWorker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropArticleMediaState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "state",
                table: "article_media");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "article_media",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "A");
        }
    }
}
