using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendconfigconecta.Migrations
{
    /// <inheritdoc />
    public partial class AddBlockedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Blocked",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Blocked",
                table: "AspNetUsers");
        }
    }
}
