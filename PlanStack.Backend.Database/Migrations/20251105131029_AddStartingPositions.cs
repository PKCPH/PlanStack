using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanStack.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddStartingPositions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BSBlueprintPosition");

            migrationBuilder.AddColumn<int>(
                name: "StartingPositionX",
                table: "BlueprintBuildingStructures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartingPositionY",
                table: "BlueprintBuildingStructures",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartingPositionX",
                table: "BlueprintBuildingStructures");

            migrationBuilder.DropColumn(
                name: "StartingPositionY",
                table: "BlueprintBuildingStructures");

            migrationBuilder.CreateTable(
                name: "BSBlueprintPosition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlueprintBuildingStructureId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_BSBlueprintPosition_BlueprintBuildingStructureId",
                table: "BSBlueprintPosition",
                column: "BlueprintBuildingStructureId");
        }
    }
}
