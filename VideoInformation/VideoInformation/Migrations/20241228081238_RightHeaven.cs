using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoInformation.Migrations
{
    /// <inheritdoc />
    public partial class RightHeaven : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Authenticated",
                table: "VideoComment",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Authenticated",
                table: "VideoComment");
        }
    }
}
