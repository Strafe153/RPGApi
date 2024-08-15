CREATE TABLE CharacterMounts
(
    "CharacterId" INTEGER NOT NULL,
    "MountId" INTEGER NOT NULL,
    CONSTRAINT "PK_CharacterMounts" PRIMARY KEY ("CharacterId", "MountId"),
    CONSTRAINT "FK_CharacterMounts_Characters_CharacterId" FOREIGN KEY ("CharacterId") REFERENCES Characters ("Id"),
    CONSTRAINT "FK_CharacterMounts_Mounts_MountId" FOREIGN KEY ("MountId") REFERENCES Mounts ("Id")
);

CREATE INDEX "IX_CharacterMounts_MountId" ON CharacterMounts ("MountId");