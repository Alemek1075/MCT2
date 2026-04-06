using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCT.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullableMatchTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "matches_match_type_fkey",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "matches_team_a_id_fkey",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "matches_team_b_id_fkey",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "matches_tournament_id_fkey",
                table: "matches");

            migrationBuilder.DropColumn(
                name: "qr_code",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "member_count",
                table: "teams");

            migrationBuilder.RenameColumn(
                name: "Places",
                table: "tournaments",
                newName: "places");

            migrationBuilder.AlterColumn<int>(
                name: "tournament_id",
                table: "matches",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "team_b_id",
                table: "matches",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "team_a_id",
                table: "matches",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "scheduled_at",
                table: "matches",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "match_type",
                table: "matches",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.InsertData(
                table: "user_roles",
                column: "role_name",
                values: new object[]
                {
                    "Admin",
                    "Player",
                    "User"
                });

            migrationBuilder.AddForeignKey(
                name: "matches_match_type_fkey",
                table: "matches",
                column: "match_type",
                principalTable: "match_types",
                principalColumn: "type_name");

            migrationBuilder.AddForeignKey(
                name: "matches_team_a_id_fkey",
                table: "matches",
                column: "team_a_id",
                principalTable: "teams",
                principalColumn: "team_id");

            migrationBuilder.AddForeignKey(
                name: "matches_team_b_id_fkey",
                table: "matches",
                column: "team_b_id",
                principalTable: "teams",
                principalColumn: "team_id");

            migrationBuilder.AddForeignKey(
                name: "matches_tournament_id_fkey",
                table: "matches",
                column: "tournament_id",
                principalTable: "tournaments",
                principalColumn: "tournament_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "matches_match_type_fkey",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "matches_team_a_id_fkey",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "matches_team_b_id_fkey",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "matches_tournament_id_fkey",
                table: "matches");

            migrationBuilder.DeleteData(
                table: "user_roles",
                keyColumn: "role_name",
                keyValue: "Admin");

            migrationBuilder.DeleteData(
                table: "user_roles",
                keyColumn: "role_name",
                keyValue: "Player");

            migrationBuilder.DeleteData(
                table: "user_roles",
                keyColumn: "role_name",
                keyValue: "User");

            migrationBuilder.RenameColumn(
                name: "places",
                table: "tournaments",
                newName: "Places");

            migrationBuilder.AddColumn<string>(
                name: "qr_code",
                table: "tickets",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "member_count",
                table: "teams",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "tournament_id",
                table: "matches",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "team_b_id",
                table: "matches",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "team_a_id",
                table: "matches",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "scheduled_at",
                table: "matches",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "match_type",
                table: "matches",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "matches_match_type_fkey",
                table: "matches",
                column: "match_type",
                principalTable: "match_types",
                principalColumn: "type_name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "matches_team_a_id_fkey",
                table: "matches",
                column: "team_a_id",
                principalTable: "teams",
                principalColumn: "team_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "matches_team_b_id_fkey",
                table: "matches",
                column: "team_b_id",
                principalTable: "teams",
                principalColumn: "team_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "matches_tournament_id_fkey",
                table: "matches",
                column: "tournament_id",
                principalTable: "tournaments",
                principalColumn: "tournament_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
