using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace hopmate.Server.Migrations
{
    /// <inheritdoc />
    public partial class InicialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "color",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_color", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "image",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    file_path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_image", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "location",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    postal_code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "request_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_request_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sponsor",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sponsor", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "trip_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    points = table.Column<int>(type: "int", nullable: false),
                    hops = table.Column<int>(type: "int", nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    image_file_path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    has_driving_license = table.Column<bool>(type: "bit", nullable: false),
                    user_name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "bit", nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    security_stamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "bit", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "bit", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "bit", nullable: false),
                    access_failed_count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "voucher_status",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_voucher_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    claim_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    claim_value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "FK_role_claims_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "driver",
                columns: table => new
                {
                    id_user = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    driving_license = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_driver", x => x.id_user);
                    table.ForeignKey(
                        name: "FK_driver_user_id_user",
                        column: x => x.id_user,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "passenger",
                columns: table => new
                {
                    id_user = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passenger", x => x.id_user);
                    table.ForeignKey(
                        name: "FK_passenger_user_id_user",
                        column: x => x.id_user,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "penalty",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    hops = table.Column<int>(type: "int", nullable: false),
                    points = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_user = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_penalty", x => x.id);
                    table.ForeignKey(
                        name: "FK_penalty_user_id_user",
                        column: x => x.id_user,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    claim_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    claim_value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_claims_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_logins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    provider_key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    provider_display_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "FK_user_logins_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    role_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_user_roles_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tokens",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    login_provider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "FK_user_tokens_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "voucher",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hops_cost = table.Column<int>(type: "int", nullable: false),
                    expiracy_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_sponsor = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    id_voucher_status = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    id_image = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_voucher", x => x.id);
                    table.ForeignKey(
                        name: "FK_voucher_image_id_image",
                        column: x => x.id_image,
                        principalTable: "image",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_voucher_sponsor_id_sponsor",
                        column: x => x.id_sponsor,
                        principalTable: "sponsor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_voucher_voucher_status_id_voucher_status",
                        column: x => x.id_voucher_status,
                        principalTable: "voucher_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reward",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    hops = table.Column<int>(type: "int", nullable: false),
                    points = table.Column<int>(type: "int", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_driver = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reward", x => x.id);
                    table.ForeignKey(
                        name: "FK_reward_driver_id_driver",
                        column: x => x.id_driver,
                        principalTable: "driver",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vehicle",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    plate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    seats = table.Column<int>(type: "int", nullable: false),
                    image_file_path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    id_driver = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    id_color = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicle", x => x.id);
                    table.ForeignKey(
                        name: "FK_vehicle_color_id_color",
                        column: x => x.id_color,
                        principalTable: "color",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vehicle_driver_id_driver",
                        column: x => x.id_driver,
                        principalTable: "driver",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "review",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date_review = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_driver = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    id_passenger = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_review", x => x.id);
                    table.ForeignKey(
                        name: "FK_review_driver_id_driver",
                        column: x => x.id_driver,
                        principalTable: "driver",
                        principalColumn: "id_user");
                    table.ForeignKey(
                        name: "FK_review_passenger_id_passenger",
                        column: x => x.id_passenger,
                        principalTable: "passenger",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "user_voucher",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    date_redeemed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    id_user = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    id_voucher = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_voucher", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_voucher_user_id_user",
                        column: x => x.id_user,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_voucher_voucher_id_voucher",
                        column: x => x.id_voucher,
                        principalTable: "voucher",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "trip",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    dt_departure = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    dt_arrival = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    available_seats = table.Column<int>(type: "int", nullable: false),
                    id_driver = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    id_vehicle = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    id_status_trip = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip", x => x.id);
                    table.ForeignKey(
                        name: "FK_trip_driver_id_driver",
                        column: x => x.id_driver,
                        principalTable: "driver",
                        principalColumn: "id_user");
                    table.ForeignKey(
                        name: "FK_trip_trip_status_id_status_trip",
                        column: x => x.id_status_trip,
                        principalTable: "trip_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_trip_vehicle_id_vehicle",
                        column: x => x.id_vehicle,
                        principalTable: "vehicle",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "passenger_trip",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    id_passenger = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    id_trip = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    id_location = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    id_request_status = table.Column<int>(type: "int", nullable: false),
                    location_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date_request = table.Column<DateTime>(type: "datetime2", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passenger_trip", x => x.id);
                    table.ForeignKey(
                        name: "FK_passenger_trip_location_id_location",
                        column: x => x.id_location,
                        principalTable: "location",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_passenger_trip_passenger_id_passenger",
                        column: x => x.id_passenger,
                        principalTable: "passenger",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_passenger_trip_request_status_id_request_status",
                        column: x => x.id_request_status,
                        principalTable: "request_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_passenger_trip_trip_id_trip",
                        column: x => x.id_trip,
                        principalTable: "trip",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trip_location",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    id_trip = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    id_location = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    is_start = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trip_location", x => x.id);
                    table.ForeignKey(
                        name: "FK_trip_location_location_id_location",
                        column: x => x.id_location,
                        principalTable: "location",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_location_trip_id_trip",
                        column: x => x.id_trip,
                        principalTable: "trip",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "color",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Black" },
                    { 2, "White" },
                    { 3, "Red" },
                    { 4, "Blue" },
                    { 5, "Gray" },
                    { 6, "Silver" },
                    { 7, "Green" },
                    { 8, "Yellow" },
                    { 9, "Orange" },
                    { 10, "Brown" }
                });

            migrationBuilder.InsertData(
                table: "request_status",
                columns: new[] { "id", "status" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Accepted" },
                    { 3, "Rejected" },
                    { 4, "WaitingList" },
                    { 5, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "trip_status",
                columns: new[] { "id", "status" },
                values: new object[,]
                {
                    { 1, "Planned" },
                    { 2, "In Progress" },
                    { 3, "Completed" },
                    { 4, "Cancelled" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_passenger_trip_id_location",
                table: "passenger_trip",
                column: "id_location");

            migrationBuilder.CreateIndex(
                name: "IX_passenger_trip_id_passenger",
                table: "passenger_trip",
                column: "id_passenger");

            migrationBuilder.CreateIndex(
                name: "IX_passenger_trip_id_request_status",
                table: "passenger_trip",
                column: "id_request_status");

            migrationBuilder.CreateIndex(
                name: "IX_passenger_trip_id_trip",
                table: "passenger_trip",
                column: "id_trip");

            migrationBuilder.CreateIndex(
                name: "IX_penalty_id_user",
                table: "penalty",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_review_id_driver",
                table: "review",
                column: "id_driver");

            migrationBuilder.CreateIndex(
                name: "IX_review_id_passenger",
                table: "review",
                column: "id_passenger");

            migrationBuilder.CreateIndex(
                name: "IX_reward_id_driver",
                table: "reward",
                column: "id_driver");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "role",
                column: "normalized_name",
                unique: true,
                filter: "[normalized_name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_role_claims_role_id",
                table: "role_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_id_driver",
                table: "trip",
                column: "id_driver");

            migrationBuilder.CreateIndex(
                name: "IX_trip_id_status_trip",
                table: "trip",
                column: "id_status_trip");

            migrationBuilder.CreateIndex(
                name: "IX_trip_id_vehicle",
                table: "trip",
                column: "id_vehicle");

            migrationBuilder.CreateIndex(
                name: "IX_trip_location_id_location",
                table: "trip_location",
                column: "id_location");

            migrationBuilder.CreateIndex(
                name: "IX_trip_location_id_trip",
                table: "trip_location",
                column: "id_trip");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "user",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "user",
                column: "normalized_user_name",
                unique: true,
                filter: "[normalized_user_name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_user_claims_user_id",
                table: "user_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_logins_user_id",
                table: "user_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_voucher_id_user",
                table: "user_voucher",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_user_voucher_id_voucher",
                table: "user_voucher",
                column: "id_voucher");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_id_color",
                table: "vehicle",
                column: "id_color");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_id_driver",
                table: "vehicle",
                column: "id_driver");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_id_image",
                table: "voucher",
                column: "id_image");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_id_sponsor",
                table: "voucher",
                column: "id_sponsor");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_id_voucher_status",
                table: "voucher",
                column: "id_voucher_status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "passenger_trip");

            migrationBuilder.DropTable(
                name: "penalty");

            migrationBuilder.DropTable(
                name: "review");

            migrationBuilder.DropTable(
                name: "reward");

            migrationBuilder.DropTable(
                name: "role_claims");

            migrationBuilder.DropTable(
                name: "trip_location");

            migrationBuilder.DropTable(
                name: "user_claims");

            migrationBuilder.DropTable(
                name: "user_logins");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "user_tokens");

            migrationBuilder.DropTable(
                name: "user_voucher");

            migrationBuilder.DropTable(
                name: "request_status");

            migrationBuilder.DropTable(
                name: "passenger");

            migrationBuilder.DropTable(
                name: "location");

            migrationBuilder.DropTable(
                name: "trip");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "voucher");

            migrationBuilder.DropTable(
                name: "trip_status");

            migrationBuilder.DropTable(
                name: "vehicle");

            migrationBuilder.DropTable(
                name: "image");

            migrationBuilder.DropTable(
                name: "sponsor");

            migrationBuilder.DropTable(
                name: "voucher_status");

            migrationBuilder.DropTable(
                name: "color");

            migrationBuilder.DropTable(
                name: "driver");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
