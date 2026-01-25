using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripPlanner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "attractions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    latitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: false),
                    rating = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: true),
                    review_count = table.Column<int>(type: "integer", nullable: true),
                    estimated_duration = table.Column<int>(type: "integer", nullable: true),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attractions", x => x.id);
                    table.ForeignKey(
                        name: "FK_attractions_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_attractions_profiles_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "trips",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_public = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    daily_hours = table.Column<int>(type: "integer", nullable: false, defaultValue: 8),
                    max_extension_hours = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false, defaultValue: new TimeOnly(9, 0, 0)),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trips", x => x.id);
                    table.ForeignKey(
                        name: "FK_trips_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trips_profiles_owner_id",
                        column: x => x.owner_id,
                        principalTable: "profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trip_attractions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    trip_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attraction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    day_number = table.Column<int>(type: "integer", nullable: false),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    planned_start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_attractions", x => x.id);
                    table.CheckConstraint("chk_day_number_positive", "day_number > 0");
                    table.CheckConstraint("chk_order_index_positive", "order_index > 0");
                    table.ForeignKey(
                        name: "FK_trip_attractions_attractions_attraction_id",
                        column: x => x.attraction_id,
                        principalTable: "attractions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trip_attractions_trips_trip_id",
                        column: x => x.trip_id,
                        principalTable: "trips",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_attractions_created_by",
                table: "attractions",
                column: "created_by_user_id",
                filter: "created_by_user_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_attractions_location_rating",
                table: "attractions",
                columns: new[] { "location_id", "rating" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "idx_attractions_name",
                table: "attractions",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "idx_locations_name_country",
                table: "locations",
                columns: new[] { "name", "country" });

            migrationBuilder.CreateIndex(
                name: "idx_trip_attractions_attraction",
                table: "trip_attractions",
                column: "attraction_id");

            migrationBuilder.CreateIndex(
                name: "idx_trip_attractions_trip_day_order",
                table: "trip_attractions",
                columns: new[] { "trip_id", "day_number", "order_index" });

            migrationBuilder.CreateIndex(
                name: "uq_trip_attraction",
                table: "trip_attractions",
                columns: new[] { "trip_id", "attraction_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_trips_location",
                table: "trips",
                column: "location_id",
                filter: "location_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_trips_owner_created",
                table: "trips",
                columns: new[] { "owner_id", "created_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "idx_trips_public",
                table: "trips",
                column: "created_at",
                descending: new bool[0],
                filter: "is_public = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trip_attractions");

            migrationBuilder.DropTable(
                name: "attractions");

            migrationBuilder.DropTable(
                name: "trips");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "profiles");
        }
    }
}
