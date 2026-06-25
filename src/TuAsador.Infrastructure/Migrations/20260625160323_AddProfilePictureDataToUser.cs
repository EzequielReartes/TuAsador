using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TuAsador.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePictureDataToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureContentType",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePictureData",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureContentType",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePictureData",
                table: "AspNetUsers");
        }
    }
}
