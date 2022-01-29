using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPGApi.Migrations
{
    public partial class RemoveMountHealth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: new Guid("a288fd81-edf2-407a-ac6b-2b1d4fd1b74d"));

            migrationBuilder.DropColumn(
                name: "Health",
                table: "Mounts");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Weapons",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Spells",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordSalt",
                table: "Players",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordHash",
                table: "Players",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Players",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Mounts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "Id", "Name", "PasswordHash", "PasswordSalt", "Role" },
                values: new object[] { new Guid("535ba88f-eb45-493d-858b-06750deaa1be"), "admin", new byte[] { 226, 132, 33, 154, 209, 4, 73, 58, 41, 18, 203, 244, 215, 52, 148, 91, 213, 248, 88, 192, 76, 107, 244, 179, 21, 207, 39, 223, 130, 247, 238, 140, 0, 99, 92, 185, 118, 43, 223, 50, 231, 68, 173, 202, 253, 225, 65, 150, 153, 65, 76, 243, 158, 233, 54, 4, 60, 114, 166, 7, 220, 51, 255, 125 }, new byte[] { 21, 76, 17, 9, 204, 37, 109, 213, 56, 71, 240, 23, 74, 7, 203, 0, 139, 117, 241, 252, 11, 58, 77, 37, 232, 31, 73, 233, 222, 87, 114, 34, 53, 57, 67, 69, 19, 207, 82, 210, 174, 134, 34, 68, 154, 148, 94, 129, 220, 149, 133, 186, 235, 251, 74, 98, 179, 148, 120, 208, 75, 138, 78, 186, 117, 120, 82, 52, 58, 237, 124, 171, 127, 48, 217, 14, 154, 97, 210, 146, 181, 128, 140, 197, 43, 185, 19, 178, 109, 144, 231, 108, 166, 250, 199, 209, 78, 255, 125, 50, 162, 209, 140, 245, 64, 248, 32, 240, 218, 84, 31, 101, 116, 30, 133, 239, 192, 136, 158, 46, 167, 88, 122, 96, 150, 157, 235, 242 }, 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: new Guid("535ba88f-eb45-493d-858b-06750deaa1be"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Weapons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Spells",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordSalt",
                table: "Players",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordHash",
                table: "Players",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Mounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Health",
                table: "Mounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "Id", "Name", "PasswordHash", "PasswordSalt", "Role" },
                values: new object[] { new Guid("a288fd81-edf2-407a-ac6b-2b1d4fd1b74d"), "admin", new byte[] { 226, 132, 33, 154, 209, 4, 73, 58, 41, 18, 203, 244, 215, 52, 148, 91, 213, 248, 88, 192, 76, 107, 244, 179, 21, 207, 39, 223, 130, 247, 238, 140, 0, 99, 92, 185, 118, 43, 223, 50, 231, 68, 173, 202, 253, 225, 65, 150, 153, 65, 76, 243, 158, 233, 54, 4, 60, 114, 166, 7, 220, 51, 255, 125 }, new byte[] { 21, 76, 17, 9, 204, 37, 109, 213, 56, 71, 240, 23, 74, 7, 203, 0, 139, 117, 241, 252, 11, 58, 77, 37, 232, 31, 73, 233, 222, 87, 114, 34, 53, 57, 67, 69, 19, 207, 82, 210, 174, 134, 34, 68, 154, 148, 94, 129, 220, 149, 133, 186, 235, 251, 74, 98, 179, 148, 120, 208, 75, 138, 78, 186, 117, 120, 82, 52, 58, 237, 124, 171, 127, 48, 217, 14, 154, 97, 210, 146, 181, 128, 140, 197, 43, 185, 19, 178, 109, 144, 231, 108, 166, 250, 199, 209, 78, 255, 125, 50, 162, 209, 140, 245, 64, 248, 32, 240, 218, 84, 31, 101, 116, 30, 133, 239, 192, 136, 158, 46, 167, 88, 122, 96, 150, 157, 235, 242 }, 0 });
        }
    }
}
