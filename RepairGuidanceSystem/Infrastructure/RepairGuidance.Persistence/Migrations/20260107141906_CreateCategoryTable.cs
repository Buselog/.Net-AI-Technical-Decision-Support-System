using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RepairGuidance.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Tool");

            migrationBuilder.AddColumn<int>(
                name: "ToolCategoryId",
                table: "Tool",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ToolCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToolCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tool_ToolCategoryId",
                table: "Tool",
                column: "ToolCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tool_ToolCategory_ToolCategoryId",
                table: "Tool",
                column: "ToolCategoryId",
                principalTable: "ToolCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tool_ToolCategory_ToolCategoryId",
                table: "Tool");

            migrationBuilder.DropTable(
                name: "ToolCategory");

            migrationBuilder.DropIndex(
                name: "IX_Tool_ToolCategoryId",
                table: "Tool");

            migrationBuilder.DropColumn(
                name: "ToolCategoryId",
                table: "Tool");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Tool",
                type: "text",
                nullable: true);
        }
    }
}
