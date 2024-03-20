using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreatorsPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddSubtitleIDToCommissionNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventURL",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<string>(
                name: "Banner",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageURL",
                table: "EventImages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "EventImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageTitle",
                table: "EventImages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_SubtitleId",
                table: "Commissions",
                column: "SubtitleId");

            migrationBuilder.AddForeignKey(
                name: "FK__Commissio__Subti__4D5F7D71",
                table: "Commissions",
                column: "SubtitleId",
                principalTable: "Subtitles",
                principalColumn: "SubtitleID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Commissio__Subti__4D5F7D71",
                table: "Commissions");

            migrationBuilder.DropIndex(
                name: "IX_Commissions_SubtitleId",
                table: "Commissions");

            migrationBuilder.DropColumn(
                name: "Banner",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "EventImages");

            migrationBuilder.DropColumn(
                name: "ImageTitle",
                table: "EventImages");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<byte[]>(
                name: "EventURL",
                table: "Events",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "ImageURL",
                table: "EventImages",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
