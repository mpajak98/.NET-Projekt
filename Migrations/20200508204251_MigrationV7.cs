using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BibliotekaMultimediow.Migrations
{
    public partial class MigrationV7 : Migration
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
                    Rok = table.Column<string>(nullable: true, defaultValue: "nieznany"),
                    WykonawcaId = table.Column<int>(nullable: false, defaultValue: 1)
                        .Annotation("Sqlite:Autoincrement", true),
                    LiczbaUtworowWAlumie = table.Column<int>(nullable: false)
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
                    WykonawcaId = table.Column<int>(nullable: false, defaultValue: 1)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlbumId = table.Column<int>(nullable: false, defaultValue: 1)
                        .Annotation("Sqlite:Autoincrement", true),
                    CzyUlubione = table.Column<bool>(nullable: false, defaultValue: false),
                    DataDodania = table.Column<DateTime>(nullable: false),
                    Rok = table.Column<string>(nullable: true, defaultValue: "nieznany"),
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
                    LiczbaUtworowWykonawcy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wykonawcy", x => x.WykonawcaId);
                });

            migrationBuilder.InsertData(
                table: "Albumy",
                columns: new[] { "AlbumId", "LiczbaUtworowWAlumie", "Nazwa", "Rok", "WykonawcaId" },
                values: new object[] { 1, 0, "nieznany", "nieznany", 1 });

            migrationBuilder.InsertData(
                table: "Wykonawcy",
                columns: new[] { "WykonawcaId", "LiczbaUtworowWykonawcy", "Nazwa" },
                values: new object[] { 1, 0, "nieznany" });
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
