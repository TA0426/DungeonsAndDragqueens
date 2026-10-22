using DungeonsAndDragqueens.WebApi.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DungeonsAndDragqueens.Migrations;

[DbContext(typeof(StoreMonstersContext))]
[Migration("20261022170000_AddDungeonProgression")]
public partial class AddDungeonProgression : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CharacterDungeonProgress",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                DungeonId = table.Column<string>(type: "text", nullable: false),
                CurrentWave = table.Column<int>(type: "integer", nullable: false),
                IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_CharacterDungeonProgress", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_CharacterDungeonProgress_CharacterId_DungeonId",
            table: "CharacterDungeonProgress",
            columns: new[] { "CharacterId", "DungeonId" }, unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable("CharacterDungeonProgress");
}
