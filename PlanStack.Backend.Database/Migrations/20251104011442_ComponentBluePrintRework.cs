using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanStack.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class ComponentBluePrintRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "StandardRuleSets");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "StandardRuleSets");

            migrationBuilder.DropColumn(
                name: "SquareMeters",
                table: "BuildingStructures");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BlueprintStandards");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "BlueprintStandards");

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

            migrationBuilder.RenameColumn(
                name: "positionY",
                table: "BlueprintBuildingStructures",
                newName: "Width");

            migrationBuilder.RenameColumn(
                name: "positionX",
                table: "BlueprintBuildingStructures",
                newName: "Length");

            migrationBuilder.AlterColumn<float>(
                name: "Price",
                table: "Components",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<float>(
                name: "Price",
                table: "BuildingStructures",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<float>(
                name: "TotalPrice",
                table: "BlueprintBuildingStructures",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateTable(
                name: "BSBlueprintPosition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    BlueprintBuildingStructureId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BSBlueprintPosition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BSBlueprintPosition_BlueprintBuildingStructures_BlueprintBuildingStructureId",
                        column: x => x.BlueprintBuildingStructureId,
                        principalTable: "BlueprintBuildingStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CBlueprintPosition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    BlueprintComponentId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CBlueprintPosition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CBlueprintPosition_BlueprintComponents_BlueprintComponentId",
                        column: x => x.BlueprintComponentId,
                        principalTable: "BlueprintComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BSBlueprintPosition_BlueprintBuildingStructureId",
                table: "BSBlueprintPosition",
                column: "BlueprintBuildingStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_CBlueprintPosition_BlueprintComponentId",
                table: "CBlueprintPosition",
                column: "BlueprintComponentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BSBlueprintPosition");

            migrationBuilder.DropTable(
                name: "CBlueprintPosition");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "BlueprintBuildingStructures");

            migrationBuilder.RenameColumn(
                name: "Width",
                table: "BlueprintBuildingStructures",
                newName: "positionY");

            migrationBuilder.RenameColumn(
                name: "Length",
                table: "BlueprintBuildingStructures",
                newName: "positionX");

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

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "Components",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "BuildingStructures",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<int>(
                name: "SquareMeters",
                table: "BuildingStructures",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
    }
}
