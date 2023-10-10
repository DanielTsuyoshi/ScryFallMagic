using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScryFallMagic.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Colecao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DataLancamento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Idioma = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colecao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cartas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    Texto = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    Raridade = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Idioma = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CustoMana = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    DataLancamento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    ColecaoId = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cartas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cartas_Colecao_ColecaoId",
                        column: x => x.ColecaoId,
                        principalTable: "Colecao",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Versoes",
                columns: table => new
                {
                    VersaoId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Sigla = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DataLancamento = table.Column<DateTime>(type: "TIMESTAMP(7)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Versoes", x => x.VersaoId);
                });
            
            migrationBuilder.CreateIndex(
                name: "IX_Cartas_ColecaoId",
                table: "Cartas",
                column: "ColecaoId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "Cartas");

            migrationBuilder.DropTable(
                name: "Colecao");

            migrationBuilder.DropTable(
                name: "Versoes");
        }
    }
}