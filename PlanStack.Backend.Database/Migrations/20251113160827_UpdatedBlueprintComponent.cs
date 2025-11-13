using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanStack.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedBlueprintComponent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHorizontal",
                table: "BlueprintComponents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StartingPositionX",
                table: "BlueprintComponents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartingPositionY",
                table: "BlueprintComponents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHorizontal",
                table: "BlueprintComponents");

            migrationBuilder.DropColumn(
                name: "StartingPositionX",
                table: "BlueprintComponents");

            migrationBuilder.DropColumn(
                name: "StartingPositionY",
                table: "BlueprintComponents");
        }
    }
}
