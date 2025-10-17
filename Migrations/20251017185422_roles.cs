using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TareasMVC.Migrations
{
    /// <inheritdoc />
    public partial class roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        if not exists (Select Id from AspNetRoles where Id = '2ef50cfa-593a-43ac-bcab-18e45234dd64')
            begin
                INSERT INTO AspNetRoles(Id, Name,NormalizedName)
                Values ('2ef50cfa-593a-43ac-bcab-18e45234dd64', 'admin', 'ADMIN')
            end
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Delete AspNetRoles where Id = '2ef50cfa-593a-43ac-bcab-18e45234dd64'");
        }
    }
}
