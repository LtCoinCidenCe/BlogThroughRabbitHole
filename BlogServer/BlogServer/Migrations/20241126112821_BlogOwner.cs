using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogServer.Migrations
{
    /// <inheritdoc />
    public partial class BlogOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OwnerID",
                table: "Blog",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "Blog");
        }
    }
}
