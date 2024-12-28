using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace llmChat.Migrations
{
    /// <inheritdoc />
    public partial class chats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "776a1603-1484-44db-8425-6b99baeff4df");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c21f327b-0f6e-4f6e-b9e2-7be8f55f61c0");

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SystemPrompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedByAppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Characters_AspNetUsers_CreatedByAppUserId",
                        column: x => x.CreatedByAppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CharacterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatHistories_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatHistories_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChatHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_ChatHistories_ChatHistoryId",
                        column: x => x.ChatHistoryId,
                        principalTable: "ChatHistories",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "aaac760a-94dd-44ff-a342-f62bc32d3d05", null, "Admin", "ADMIN" },
                    { "d043148d-1054-4bde-b90e-ed57f2c84c53", null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_AppUserId",
                table: "Characters",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CreatedByAppUserId",
                table: "Characters",
                column: "CreatedByAppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Name_CreatedByAppUserId",
                table: "Characters",
                columns: new[] { "Name", "CreatedByAppUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatHistories_AppUserId_CharacterId",
                table: "ChatHistories",
                columns: new[] { "AppUserId", "CharacterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatHistories_CharacterId",
                table: "ChatHistories",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ChatHistoryId",
                table: "Message",
                column: "ChatHistoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "ChatHistories");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "aaac760a-94dd-44ff-a342-f62bc32d3d05");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d043148d-1054-4bde-b90e-ed57f2c84c53");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "776a1603-1484-44db-8425-6b99baeff4df", null, "Admin", "ADMIN" },
                    { "c21f327b-0f6e-4f6e-b9e2-7be8f55f61c0", null, "User", "USER" }
                });
        }
    }
}
