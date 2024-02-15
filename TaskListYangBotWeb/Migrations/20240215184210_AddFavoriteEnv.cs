using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskListYangBotWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteEnv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoriteEnvironments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnvironmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PoolId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteEnvironments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteEnvironments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteEnvironments_UserId",
                table: "FavoriteEnvironments",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteEnvironments");
        }
    }
}
