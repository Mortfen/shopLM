using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shop.api.Migrations
{
    /// <inheritdoc />
    public partial class InitFIXStamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "ConcurrencyStamp",
                value: "11111111-1111-1111-1111-111111111111");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "ConcurrencyStamp",
                value: "22222222-2222-2222-2222-222222222222");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "ConcurrencyStamp",
                value: "33333333-3333-3333-3333-333333333333");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"),
                column: "ConcurrencyStamp",
                value: "a00bbd1f-c7d1-4581-99a5-c03492e4f260");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"),
                column: "ConcurrencyStamp",
                value: "4318d17b-0f71-4060-af56-9a64d7074a2d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"),
                column: "ConcurrencyStamp",
                value: "95600f02-9587-4f78-9cf7-2516b2cd4c19");
        }
    }
}
