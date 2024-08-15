CREATE TABLE CharacterSpells
(
    "CharacterId" INTEGER NOT NULL,
    "SpellId" INTEGER NOT NULL,
    CONSTRAINT "PK_CharacterSpells" PRIMARY KEY ("CharacterId", "SpellId"),
    CONSTRAINT "FK_CharacterWeapons_Characters_CharacterId" FOREIGN KEY ("CharacterId") REFERENCES Characters ("Id"),
    CONSTRAINT "FK_CharacterSpells_Spells_SpellId" FOREIGN KEY ("SpellId") REFERENCES Spells ("Id")
);

CREATE INDEX "IX_CharacterSpells_SpellId" ON CharacterSpells ("SpellId");