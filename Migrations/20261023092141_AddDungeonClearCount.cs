using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DungeonsAndDragqueens.Migrations
{
    /// <inheritdoc />
    public partial class AddDungeonClearCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClearCount",
                table: "CharacterDungeonProgress",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                "UPDATE \"CharacterDungeonProgress\" " +
                "SET \"ClearCount\" = 1, \"CurrentWave\" = 1 " +
                "WHERE \"IsCompleted\" = TRUE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClearCount",
                table: "CharacterDungeonProgress");
        }
    }
}
