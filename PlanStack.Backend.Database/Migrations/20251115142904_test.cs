using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanStack.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlueprintComponents_Room_RoomId",
                table: "BlueprintComponents");

            migrationBuilder.DropForeignKey(
                name: "FK_Room_Blueprints_BlueprintId",
                table: "Room");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Room",
                table: "Room");

            migrationBuilder.RenameTable(
                name: "Room",
                newName: "Rooms");

            migrationBuilder.RenameIndex(
                name: "IX_Room_BlueprintId",
                table: "Rooms",
                newName: "IX_Rooms_BlueprintId");

            migrationBuilder.AddColumn<int>(
                name: "RoomType",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlueprintComponents_Rooms_RoomId",
                table: "BlueprintComponents",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Blueprints_BlueprintId",
                table: "Rooms",
                column: "BlueprintId",
                principalTable: "Blueprints",
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
                name: "FK_Rooms_Blueprints_BlueprintId",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomType",
                table: "Rooms");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "Room");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_BlueprintId",
                table: "Room",
                newName: "IX_Room_BlueprintId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Room",
                table: "Room",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlueprintComponents_Room_RoomId",
                table: "BlueprintComponents",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_Blueprints_BlueprintId",
                table: "Room",
                column: "BlueprintId",
                principalTable: "Blueprints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
