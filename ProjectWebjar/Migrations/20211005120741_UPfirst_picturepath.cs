using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectWebjar.Migrations
{
    public partial class UPfirst_picturepath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Picture",
                table: "Products",
                newName: "PicturePath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PicturePath",
                table: "Products",
                newName: "Picture");
        }
    }
}
