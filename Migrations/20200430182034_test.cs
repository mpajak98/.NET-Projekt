using Microsoft.EntityFrameworkCore.Migrations;

namespace BibliotekaMultimediow.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rok",
                table: "Albumy",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rok",
                table: "Albumy");
        }
    }
}
