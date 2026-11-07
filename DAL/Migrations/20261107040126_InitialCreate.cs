using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhongBan",
                columns: table => new
                {
                    MaPB = table.Column<string>(maxLength: 10, nullable: false),
                    TenPB = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongBan", x => x.MaPB);
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    MaNV = table.Column<string>(maxLength: 20, nullable: false),
                    HoTen = table.Column<string>(maxLength: 100, nullable: false),
                    GioiTinh = table.Column<bool>(nullable: false),
                    NgaySinh = table.Column<DateTime>(nullable: false),
                    DiaChi = table.Column<string>(maxLength: 200, nullable: true),
                    Luong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaPB = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.MaNV);
                    table.ForeignKey(
                        name: "FK_NhanVien_PhongBan_MaPB",
                        column: x => x.MaPB,
                        principalTable: "PhongBan",
                        principalColumn: "MaPB",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_MaPB",
                table: "NhanVien",
                column: "MaPB");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "PhongBan");
        }
    }
}
