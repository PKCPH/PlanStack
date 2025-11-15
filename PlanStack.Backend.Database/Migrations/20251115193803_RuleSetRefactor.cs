using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanStack.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class RuleSetRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RuleSets_BuildingStructures_BuildingStructureId",
                table: "RuleSets");

            migrationBuilder.DropForeignKey(
                name: "FK_RuleSets_Components_ComponentId",
                table: "RuleSets");

            migrationBuilder.DropForeignKey(
                name: "FK_RuleSets_Standards_StandardId",
                table: "RuleSets");

            migrationBuilder.DropIndex(
                name: "IX_RuleSets_BuildingStructureId",
                table: "RuleSets");

            migrationBuilder.DropIndex(
                name: "IX_RuleSets_ComponentId",
                table: "RuleSets");

            migrationBuilder.DropColumn(
                name: "Ratio",
                table: "RuleSets");

            migrationBuilder.RenameColumn(
                name: "ComponentId",
                table: "RuleSets",
                newName: "ObjectTypeDefinition");

            migrationBuilder.RenameColumn(
                name: "BuildingStructureId",
                table: "RuleSets",
                newName: "ObjectTypeComparison");

            migrationBuilder.AlterColumn<int>(
                name: "StandardId",
                table: "RuleSets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ComparisonValue",
                table: "RuleSets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefinitionValue",
                table: "RuleSets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_RuleSets_Standards_StandardId",
                table: "RuleSets",
                column: "StandardId",
                principalTable: "Standards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RuleSets_Standards_StandardId",
                table: "RuleSets");

            migrationBuilder.DropColumn(
                name: "ComparisonValue",
                table: "RuleSets");

            migrationBuilder.DropColumn(
                name: "DefinitionValue",
                table: "RuleSets");

            migrationBuilder.RenameColumn(
                name: "ObjectTypeDefinition",
                table: "RuleSets",
                newName: "ComponentId");

            migrationBuilder.RenameColumn(
                name: "ObjectTypeComparison",
                table: "RuleSets",
                newName: "BuildingStructureId");

            migrationBuilder.AlterColumn<int>(
                name: "StandardId",
                table: "RuleSets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Ratio",
                table: "RuleSets",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateIndex(
                name: "IX_RuleSets_BuildingStructureId",
                table: "RuleSets",
                column: "BuildingStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleSets_ComponentId",
                table: "RuleSets",
                column: "ComponentId");

            migrationBuilder.AddForeignKey(
                name: "FK_RuleSets_BuildingStructures_BuildingStructureId",
                table: "RuleSets",
                column: "BuildingStructureId",
                principalTable: "BuildingStructures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RuleSets_Components_ComponentId",
                table: "RuleSets",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RuleSets_Standards_StandardId",
                table: "RuleSets",
                column: "StandardId",
                principalTable: "Standards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
