using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomersService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamePasswordHashColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Customers",
                newName: "Password");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Customers",
                newName: "PasswordHash");
        }
    }
}
