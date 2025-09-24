using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_default_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "Created", "Deleted", "Email", "FirstName", "LastName", "PasswordHash", "PhoneNumber", "RestaurantId", "Role" },
                values: new object[] { 1, "", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "manager@vasutetterem.hu", "John", "Doe", "$2a$11$VoJtOj.3CALx84THrqdpruADp9ldqh16.RjH/pkbbOtdHOCk1N4gG", "0662123456", 1, "Restaurant" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
