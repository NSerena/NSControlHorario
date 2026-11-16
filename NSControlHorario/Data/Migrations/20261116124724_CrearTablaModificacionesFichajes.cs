using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSControlHorario.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaModificacionesFichajes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModificacionesFichajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FichajeId = table.Column<int>(type: "int", nullable: false),
                    UsuarioModificacionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FechaModificacionUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaHoraAnteriorUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoAnterior = table.Column<int>(type: "int", nullable: false),
                    FechaHoraNuevaUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoNuevo = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModificacionesFichajes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModificacionesFichajes_AspNetUsers_UsuarioModificacionId",
                        column: x => x.UsuarioModificacionId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ModificacionesFichajes_Fichajes_FichajeId",
                        column: x => x.FichajeId,
                        principalTable: "Fichajes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModificacionesFichajes_FichajeId",
                table: "ModificacionesFichajes",
                column: "FichajeId");

            migrationBuilder.CreateIndex(
                name: "IX_ModificacionesFichajes_UsuarioModificacionId",
                table: "ModificacionesFichajes",
                column: "UsuarioModificacionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModificacionesFichajes");
        }
    }
}
