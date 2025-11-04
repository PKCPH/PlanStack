using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanStack.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class BlueprintUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "positionX",
                table: "BlueprintComponents");

            migrationBuilder.DropColumn(
                name: "positionY",
                table: "BlueprintComponents");

            migrationBuilder.RenameColumn(
                name: "Length",
                table: "BlueprintBuildingStructures",
                newName: "Height");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Blueprints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "Blueprints",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "Blueprints");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Blueprints");

            migrationBuilder.RenameColumn(
                name: "Height",
                table: "BlueprintBuildingStructures",
                newName: "Length");

            migrationBuilder.AddColumn<int>(
                name: "positionX",
                table: "BlueprintComponents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "positionY",
                table: "BlueprintComponents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
