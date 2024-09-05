using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HONATIMEPIECES.Migrations
{
    /// <inheritdoc />
    public partial class updateProductController : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadImage_Products_ProductId",
                table: "UploadImage");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadImage_Products_ProductId",
                table: "UploadImage",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadImage_Products_ProductId",
                table: "UploadImage");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadImage_Products_ProductId",
                table: "UploadImage",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
