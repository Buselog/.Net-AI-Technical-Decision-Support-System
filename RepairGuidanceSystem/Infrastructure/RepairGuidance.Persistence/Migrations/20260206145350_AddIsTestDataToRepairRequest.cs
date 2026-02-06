using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepairGuidance.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsTestDataToRepairRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTestData",
                table: "RepairRequest",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTestData",
                table: "RepairRequest");
        }
    }
}
