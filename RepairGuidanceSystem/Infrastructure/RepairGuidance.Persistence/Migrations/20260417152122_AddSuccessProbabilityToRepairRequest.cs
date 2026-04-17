using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepairGuidance.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSuccessProbabilityToRepairRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SuccessProbability",
                table: "RepairRequest",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuccessProbability",
                table: "RepairRequest");
        }
    }
}
