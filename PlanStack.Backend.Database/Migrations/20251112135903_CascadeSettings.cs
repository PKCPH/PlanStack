using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanStack.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class CascadeSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlueprintBuildingStructures_Blueprints_BlueprintId",
                table: "BlueprintBuildingStructures");

            migrationBuilder.DropForeignKey(
                name: "FK_BlueprintBuildingStructures_BuildingStructures_BuildingStructureId",
                table: "BlueprintBuildingStructures");

            migrationBuilder.DropForeignKey(
                name: "FK_BlueprintComponents_Blueprints_BlueprintId",
                table: "BlueprintComponents");

            migrationBuilder.DropForeignKey(
                name: "FK_BlueprintComponents_Components_ComponentId",
                table: "BlueprintComponents");

            migrationBuilder.DropForeignKey(
                name: "FK_BlueprintStandards_Blueprints_BlueprintId",
                table: "BlueprintStandards");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Components",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "Components",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_BlueprintBuildingStructures_Blueprints_BlueprintId",
                table: "BlueprintBuildingStructures",
                column: "BlueprintId",
                principalTable: "Blueprints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlueprintBuildingStructures_BuildingStructures_BuildingStructureId",
                table: "BlueprintBuildingStructures",
                column: "BuildingStructureId",
                principalTable: "BuildingStructures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlueprintComponents_Blueprints_BlueprintId",
                table: "BlueprintComponents",
                column: "BlueprintId",
                principalTable: "Blueprints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlueprintComponents_Components_ComponentId",
                table: "BlueprintComponents",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlueprintStandards_Blueprints_BlueprintId",
                table: "BlueprintStandards",
                column: "BlueprintId",
                principalTable: "Blueprints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlueprintBuildingStructures_Blueprints_BlueprintId",
                table: "BlueprintBuildingStructures");

            migrationBuilder.DropForeignKey(
                name: "FK_BlueprintBuildingStructures_BuildingStructures_BuildingStructureId",
                table: "BlueprintBuildingStructures");

            migrationBuilder.DropForeignKey(
                name: "FK_BlueprintComponents_Blueprints_BlueprintId",
                table: "BlueprintComponents");

            migrationBuilder.DropForeignKey(
                name: "FK_BlueprintComponents_Components_ComponentId",
                table: "BlueprintComponents");

            migrationBuilder.DropForeignKey(
                name: "FK_BlueprintStandards_Blueprints_BlueprintId",
                table: "BlueprintStandards");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Components");

            migrationBuilder.AddForeignKey(
                name: "FK_BlueprintBuildingStructures_Blueprints_BlueprintId",
                table: "BlueprintBuildingStructures",
                column: "BlueprintId",
                principalTable: "Blueprints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlueprintBuildingStructures_BuildingStructures_BuildingStructureId",
                table: "BlueprintBuildingStructures",
                column: "BuildingStructureId",
                principalTable: "BuildingStructures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlueprintComponents_Blueprints_BlueprintId",
                table: "BlueprintComponents",
                column: "BlueprintId",
                principalTable: "Blueprints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlueprintComponents_Components_ComponentId",
                table: "BlueprintComponents",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlueprintStandards_Blueprints_BlueprintId",
                table: "BlueprintStandards",
                column: "BlueprintId",
                principalTable: "Blueprints",
                principalColumn: "Id");
        }
    }
}
