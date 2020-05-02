using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BibliotekaMultimediow.Migrations
{
    public partial class MigrationV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Albumy",
                columns: table => new
                {
                    AlbumId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nazwa = table.Column<string>(nullable: true),
                    Rok = table.Column<string>(nullable: true),
                    WykonawcaId = table.Column<int>(nullable: false),
                    CzyUlubione = table.Column<bool>(nullable: false),
                    Ocena = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albumy", x => x.AlbumId);
                });

            migrationBuilder.CreateTable(
                name: "Utwory",
                columns: table => new
                {
                    UtworId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nazwa = table.Column<string>(nullable: true),
                    WykonawcaId = table.Column<int>(nullable: false),
                    AlbumId = table.Column<int>(nullable: false),
                    Ocena = table.Column<int>(nullable: false),
                    CzyUlubione = table.Column<bool>(nullable: false),
                    DataDodania = table.Column<DateTime>(nullable: false),
                    Rok = table.Column<string>(nullable: true),
                    UrlPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utwory", x => x.UtworId);
                });

            migrationBuilder.CreateTable(
                name: "Wykonawcy",
                columns: table => new
                {
                    WykonawcaId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nazwa = table.Column<string>(nullable: true),
                    CzyUlubione = table.Column<bool>(nullable: false),
                    Ocena = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wykonawcy", x => x.WykonawcaId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Albumy");

            migrationBuilder.DropTable(
                name: "Utwory");

            migrationBuilder.DropTable(
                name: "Wykonawcy");
        }
    }
}
