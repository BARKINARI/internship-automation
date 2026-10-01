using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjeAndRelationalMesajEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GonderenAd",
                table: "Mesajlar");

            migrationBuilder.DropColumn(
                name: "GonderenRol",
                table: "Mesajlar");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "StajBasvurulari",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StajBasvuruId",
                table: "Mesajlar",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "AliciId",
                table: "Mesajlar",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GonderenId",
                table: "Mesajlar",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjeId",
                table: "Mesajlar",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Projeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Baslik = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    YoneticiId = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projeler_Users_YoneticiId",
                        column: x => x.YoneticiId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "StajyerProjeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjeId = table.Column<int>(type: "integer", nullable: false),
                    StajBasvuruId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    AtanmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TeslimTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    TeslimAciklamasi = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DosyaYolu = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Puan = table.Column<int>(type: "integer", nullable: true),
                    YoneticiNotu = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StajyerProjeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StajyerProjeler_Projeler_ProjeId",
                        column: x => x.ProjeId,
                        principalTable: "Projeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StajyerProjeler_StajBasvurulari_StajBasvuruId",
                        column: x => x.StajBasvuruId,
                        principalTable: "StajBasvurulari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StajyerProjeler_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StajBasvurulari_UserId",
                table: "StajBasvurulari",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_AliciId",
                table: "Mesajlar",
                column: "AliciId");

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_GonderenId",
                table: "Mesajlar",
                column: "GonderenId");

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_ProjeId",
                table: "Mesajlar",
                column: "ProjeId");

            migrationBuilder.CreateIndex(
                name: "IX_Projeler_YoneticiId",
                table: "Projeler",
                column: "YoneticiId");

            migrationBuilder.CreateIndex(
                name: "IX_StajyerProjeler_ProjeId",
                table: "StajyerProjeler",
                column: "ProjeId");

            migrationBuilder.CreateIndex(
                name: "IX_StajyerProjeler_StajBasvuruId",
                table: "StajyerProjeler",
                column: "StajBasvuruId");

            migrationBuilder.CreateIndex(
                name: "IX_StajyerProjeler_UserId",
                table: "StajyerProjeler",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mesajlar_Projeler_ProjeId",
                table: "Mesajlar",
                column: "ProjeId",
                principalTable: "Projeler",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Mesajlar_Users_AliciId",
                table: "Mesajlar",
                column: "AliciId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mesajlar_Users_GonderenId",
                table: "Mesajlar",
                column: "GonderenId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StajBasvurulari_Users_UserId",
                table: "StajBasvurulari",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mesajlar_Projeler_ProjeId",
                table: "Mesajlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Mesajlar_Users_AliciId",
                table: "Mesajlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Mesajlar_Users_GonderenId",
                table: "Mesajlar");

            migrationBuilder.DropForeignKey(
                name: "FK_StajBasvurulari_Users_UserId",
                table: "StajBasvurulari");

            migrationBuilder.DropTable(
                name: "StajyerProjeler");

            migrationBuilder.DropTable(
                name: "Projeler");

            migrationBuilder.DropIndex(
                name: "IX_StajBasvurulari_UserId",
                table: "StajBasvurulari");

            migrationBuilder.DropIndex(
                name: "IX_Mesajlar_AliciId",
                table: "Mesajlar");

            migrationBuilder.DropIndex(
                name: "IX_Mesajlar_GonderenId",
                table: "Mesajlar");

            migrationBuilder.DropIndex(
                name: "IX_Mesajlar_ProjeId",
                table: "Mesajlar");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StajBasvurulari");

            migrationBuilder.DropColumn(
                name: "AliciId",
                table: "Mesajlar");

            migrationBuilder.DropColumn(
                name: "GonderenId",
                table: "Mesajlar");

            migrationBuilder.DropColumn(
                name: "ProjeId",
                table: "Mesajlar");

            migrationBuilder.AlterColumn<int>(
                name: "StajBasvuruId",
                table: "Mesajlar",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GonderenAd",
                table: "Mesajlar",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GonderenRol",
                table: "Mesajlar",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
