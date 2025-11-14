using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanStack.Backend.Database.Migrations
{
    public partial class ProjectIdToGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CBlueprintPosition");

            migrationBuilder.DropForeignKey(
                name: "FK_Blueprints_Projects_ProjectId",
                table: "Blueprints");

            migrationBuilder.DropIndex(
                name: "IX_Blueprints_ProjectId",
                table: "Blueprints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Projects",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Projects");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Projects",
                table: "Projects",
                column: "Id");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Blueprints");

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Blueprints",
                type: "uniqueidentifier",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Blueprints_ProjectId",
                table: "Blueprints",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blueprints_Projects_ProjectId",
                table: "Blueprints",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blueprints_Projects_ProjectId",
                table: "Blueprints");

            migrationBuilder.DropIndex(
                name: "IX_Blueprints_ProjectId",
                table: "Blueprints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Projects",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Blueprints");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Projects",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Projects",
                table: "Projects",
                column: "Id");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Blueprints",
                type: "int",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Blueprints_ProjectId",
                table: "Blueprints",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blueprints_Projects_ProjectId",
                table: "Blueprints",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateTable(
                name: "CBlueprintPosition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlueprintComponentId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_CBlueprintPosition_BlueprintComponentId",
                table: "CBlueprintPosition",
                column: "BlueprintComponentId");
        }
    }
}