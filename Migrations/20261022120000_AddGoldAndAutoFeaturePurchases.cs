using DungeonsAndDragqueens.WebApi.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DungeonsAndDragqueens.Migrations;

[DbContext(typeof(StoreMonstersContext))]
[Migration("20261022120000_AddGoldAndAutoFeaturePurchases")]
public partial class AddGoldAndAutoFeaturePurchases : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Gold",
            table: "PlayerAccounts",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<bool>(
            name: "HasAutoCamp",
            table: "PlayerAccounts",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "HasAutoFight",
            table: "PlayerAccounts",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Gold", table: "PlayerAccounts");
        migrationBuilder.DropColumn(name: "HasAutoCamp", table: "PlayerAccounts");
        migrationBuilder.DropColumn(name: "HasAutoFight", table: "PlayerAccounts");
    }
}
