using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Erp___Kurum_Ici_Haberlesme.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AltBirimId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BirimId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "OnlineDurumu",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PersonelAdSoyad",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilResmi",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TcKimlikNo",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RolAd",
                table: "AspNetRoles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Birim",
                columns: table => new
                {
                    BirimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BirimAd = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Birim", x => x.BirimId);
                });

            migrationBuilder.CreateTable(
                name: "AltBirim",
                columns: table => new
                {
                    AltBirimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AltBirimAdı = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirimId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AltBirim", x => x.AltBirimId);
                    table.ForeignKey(
                        name: "FK_AltBirim_Birim_BirimId",
                        column: x => x.BirimId,
                        principalTable: "Birim",
                        principalColumn: "BirimId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AltBirimId",
                table: "AspNetUsers",
                column: "AltBirimId");

            migrationBuilder.CreateIndex(
                name: "IX_AltBirim_BirimId",
                table: "AltBirim",
                column: "BirimId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AltBirim_AltBirimId",
                table: "AspNetUsers",
                column: "AltBirimId",
                principalTable: "AltBirim",
                principalColumn: "AltBirimId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AltBirim_AltBirimId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "AltBirim");

            migrationBuilder.DropTable(
                name: "Birim");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AltBirimId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AltBirimId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BirimId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OnlineDurumu",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PersonelAdSoyad",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilResmi",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TcKimlikNo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RolAd",
                table: "AspNetRoles");
        }
    }
}
