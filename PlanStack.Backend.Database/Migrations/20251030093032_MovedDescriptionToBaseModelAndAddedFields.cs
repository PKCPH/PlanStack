using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanStack.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class MovedDescriptionToBaseModelAndAddedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "StandardRuleSets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "StandardRuleSets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Components",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Components",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImgPath",
                table: "Components",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "BuildingStructures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BuildingStructures",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BlueprintStandards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BlueprintStandards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Blueprints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BlueprintComponents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BlueprintComponents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BlueprintBuildingStructures",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BlueprintBuildingStructures",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "StandardRuleSets");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "StandardRuleSets");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "ImgPath",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "BuildingStructures");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BuildingStructures");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BlueprintStandards");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "BlueprintStandards");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Blueprints");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BlueprintComponents");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "BlueprintComponents");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BlueprintBuildingStructures");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "BlueprintBuildingStructures");
        }
    }
}
