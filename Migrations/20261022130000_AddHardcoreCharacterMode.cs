using DungeonsAndDragqueens.WebApi.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DungeonsAndDragqueens.Migrations;

[DbContext(typeof(StoreMonstersContext))]
[Migration("20261022130000_AddHardcoreCharacterMode")]
public partial class AddHardcoreCharacterMode : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsHardcore",
            table: "Characters",
            type: "boolean",
            nullable: false,
            defaultValue: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "IsHardcore", table: "Characters");
    }
}
