using DungeonsAndDragqueens.WebApi.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DungeonsAndDragqueens.Migrations;

[DbContext(typeof(StoreMonstersContext))]
[Migration("20261022150000_AddWeaponsAndArmor")]
public partial class AddWeaponsAndArmor : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(name: "AsciiArt", table: "Swords", type: "text", nullable: false, defaultValue: "");
        foreach (var slot in new[] { "BootsId", "ChestId", "LegsId", "GlovesId", "HelmetId" })
            migrationBuilder.AddColumn<Guid>(name: slot, table: "Characters", type: "uuid", nullable: true);
        migrationBuilder.CreateTable(
            name: "Armors",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Slot = table.Column<string>(type: "text", nullable: false),
                Defense = table.Column<int>(type: "integer", nullable: false),
                SwordDamageBonus = table.Column<int>(type: "integer", nullable: false),
                Description = table.Column<string>(type: "text", nullable: false),
                AsciiArt = table.Column<string>(type: "text", nullable: false)
            }, constraints: table => table.PrimaryKey("PK_Armors", x => x.Id));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Armors");
        migrationBuilder.DropColumn(name: "AsciiArt", table: "Swords");
        foreach (var slot in new[] { "BootsId", "ChestId", "LegsId", "GlovesId", "HelmetId" })
            migrationBuilder.DropColumn(name: slot, table: "Characters");
    }
}
