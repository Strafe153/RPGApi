using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Speed = table.Column<int>(type: "int", nullable: false, defaultValue: 10)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Damage = table.Column<int>(type: "int", nullable: false, defaultValue: 15)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spells", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Weapons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Damage = table.Column<int>(type: "int", nullable: false, defaultValue: 30)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weapons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Race = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Health = table.Column<int>(type: "int", nullable: false, defaultValue: 100),
                    PlayerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterMounts",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    MountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterMounts", x => new { x.CharacterId, x.MountId });
                    table.ForeignKey(
                        name: "FK_CharacterMounts_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterMounts_Mounts_MountId",
                        column: x => x.MountId,
                        principalTable: "Mounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterSpells",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    SpellId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSpells", x => new { x.CharacterId, x.SpellId });
                    table.ForeignKey(
                        name: "FK_CharacterSpells_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterSpells_Spells_SpellId",
                        column: x => x.SpellId,
                        principalTable: "Spells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterWeapons",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    WeaponId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterWeapons", x => new { x.CharacterId, x.WeaponId });
                    table.ForeignKey(
                        name: "FK_CharacterWeapons_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterWeapons_Weapons_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "Weapons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "Id", "Name", "Role", "PasswordHash", "PasswordSalt" },
                values: new object[] { 1, "Admin", 0, new byte[] { 85, 231, 233, 62, 74, 123, 216, 86, 236, 143, 206, 171, 38, 166, 172, 188, 140, 4, 18, 183, 101, 135, 207, 17, 121, 243, 150, 75, 100, 91, 103, 147, 46, 13, 72, 170, 115, 104, 182, 74, 122, 3, 73, 98, 137, 41, 10, 143, 166, 201, 156, 20, 164, 246, 119, 91, 126, 6, 163, 185, 51, 70, 252, 95 }, new byte[] { 213, 144, 108, 126, 63, 226, 53, 28, 69, 97, 132, 35, 230, 149, 184, 102, 185, 103, 146, 148, 35, 216, 144, 10, 206, 153, 168, 152, 11, 157, 102, 157, 46, 239, 139, 62, 182, 80, 13, 152, 130, 248, 43, 25, 131, 103, 86, 186, 27, 96, 194, 44, 5, 219, 158, 21, 115, 135, 190, 116, 234, 146, 99, 84, 250, 185, 69, 178, 37, 161, 159, 146, 127, 10, 118, 63, 74, 167, 106, 193, 203, 174, 145, 49, 208, 100, 141, 185, 23, 6, 236, 216, 252, 12, 112, 192, 40, 58, 74, 222, 21, 236, 14, 135, 212, 242, 35, 251, 143, 55, 86, 117, 200, 206, 182, 217, 245, 9, 54, 60, 2, 133, 18, 33, 109, 18, 154, 249 } });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterMounts_MountId",
                table: "CharacterMounts",
                column: "MountId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_PlayerId",
                table: "Characters",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSpells_SpellId",
                table: "CharacterSpells",
                column: "SpellId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterWeapons_WeaponId",
                table: "CharacterWeapons",
                column: "WeaponId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterMounts");

            migrationBuilder.DropTable(
                name: "CharacterSpells");

            migrationBuilder.DropTable(
                name: "CharacterWeapons");

            migrationBuilder.DropTable(
                name: "Mounts");

            migrationBuilder.DropTable(
                name: "Spells");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Weapons");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
