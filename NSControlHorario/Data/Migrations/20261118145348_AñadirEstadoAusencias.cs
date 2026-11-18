using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSControlHorario.Migrations
{
    /// <inheritdoc />
    public partial class AñadirEstadoAusencias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Ausencias",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaResolucionUtc",
                table: "Ausencias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoResolucion",
                table: "Ausencias",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioResolucionId",
                table: "Ausencias",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ausencias_UsuarioResolucionId",
                table: "Ausencias",
                column: "UsuarioResolucionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ausencias_AspNetUsers_UsuarioResolucionId",
                table: "Ausencias",
                column: "UsuarioResolucionId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ausencias_AspNetUsers_UsuarioResolucionId",
                table: "Ausencias");

            migrationBuilder.DropIndex(
                name: "IX_Ausencias_UsuarioResolucionId",
                table: "Ausencias");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Ausencias");

            migrationBuilder.DropColumn(
                name: "FechaResolucionUtc",
                table: "Ausencias");

            migrationBuilder.DropColumn(
                name: "MotivoResolucion",
                table: "Ausencias");

            migrationBuilder.DropColumn(
                name: "UsuarioResolucionId",
                table: "Ausencias");
        }
    }
}
