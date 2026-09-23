using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ksimb_membership.Migrations
{
    /// <inheritdoc />
    public partial class MemberCardNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MemberCardNumber",
                table: "Members",
                type: "integer",
                nullable: true)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Members_MemberCardNumber",
                table: "Members",
                column: "MemberCardNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_MemberCardNumber",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "MemberCardNumber",
                table: "Members");
        }
    }
}
