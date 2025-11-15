using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanStack.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlueprintStandards_Standards_StandardId",
                table: "BlueprintStandards");

            migrationBuilder.AddColumn<int>(
                name: "Comparison",
                table: "RuleSets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "StandardId",
                table: "BlueprintStandards",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BlueprintId",
                table: "BlueprintStandards",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsValidated",
                table: "Blueprints",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "ComponentId",
                table: "BlueprintComponents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BlueprintId",
                table: "BlueprintComponents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "BlueprintComponents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BuildingStructureId",
                table: "BlueprintBuildingStructures",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BlueprintId",
                table: "BlueprintBuildingStructures",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SquareMeters = table.Column<int>(type: "int", nullable: false),
                    RoomType = table.Column<int>(type: "int", nullable: false),
                    BlueprintId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Blueprints_BlueprintId",
                        column: x => x.BlueprintId,
                        principalTable: "Blueprints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlueprintComponents_RoomId",
                table: "BlueprintComponents",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_BlueprintId",
                table: "Rooms",
                column: "BlueprintId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlueprintComponents_Rooms_RoomId",
                table: "BlueprintComponents",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlueprintStandards_Standards_StandardId",
                table: "BlueprintStandards",
                column: "StandardId",
                principalTable: "Standards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlueprintComponents_Rooms_RoomId",
                table: "BlueprintComponents");

            migrationBuilder.DropForeignKey(
                name: "FK_BlueprintStandards_Standards_StandardId",
                table: "BlueprintStandards");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_BlueprintComponents_RoomId",
                table: "BlueprintComponents");

            migrationBuilder.DropColumn(
                name: "Comparison",
                table: "RuleSets");

            migrationBuilder.DropColumn(
                name: "IsValidated",
                table: "Blueprints");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "BlueprintComponents");

            migrationBuilder.AlterColumn<int>(
                name: "StandardId",
                table: "BlueprintStandards",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BlueprintId",
                table: "BlueprintStandards",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ComponentId",
                table: "BlueprintComponents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BlueprintId",
                table: "BlueprintComponents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BuildingStructureId",
                table: "BlueprintBuildingStructures",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BlueprintId",
                table: "BlueprintBuildingStructures",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_BlueprintStandards_Standards_StandardId",
                table: "BlueprintStandards",
                column: "StandardId",
                principalTable: "Standards",
                principalColumn: "Id");
        }
    }
}
