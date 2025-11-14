using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanStack.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredStandardRulesetRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StandardRuleSets");

            migrationBuilder.AddColumn<int>(
                name: "BuildingStructureId",
                table: "RuleSets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StandardId",
                table: "RuleSets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RuleSets_BuildingStructureId",
                table: "RuleSets",
                column: "BuildingStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleSets_StandardId",
                table: "RuleSets",
                column: "StandardId");

            migrationBuilder.AddForeignKey(
                name: "FK_RuleSets_BuildingStructures_BuildingStructureId",
                table: "RuleSets",
                column: "BuildingStructureId",
                principalTable: "BuildingStructures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RuleSets_Standards_StandardId",
                table: "RuleSets",
                column: "StandardId",
                principalTable: "Standards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RuleSets_BuildingStructures_BuildingStructureId",
                table: "RuleSets");

            migrationBuilder.DropForeignKey(
                name: "FK_RuleSets_Standards_StandardId",
                table: "RuleSets");

            migrationBuilder.DropIndex(
                name: "IX_RuleSets_BuildingStructureId",
                table: "RuleSets");

            migrationBuilder.DropIndex(
                name: "IX_RuleSets_StandardId",
                table: "RuleSets");

            migrationBuilder.DropColumn(
                name: "BuildingStructureId",
                table: "RuleSets");

            migrationBuilder.DropColumn(
                name: "StandardId",
                table: "RuleSets");

            migrationBuilder.CreateTable(
                name: "StandardRuleSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleSetId = table.Column<int>(type: "int", nullable: true),
                    StandardId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardRuleSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StandardRuleSets_RuleSets_RuleSetId",
                        column: x => x.RuleSetId,
                        principalTable: "RuleSets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StandardRuleSets_Standards_StandardId",
                        column: x => x.StandardId,
                        principalTable: "Standards",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StandardRuleSets_RuleSetId",
                table: "StandardRuleSets",
                column: "RuleSetId");

            migrationBuilder.CreateIndex(
                name: "IX_StandardRuleSets_StandardId",
                table: "StandardRuleSets",
                column: "StandardId");
        }
    }
}
