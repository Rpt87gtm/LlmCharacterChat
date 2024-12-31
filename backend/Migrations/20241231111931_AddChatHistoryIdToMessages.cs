using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace llmChat.Migrations
{
    /// <inheritdoc />
    public partial class AddChatHistoryIdToMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_ChatHistories_ChatHistoryId",
                table: "Message");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Message",
                table: "Message");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "aaac760a-94dd-44ff-a342-f62bc32d3d05");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d043148d-1054-4bde-b90e-ed57f2c84c53");

            migrationBuilder.RenameTable(
                name: "Message",
                newName: "Messages");

            migrationBuilder.RenameIndex(
                name: "IX_Message_ChatHistoryId",
                table: "Messages",
                newName: "IX_Messages_ChatHistoryId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChatHistoryId",
                table: "Messages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "Id");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "e1f584b9-1eeb-400f-8a37-53ee795ab76d", null, "Admin", "ADMIN" },
                    { "e8f7e206-89ff-4bb6-b63d-7b9c27b1d907", null, "User", "USER" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatHistories_ChatHistoryId",
                table: "Messages",
                column: "ChatHistoryId",
                principalTable: "ChatHistories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatHistories_ChatHistoryId",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e1f584b9-1eeb-400f-8a37-53ee795ab76d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e8f7e206-89ff-4bb6-b63d-7b9c27b1d907");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "Message");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ChatHistoryId",
                table: "Message",
                newName: "IX_Message_ChatHistoryId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChatHistoryId",
                table: "Message",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Message",
                table: "Message",
                column: "Id");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "aaac760a-94dd-44ff-a342-f62bc32d3d05", null, "Admin", "ADMIN" },
                    { "d043148d-1054-4bde-b90e-ed57f2c84c53", null, "User", "USER" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Message_ChatHistories_ChatHistoryId",
                table: "Message",
                column: "ChatHistoryId",
                principalTable: "ChatHistories",
                principalColumn: "Id");
        }
    }
}
