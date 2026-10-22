using DungeonsAndDragqueens.WebApi.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DungeonsAndDragqueens.Migrations;

[DbContext(typeof(StoreMonstersContext))]
[Migration("20261022160000_AddCharacterInventoryAndLoot")]
public partial class AddCharacterInventoryAndLoot : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        foreach (var levelColumn in new[] { "SwordLevel", "BootsLevel", "ChestLevel", "LegsLevel", "GlovesLevel", "HelmetLevel" })
            migrationBuilder.AddColumn<int>(name: levelColumn, table: "Characters", type: "integer", nullable: false, defaultValue: 1);

        migrationBuilder.CreateTable(
            name: "CharacterInventoryItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                SwordId = table.Column<Guid>(type: "uuid", nullable: true),
                ArmorId = table.Column<Guid>(type: "uuid", nullable: true),
                Level = table.Column<int>(type: "integer", nullable: false),
                Quantity = table.Column<int>(type: "integer", nullable: false)
            }, constraints: table => table.PrimaryKey("PK_CharacterInventoryItems", x => x.Id));

        migrationBuilder.CreateTable(
            name: "CharacterLootOffers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CharacterId = table.Column<Guid>(type: "uuid", nullable: false),
                ChoicesJson = table.Column<string>(type: "text", nullable: false),
                IsClaimed = table.Column<bool>(type: "boolean", nullable: false)
            }, constraints: table => table.PrimaryKey("PK_CharacterLootOffers", x => x.Id));

        migrationBuilder.Sql("""
            INSERT INTO "CharacterInventoryItems" ("Id", "CharacterId", "SwordId", "ArmorId", "Level", "Quantity")
            SELECT gen_random_uuid(), "Id", "SwordId", NULL, "SwordLevel", 1 FROM "Characters" WHERE "SwordId" IS NOT NULL
            UNION ALL
            SELECT gen_random_uuid(), "Id", NULL, "BootsId", "BootsLevel", 1 FROM "Characters" WHERE "BootsId" IS NOT NULL
            UNION ALL
            SELECT gen_random_uuid(), "Id", NULL, "ChestId", "ChestLevel", 1 FROM "Characters" WHERE "ChestId" IS NOT NULL
            UNION ALL
            SELECT gen_random_uuid(), "Id", NULL, "LegsId", "LegsLevel", 1 FROM "Characters" WHERE "LegsId" IS NOT NULL
            UNION ALL
            SELECT gen_random_uuid(), "Id", NULL, "GlovesId", "GlovesLevel", 1 FROM "Characters" WHERE "GlovesId" IS NOT NULL
            UNION ALL
            SELECT gen_random_uuid(), "Id", NULL, "HelmetId", "HelmetLevel", 1 FROM "Characters" WHERE "HelmetId" IS NOT NULL;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("CharacterInventoryItems");
        migrationBuilder.DropTable("CharacterLootOffers");
        foreach (var levelColumn in new[] { "SwordLevel", "BootsLevel", "ChestLevel", "LegsLevel", "GlovesLevel", "HelmetLevel" })
            migrationBuilder.DropColumn(name: levelColumn, table: "Characters");
    }
}
