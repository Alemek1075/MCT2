using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MCT.Migrations
{
    /// <inheritdoc />
    public partial class Migr1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "match_types",
                columns: table => new
                {
                    type_name = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("match_types_pkey", x => x.type_name);
                });

            migrationBuilder.CreateTable(
                name: "payment_statuses",
                columns: table => new
                {
                    status_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("payment_statuses_pkey", x => x.status_name);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    team_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    short_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    region = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    member_count = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("teams_pkey", x => x.team_id);
                });

            migrationBuilder.CreateTable(
                name: "ticket_statuses",
                columns: table => new
                {
                    status_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ticket_statuses_pkey", x => x.status_name);
                });

            migrationBuilder.CreateTable(
                name: "tournament_statuses",
                columns: table => new
                {
                    status_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tournament_statuses_pkey", x => x.status_name);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    role_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_roles_pkey", x => x.role_name);
                });

            migrationBuilder.CreateTable(
                name: "tournaments",
                columns: table => new
                {
                    tournament_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Places = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tournaments_pkey", x => x.tournament_id);
                    table.ForeignKey(
                        name: "tournaments_status_fkey",
                        column: x => x.status,
                        principalTable: "tournament_statuses",
                        principalColumn: "status_name");
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.user_id);
                    table.ForeignKey(
                        name: "users_role_fkey",
                        column: x => x.role,
                        principalTable: "user_roles",
                        principalColumn: "role_name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    match_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tournament_id = table.Column<int>(type: "integer", nullable: false),
                    team_a_id = table.Column<int>(type: "integer", nullable: false),
                    team_b_id = table.Column<int>(type: "integer", nullable: false),
                    winner_id = table.Column<int>(type: "integer", nullable: true),
                    scheduled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    score_a = table.Column<int>(type: "integer", nullable: true),
                    score_b = table.Column<int>(type: "integer", nullable: true),
                    match_type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("matches_pkey", x => x.match_id);
                    table.ForeignKey(
                        name: "matches_match_type_fkey",
                        column: x => x.match_type,
                        principalTable: "match_types",
                        principalColumn: "type_name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "matches_team_a_id_fkey",
                        column: x => x.team_a_id,
                        principalTable: "teams",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "matches_team_b_id_fkey",
                        column: x => x.team_b_id,
                        principalTable: "teams",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "matches_tournament_id_fkey",
                        column: x => x.tournament_id,
                        principalTable: "tournaments",
                        principalColumn: "tournament_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "matches_winner_id_fkey",
                        column: x => x.winner_id,
                        principalTable: "teams",
                        principalColumn: "team_id");
                });

            migrationBuilder.CreateTable(
                name: "tournament_teams",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tournament_id = table.Column<int>(type: "integer", nullable: true),
                    team_id = table.Column<int>(type: "integer", nullable: true),
                    placement = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tournament_teams_pkey", x => x.id);
                    table.ForeignKey(
                        name: "tournament_teams_team_id_fkey",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "team_id");
                    table.ForeignKey(
                        name: "tournament_teams_tournament_id_fkey",
                        column: x => x.tournament_id,
                        principalTable: "tournaments",
                        principalColumn: "tournament_id");
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    player_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    team_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("players_pkey", x => x.player_id);
                    table.ForeignKey(
                        name: "players_team_id_fkey",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "team_id");
                    table.ForeignKey(
                        name: "players_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "tickets",
                columns: table => new
                {
                    ticket_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    tournament_id = table.Column<int>(type: "integer", nullable: false),
                    purchase_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    qr_code = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tickets_pkey", x => x.ticket_id);
                    table.ForeignKey(
                        name: "tickets_status_fkey",
                        column: x => x.status,
                        principalTable: "ticket_statuses",
                        principalColumn: "status_name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "tickets_tournament_id_fkey",
                        column: x => x.tournament_id,
                        principalTable: "tournaments",
                        principalColumn: "tournament_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "tickets_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stats",
                columns: table => new
                {
                    stat_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_id = table.Column<int>(type: "integer", nullable: true),
                    match_id = table.Column<int>(type: "integer", nullable: true),
                    kills = table.Column<int>(type: "integer", nullable: true),
                    deaths = table.Column<int>(type: "integer", nullable: true),
                    assists = table.Column<int>(type: "integer", nullable: true),
                    hs_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("stats_pkey", x => x.stat_id);
                    table.ForeignKey(
                        name: "stats_match_id_fkey",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "match_id");
                    table.ForeignKey(
                        name: "stats_player_id_fkey",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "player_id");
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ticket_id = table.Column<int>(type: "integer", nullable: true),
                    transaction_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    payment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("payments_pkey", x => x.payment_id);
                    table.ForeignKey(
                        name: "payments_status_fkey",
                        column: x => x.status,
                        principalTable: "payment_statuses",
                        principalColumn: "status_name");
                    table.ForeignKey(
                        name: "payments_ticket_id_fkey",
                        column: x => x.ticket_id,
                        principalTable: "tickets",
                        principalColumn: "ticket_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_matches_match_type",
                table: "matches",
                column: "match_type");

            migrationBuilder.CreateIndex(
                name: "IX_matches_team_a_id",
                table: "matches",
                column: "team_a_id");

            migrationBuilder.CreateIndex(
                name: "IX_matches_team_b_id",
                table: "matches",
                column: "team_b_id");

            migrationBuilder.CreateIndex(
                name: "IX_matches_tournament_id",
                table: "matches",
                column: "tournament_id");

            migrationBuilder.CreateIndex(
                name: "IX_matches_winner_id",
                table: "matches",
                column: "winner_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_status",
                table: "payments",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_payments_ticket_id",
                table: "payments",
                column: "ticket_id");

            migrationBuilder.CreateIndex(
                name: "IX_players_team_id",
                table: "players",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_players_user_id",
                table: "players",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_stats_match_id",
                table: "stats",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_stats_player_id",
                table: "stats",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_status",
                table: "tickets",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_tournament_id",
                table: "tickets",
                column: "tournament_id");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_user_id",
                table: "tickets",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tournament_teams_team_id",
                table: "tournament_teams",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_tournament_teams_tournament_id",
                table: "tournament_teams",
                column: "tournament_id");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_status",
                table: "tournaments",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_users_role",
                table: "users",
                column: "role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "stats");

            migrationBuilder.DropTable(
                name: "tournament_teams");

            migrationBuilder.DropTable(
                name: "payment_statuses");

            migrationBuilder.DropTable(
                name: "tickets");

            migrationBuilder.DropTable(
                name: "matches");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "ticket_statuses");

            migrationBuilder.DropTable(
                name: "match_types");

            migrationBuilder.DropTable(
                name: "tournaments");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "tournament_statuses");

            migrationBuilder.DropTable(
                name: "user_roles");
        }
    }
}
