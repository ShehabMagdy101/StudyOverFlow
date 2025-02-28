using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyOverFlow.API.Migrations
{
    /// <inheritdoc />
    public partial class addImageToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImageURL",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageURL",
                table: "AspNetUsers");
        }
    }
}
