using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanStack.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class StandardRuleSetRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComparisonValue",
                table: "RuleSets");

            migrationBuilder.DropColumn(
                name: "DefinitionValue",
                table: "RuleSets");

            migrationBuilder.AddColumn<int>(
                name: "ComparisonValue",
                table: "StandardRuleSet",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefinitionValue",
                table: "StandardRuleSet",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "StandardRuleSet",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComparisonValue",
                table: "StandardRuleSet");

            migrationBuilder.DropColumn(
                name: "DefinitionValue",
                table: "StandardRuleSet");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "StandardRuleSet");

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
        }
    }
}
