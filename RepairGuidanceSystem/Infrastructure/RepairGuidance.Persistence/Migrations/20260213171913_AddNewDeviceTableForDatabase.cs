using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RepairGuidance.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNewDeviceTableForDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "RepairRequest",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DifficultyScore = table.Column<int>(type: "integer", nullable: false),
                    ToolCategoryId = table.Column<int>(type: "integer", nullable: false),
                    IsPredefined = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Device_ToolCategory_ToolCategoryId",
                        column: x => x.ToolCategoryId,
                        principalTable: "ToolCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RepairRequest_DeviceId",
                table: "RepairRequest",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_ToolCategoryId",
                table: "Device",
                column: "ToolCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RepairRequest_Device_DeviceId",
                table: "RepairRequest",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepairRequest_Device_DeviceId",
                table: "RepairRequest");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropIndex(
                name: "IX_RepairRequest_DeviceId",
                table: "RepairRequest");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "RepairRequest");
        }
    }
}
