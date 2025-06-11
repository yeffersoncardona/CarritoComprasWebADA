using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCarritoComprasAda.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioidCarrito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "CarritoItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "CarritoItems");
        }
    }
}
