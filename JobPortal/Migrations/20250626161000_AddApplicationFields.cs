using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Applications",
                newName: "ResumeUrl");

            migrationBuilder.RenameColumn(
                name: "Resume",
                table: "Applications",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Applications",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "ResumeUrl",
                table: "Applications",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Applications",
                newName: "Resume");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Applications",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
