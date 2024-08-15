CREATE TABLE CharacterRaces
(
	"Id" SMALLSERIAL NOT NULL,
	"Race" VARCHAR(20) NOT NULL,
	CONSTRAINT "PK_CharacterRaces" PRIMARY KEY ("Id")
);

INSERT INTO CharacterRaces ("Race")
VALUES
	('Human'),
	('Elf'),
	('Dwarf'),
	('Orc'),
	('Goblin'),
	('Hobbit'),
	('Fairy'),
	('Centaur'),
	('Lizard'),
	('Gnome'),
	('Troll'),
	('Beastman'),
	('Nymph'),
	('Treant'),
	('Elemental'),
	('Vampire'),
	('Werewolf'),
	('Angel'),
	('Demon'),
	('Dragon'),
	('Undead');

CREATE TABLE Characters
(
	"Id" SERIAL NOT NULL,
	"Name" VARCHAR(30) NOT NULL,
	"Race" SMALLINT NOT NULL,
	"Health" SMALLINT NOT NULL,
	"PlayerId" INTEGER NOT NULL,
	CONSTRAINT "PK_Characters" PRIMARY KEY ("Id"),
	CONSTRAINT "FK_Characters_CharacterRaces_Race" FOREIGN KEY ("Race") REFERENCES CharacterRaces ("Id"),
	CONSTRAINT "FK_Characters_Players_PlayerId" FOREIGN KEY ("PlayerId") REFERENCES Players ("Id"),
	CONSTRAINT "Health_Range" CHECK ("Health" >= 0 AND "Health" <= 100)
);

CREATE INDEX "IX_Characters_PlayerId" ON Characters ("PlayerId");