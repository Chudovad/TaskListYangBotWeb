using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskListYangBotWeb.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUserWebMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "UsersWeb");

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "UsersWeb",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "UsersWeb");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "UsersWeb",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
