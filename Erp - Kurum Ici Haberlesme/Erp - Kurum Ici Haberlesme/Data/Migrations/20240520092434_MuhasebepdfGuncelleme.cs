using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Erp___Kurum_Ici_Haberlesme.Data.Migrations
{
    public partial class MuhasebepdfGuncelleme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "OdemeMiktari",
                table: "Haberlesme",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PdfFile",
                table: "Haberlesme",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfFile",
                table: "Haberlesme");

            migrationBuilder.AlterColumn<decimal>(
                name: "OdemeMiktari",
                table: "Haberlesme",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
