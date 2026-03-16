using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProviderOptimizerService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Providers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Latitud = table.Column<double>(type: "double precision", nullable: false),
                    Longitud = table.Column<double>(type: "double precision", nullable: false),
                    Calificacion = table.Column<double>(type: "double precision", nullable: false),
                    Disponible = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Providers",
                columns: new[] { "Id", "Calificacion", "Disponible", "Latitud", "Longitud" },
                values: new object[,]
                {
                    { "P1", 4.5, true, 4.6500000000000004, -74.049999999999997 },
                    { "P2", 4.7999999999999998, true, 4.6600000000000001, -74.040000000000006 },
                    { "P3", 4.2000000000000002, false, 4.6399999999999997, -74.060000000000002 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Providers");
        }
    }
}
