using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mailing.API.Migrations
{
    public partial class Initial5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BestAnswerId",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "EmailNotifyWhenAnswered",
                table: "Users",
                newName: "IsSubscribedForMailing");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSubscribedForMailing",
                table: "Users",
                newName: "EmailNotifyWhenAnswered");

            migrationBuilder.AddColumn<long>(
                name: "BestAnswerId",
                table: "Questions",
                type: "bigint",
                nullable: true);
        }
    }
}
