using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Erp___Kurum_Ici_Haberlesme.Data.Migrations
{
    public partial class MuhasebepdfGuncelleme2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OnaylayanPersonel",
                table: "Haberlesme",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnaylayanPersonel",
                table: "Haberlesme");
        }
    }
}
