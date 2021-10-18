using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectWebjar.Migrations
{
    public partial class reset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttributeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Attributes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttributeId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Attributes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
