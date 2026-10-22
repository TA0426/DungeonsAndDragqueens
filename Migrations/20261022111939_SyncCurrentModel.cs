using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DungeonsAndDragqueens.Migrations
{
    /// <inheritdoc />
    public partial class SyncCurrentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*             migrationBuilder.DropIndex(
                            name: "IX_CharacterDungeonProgress_CharacterId_DungeonId",
                            table: "CharacterDungeonProgress"); */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*             migrationBuilder.CreateIndex(
                            name: "IX_CharacterDungeonProgress_CharacterId_DungeonId",
                            table: "CharacterDungeonProgress",
                            columns: new[] { "CharacterId", "DungeonId" },
                            unique: true); */
        }
    }
}
