CREATE TABLE CharacterWeapons
(
    "CharacterId" INTEGER NOT NULL,
    "WeaponId" INTEGER NOT NULL,
    CONSTRAINT "PK_CharacterWeapons" PRIMARY KEY ("CharacterId", "WeaponId"),
    CONSTRAINT "FK_CharacterWeapons_Characters_CharacterId" FOREIGN KEY ("CharacterId") REFERENCES Characters ("Id"),
    CONSTRAINT "FK_CharacterWeapons_Weapons_WeaponId" FOREIGN KEY ("WeaponId") REFERENCES Weapons ("Id")
);

CREATE INDEX "IX_CharacterWeapons_WeaponId" ON CharacterWeapons ("WeaponId");