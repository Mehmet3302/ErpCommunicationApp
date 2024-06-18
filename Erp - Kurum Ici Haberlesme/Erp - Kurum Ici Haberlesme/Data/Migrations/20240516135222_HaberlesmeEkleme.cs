using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Erp___Kurum_Ici_Haberlesme.Data.Migrations
{
    public partial class HaberlesmeEkleme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Haberlesme",
                columns: table => new
                {
                    TalebId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OncelikDurumu = table.Column<int>(type: "int", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SatinAlmaDurumuu = table.Column<int>(type: "int", nullable: true),
                    FiyatAlmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OdemeMiktari = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OdemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OdemeOnaylandiMi = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Haberlesme", x => x.TalebId);
                    table.ForeignKey(
                        name: "FK_Haberlesme_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Haberlesme_UserId",
                table: "Haberlesme",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Haberlesme");
        }
    }
}
