using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanStack.Backend.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixedRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b2e8c3e2-8db9-4c4c-b4f2-879b2f41f001",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "User", "USER" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b2e8c3e2-8db9-4c4c-b4f2-879b2f41f001",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Player", "PLAYER" });
        }
    }
}
