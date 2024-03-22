using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChampionshipApp.Migrations
{
    /// <inheritdoc />
    public partial class AlteredTeamType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TeamTypes");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "TeamTypes");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "TeamTypes");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "TeamTypes");

            migrationBuilder.AddColumn<int>(
                name: "TeamSize",
                table: "TeamTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamSize",
                table: "TeamTypes");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TeamTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "TeamTypes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "TeamTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "TeamTypes",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
