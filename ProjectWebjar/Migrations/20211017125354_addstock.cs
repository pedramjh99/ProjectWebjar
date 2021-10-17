using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectWebjar.Migrations
{
    public partial class addstock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InStock",
                table: "AttributeProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InStock",
                table: "AttributeProducts");
        }
    }
}
