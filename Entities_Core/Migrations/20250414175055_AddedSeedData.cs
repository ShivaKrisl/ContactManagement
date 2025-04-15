using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entities_Core.Migrations
{
    /// <inheritdoc />
    public partial class AddedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryId", "CountryName" },
                values: new object[,]
                {
                    { new Guid("05f889ab-5977-4a9d-af70-a9aefb7edfb1"), "United Kingdom" },
                    { new Guid("1d9a7593-7346-4dbb-91b3-5b674af4e40e"), "United States" },
                    { new Guid("3a431b58-1288-4949-bc4c-9983943656a7"), "Canada" },
                    { new Guid("9837d12f-07dc-4aad-8791-cf6a7b5f2e1d"), "Mexico" },
                    { new Guid("b3c1562a-08e4-4c81-88ff-96427379cb19"), "Australia" },
                    { new Guid("f1a6e107-9bae-4592-b48b-8cfa8ed12345"), "India" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("05f889ab-5977-4a9d-af70-a9aefb7edfb1"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("1d9a7593-7346-4dbb-91b3-5b674af4e40e"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("3a431b58-1288-4949-bc4c-9983943656a7"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("9837d12f-07dc-4aad-8791-cf6a7b5f2e1d"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("b3c1562a-08e4-4c81-88ff-96427379cb19"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("f1a6e107-9bae-4592-b48b-8cfa8ed12345"));
        }
    }
}
