using Microsoft.EntityFrameworkCore.Migrations;

namespace Shikimori.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    TitleRus = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TitleEng = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Duration = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Url);
                });

            migrationBuilder.CreateTable(
                name: "GenreVideo",
                columns: table => new
                {
                    GenresKey = table.Column<string>(type: "character varying(100)", nullable: false),
                    VideosUrl = table.Column<string>(type: "character varying(1000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreVideo", x => new { x.GenresKey, x.VideosUrl });
                    table.ForeignKey(
                        name: "FK_GenreVideo_Genres_GenresKey",
                        column: x => x.GenresKey,
                        principalTable: "Genres",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreVideo_Videos_VideosUrl",
                        column: x => x.VideosUrl,
                        principalTable: "Videos",
                        principalColumn: "Url",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenreVideo_VideosUrl",
                table: "GenreVideo",
                column: "VideosUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenreVideo");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Videos");
        }
    }
}
