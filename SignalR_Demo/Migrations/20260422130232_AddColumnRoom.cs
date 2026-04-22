using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SignalR_Demo.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Room",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Room",
                table: "ChatMessages");
        }
    }
}
