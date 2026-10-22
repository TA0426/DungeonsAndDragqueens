using DungeonsAndDragqueens.WebApi.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DungeonsAndDragqueens.Migrations;

[DbContext(typeof(StoreMonstersContext))]
[Migration("20261022140000_AddCharacterClass")]
public partial class AddCharacterClass : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CharacterClass",
            table: "Characters",
            type: "text",
            nullable: false,
            defaultValue: "Warrior");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "CharacterClass", table: "Characters");
    }
}
