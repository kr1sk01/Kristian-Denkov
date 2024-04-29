using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChampionshipMaster.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedOnline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Online",
                table: "AspNetUsers",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Online",
                table: "AspNetUsers");
        }
    }
}
