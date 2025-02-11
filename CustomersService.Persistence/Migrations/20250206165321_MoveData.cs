using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomersService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"Customers\" SET \"PasswordHash\" = \"Password\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
