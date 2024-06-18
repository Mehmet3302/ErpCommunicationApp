using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Erp___Kurum_Ici_Haberlesme.Data.Migrations
{
    public partial class ArayuzDuzeltme3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Haberlesme");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Haberlesme",
                columns: table => new
                {
                    TalebId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    FiyatAlmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OdemeMiktari = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OdemeOnaylandiMi = table.Column<bool>(type: "bit", nullable: true),
                    OdemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OnaylayanPersonel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OncelikDurumu = table.Column<int>(type: "int", nullable: false),
                    PdfFile = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    SatinAlmaDurumuu = table.Column<int>(type: "int", nullable: true)
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
    }
}
