using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mailing.API.Migrations
{
    public partial class Initial4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BestAnswerId",
                table: "Questions",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BestAnswerId",
                table: "Questions");
        }
    }
}
