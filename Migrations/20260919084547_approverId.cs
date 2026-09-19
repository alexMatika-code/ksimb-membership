using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ksimb_membership.Migrations
{
    /// <inheritdoc />
    public partial class approverId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApproverId",
                table: "Members",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproverId",
                table: "Members");
        }
    }
}
