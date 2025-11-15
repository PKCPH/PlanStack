using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanStack.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class StandardRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RuleSets_Standards_StandardId",
                table: "RuleSets");

            migrationBuilder.DropIndex(
                name: "IX_RuleSets_StandardId",
                table: "RuleSets");

            migrationBuilder.DropColumn(
                name: "StandardId",
                table: "RuleSets");

            migrationBuilder.CreateTable(
                name: "StandardRuleSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleSetId = table.Column<int>(type: "int", nullable: false),
                    StandardId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardRuleSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StandardRuleSet_RuleSets_RuleSetId",
                        column: x => x.RuleSetId,
                        principalTable: "RuleSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StandardRuleSet_Standards_StandardId",
                        column: x => x.StandardId,
                        principalTable: "Standards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StandardRuleSet_RuleSetId",
                table: "StandardRuleSet",
                column: "RuleSetId");

            migrationBuilder.CreateIndex(
                name: "IX_StandardRuleSet_StandardId",
                table: "StandardRuleSet",
                column: "StandardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StandardRuleSet");

            migrationBuilder.AddColumn<int>(
                name: "StandardId",
                table: "RuleSets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RuleSets_StandardId",
                table: "RuleSets",
                column: "StandardId");

            migrationBuilder.AddForeignKey(
                name: "FK_RuleSets_Standards_StandardId",
                table: "RuleSets",
                column: "StandardId",
                principalTable: "Standards",
                principalColumn: "Id");
        }
    }
}
