using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BolivianDaily.CheckerWorker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "checked_articles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    scraped_article_id = table.Column<long>(type: "bigint", nullable: false),
                    source_id = table.Column<long>(type: "bigint", nullable: true),
                    source_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    source_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    url = table.Column<string>(type: "text", nullable: false),
                    category_id = table.Column<long>(type: "bigint", nullable: true),
                    category_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    title = table.Column<string>(type: "text", nullable: false),
                    pretitle = table.Column<string>(type: "text", nullable: true),
                    subtitle = table.Column<string>(type: "text", nullable: true),
                    lead = table.Column<string>(type: "text", nullable: true),
                    body = table.Column<string>(type: "text", nullable: false),
                    author = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    scraped_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    state = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false, defaultValue: "A"),
                    checked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    warnings = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_valid = table.Column<bool>(type: "boolean", nullable: false),
                    html_format_passed = table.Column<bool>(type: "boolean", nullable: false),
                    html_format_reason = table.Column<string>(type: "text", nullable: true),
                    category_accuracy_passed = table.Column<bool>(type: "boolean", nullable: false),
                    category_accuracy_reason = table.Column<string>(type: "text", nullable: true),
                    suggested_category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    no_advertising_passed = table.Column<bool>(type: "boolean", nullable: false),
                    no_advertising_reason = table.Column<string>(type: "text", nullable: true),
                    detected_networks = table.Column<string>(type: "text", nullable: true),
                    ready_to_publish_passed = table.Column<bool>(type: "boolean", nullable: false),
                    ready_to_publish_reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checked_articles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "checked_article_media",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    checked_article_id = table.Column<long>(type: "bigint", nullable: false),
                    url = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checked_article_media", x => x.id);
                    table.ForeignKey(
                        name: "FK_checked_article_media_checked_articles_checked_article_id",
                        column: x => x.checked_article_id,
                        principalTable: "checked_articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_checked_article_media_checked_article_id",
                table: "checked_article_media",
                column: "checked_article_id");

            migrationBuilder.CreateIndex(
                name: "IX_checked_articles_scraped_article_id",
                table: "checked_articles",
                column: "scraped_article_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_checked_articles_url",
                table: "checked_articles",
                column: "url",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "checked_article_media");

            migrationBuilder.DropTable(
                name: "checked_articles");
        }
    }
}
