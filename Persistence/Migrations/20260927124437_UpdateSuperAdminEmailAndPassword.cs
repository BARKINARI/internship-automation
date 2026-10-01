using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSuperAdminEmailAndPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "OperationClaims",
                columns: new[] { "Id", "CreatedDate", "DeletedDate", "Name", "UpdatedDate" },
                values: new object[,]
                {
                    { 22, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ProjeYoneticisi", null },
                    { 23, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Stajyer", null },
                    { 24, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "StajSorumlusu", null },
                    { 25, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Egitmen", null },
                    { 26, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Staj.Basvurular", null },
                    { 27, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Staj.YeniBasvuru", null },
                    { 28, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Proje.Yonetimi", null },
                    { 29, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Proje.TakipNotlandirma", null },
                    { 30, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Iletisim.OgrenciMesajlari", null }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "PasswordHash", "PasswordSalt" },
                values: new object[] { "admin@ae.com", new byte[] { 124, 120, 138, 153, 202, 187, 53, 228, 67, 165, 14, 28, 10, 108, 164, 239, 44, 66, 172, 255, 224, 141, 69, 35, 83, 52, 87, 61, 56, 135, 38, 27, 127, 107, 84, 212, 212, 203, 67, 76, 9, 117, 163, 105, 193, 66, 6, 235, 149, 9, 109, 29, 227, 203, 3, 62, 74, 157, 124, 93, 52, 138, 211, 49 }, new byte[] { 4, 184, 206, 205, 237, 196, 95, 218, 122, 40, 188, 148, 212, 242, 30, 31, 149, 83, 45, 201, 104, 52, 45, 207, 235, 170, 209, 43, 103, 1, 193, 51, 52, 129, 106, 104, 203, 170, 142, 26, 209, 7, 81, 26, 133, 133, 136, 153, 138, 164, 12, 133, 211, 16, 205, 182, 63, 150, 112, 247, 241, 43, 30, 68, 26, 19, 220, 232, 233, 129, 77, 32, 250, 244, 56, 122, 79, 201, 239, 114, 77, 115, 220, 221, 150, 146, 22, 56, 47, 142, 65, 17, 7, 136, 22, 223, 41, 66, 195, 33, 206, 10, 140, 181, 60, 24, 240, 11, 230, 146, 43, 189, 212, 50, 202, 153, 84, 123, 245, 127, 57, 246, 21, 124, 50, 23, 235, 189 } });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OperationClaims",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "OperationClaims",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "OperationClaims",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "OperationClaims",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "OperationClaims",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "OperationClaims",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "OperationClaims",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "OperationClaims",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "OperationClaims",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "PasswordHash", "PasswordSalt" },
                values: new object[] { "admin@staj.com", new byte[] { 111, 77, 43, 9, 234, 87, 104, 46, 16, 129, 163, 125, 191, 92, 99, 115, 177, 3, 101, 144, 155, 254, 23, 122, 172, 59, 157, 104, 95, 83, 52, 158, 33, 189, 55, 83, 180, 96, 111, 50, 129, 59, 205, 180, 120, 41, 24, 16, 100, 189, 84, 113, 47, 161, 233, 228, 50, 169, 67, 36, 82, 65, 194, 20 }, new byte[] { 195, 238, 182, 252, 241, 235, 38, 47, 57, 8, 3, 70, 160, 140, 120, 7, 173, 225, 237, 150, 128, 207, 222, 143, 0, 190, 235, 143, 59, 208, 139, 66, 20, 232, 191, 208, 207, 190, 182, 169, 11, 30, 57, 109, 203, 6, 196, 86, 144, 48, 93, 20, 67, 39, 88, 58, 88, 153, 150, 164, 150, 184, 99, 244, 180, 23, 115, 128, 183, 91, 219, 216, 219, 244, 102, 21, 2, 126, 32, 6, 24, 120, 243, 48, 118, 119, 24, 5, 96, 182, 120, 59, 206, 52, 71, 144, 97, 73, 237, 8, 22, 244, 128, 38, 250, 40, 247, 23, 52, 169, 125, 76, 176, 132, 143, 18, 40, 194, 219, 7, 105, 228, 52, 215, 135, 82, 224, 161 } });
        }
    }
}
