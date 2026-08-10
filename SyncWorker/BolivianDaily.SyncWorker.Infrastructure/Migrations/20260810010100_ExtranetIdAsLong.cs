using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BolivianDaily.SyncWorker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExtranetIdAsLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"synced_articles\" ALTER COLUMN \"extranet_id\" TYPE bigint USING \"extranet_id\"::bigint;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"synced_articles\" ALTER COLUMN \"extranet_id\" TYPE character varying(250) USING \"extranet_id\"::text;");
        }
    }
}
