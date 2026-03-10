using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CarRental.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeedAllDataFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rentals",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rentals",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RolesId", "UsersId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RolesId", "UsersId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RolesId", "UsersId" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RolesId", "UsersId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "Brand", "DailyPrice", "DistanceKm", "LicensePlate", "Model", "Status" },
                values: new object[,]
                {
                    { 1, "Toyota", 15000, 110000, "AA-BB-123", "Corolla", 0 },
                    { 2, "Tesla", 45000, 23000, "EL-ON-420", "Model 3", 0 },
                    { 3, "Ford", 8000, 180000, "SIX-767", "Mondeo", 2 }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "RoleType" },
                values: new object[,]
                {
                    { 1, 2 },
                    { 2, 1 },
                    { 3, 0 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "Email", "FirstName", "LastName", "Password", "Phone", "UserName" },
                values: new object[,]
                {
                    { 1, "Admin Street 1", "admin@admin.hu", "Admin", "Admin", "admin123", "+36206766767", "admin" },
                    { 2, "Haszkovó u. 67", "vadjanos67@freemail.hu", "Vad", "János", "viccesjelszo", "+36306766767", "VadonJani67" },
                    { 3, "Fast Street 1", "acsaladazelso@csalad.com", "Dom", "Toretto", "csalad4ever", "+3612345678", "DomToretto" },
                    { 4, "Vágó utca 13", "johnp13@gmail.com", "Sertés", "János", "tehenhus42", "+36706766767", "JohnPork13" }
                });

            migrationBuilder.InsertData(
                table: "Rentals",
                columns: new[] { "Id", "ApprovedByUserId", "CarId", "ClosedAt", "EndDate", "GuestEmail", "GuestName", "GuestPhone", "HandedOverAt", "StartDate", "Status", "UserId" },
                values: new object[,]
                {
                    { 1, null, 1, null, new DateTime(2024, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 2 },
                    { 2, null, 3, null, new DateTime(2024, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, new DateTime(2024, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 4 }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RolesId", "UsersId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 2, 4 },
                    { 3, 3 }
                });
        }
    }
}
