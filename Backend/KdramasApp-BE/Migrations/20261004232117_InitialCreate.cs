using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KdramasApp_BE.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    ActorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.ActorId);
                });

            migrationBuilder.CreateTable(
                name: "Kdramas",
                columns: table => new
                {
                    KdramaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ReleaseYear = table.Column<int>(type: "int", nullable: false),
                    BroadcastingNetwork = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NoOfEpisodes = table.Column<int>(type: "int", nullable: false),
                    PosterUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kdramas", x => x.KdramaId);
                    table.CheckConstraint("CK_NoEps_Positive", "[NoOfEpisodes] > 0");
                    table.CheckConstraint("CK_Valid_ReleaseYear", "[ReleaseYear] >= 1900 AND [ReleaseYear] <= 2050");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "KdramaActors",
                columns: table => new
                {
                    KdramaId = table.Column<int>(type: "int", nullable: false),
                    ActorId = table.Column<int>(type: "int", nullable: false),
                    RoleType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KdramaActors", x => new { x.KdramaId, x.ActorId });
                    table.ForeignKey(
                        name: "FK_KdramaActors_Actors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Actors",
                        principalColumn: "ActorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KdramaActors_Kdramas_KdramaId",
                        column: x => x.KdramaId,
                        principalTable: "Kdramas",
                        principalColumn: "KdramaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserKdramas",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    KdramaId = table.Column<int>(type: "int", nullable: false),
                    WatchPlatform = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WatchedYear = table.Column<int>(type: "int", nullable: false),
                    WatchedMonth = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserKdramas", x => new { x.UserId, x.KdramaId });
                    table.CheckConstraint("CK_Valid_WatchedYear", "[WatchedYear] >= 1900 AND [WatchedYear] <= 2050");
                    table.ForeignKey(
                        name: "FK_UserKdramas_Kdramas_KdramaId",
                        column: x => x.KdramaId,
                        principalTable: "Kdramas",
                        principalColumn: "KdramaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserKdramas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KdramaActors_ActorId",
                table: "KdramaActors",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserKdramas_KdramaId",
                table: "UserKdramas",
                column: "KdramaId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KdramaActors");

            migrationBuilder.DropTable(
                name: "UserKdramas");

            migrationBuilder.DropTable(
                name: "Actors");

            migrationBuilder.DropTable(
                name: "Kdramas");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
