using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OppgaveUkeEnModul3.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToCharacter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Characters",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Characters");
        }
    }
}
