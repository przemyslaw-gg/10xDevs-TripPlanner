using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AllowZeroOrderIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "chk_order_index_positive",
                table: "trip_attractions");

            migrationBuilder.AddCheckConstraint(
                name: "chk_order_index_non_negative",
                table: "trip_attractions",
                sql: "order_index >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "chk_order_index_non_negative",
                table: "trip_attractions");

            migrationBuilder.AddCheckConstraint(
                name: "chk_order_index_positive",
                table: "trip_attractions",
                sql: "order_index > 0");
        }
    }
}
