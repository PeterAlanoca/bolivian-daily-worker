using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolivianDaily.CheckerWorker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "state",
                table: "checked_articles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "checked_articles",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "A");
        }
    }
}
