CREATE TABLE WeaponTypes
(
	"Id" SMALLSERIAL NOT NULL,
	"Type" VARCHAR(20) NOT NULL,
	CONSTRAINT "PK_WeaponTypes" PRIMARY KEY ("Id")
);

INSERT INTO WeaponTypes ("Type")
VALUES
	('Sword'),
	('Knife'),
	('Dagger'),
	('Axe'),
	('Scythe'),
	('Machete'),
	('Halberd'),
	('Naginata'),
	('Trident'),
	('Pike'),
	('Kama'),
	('Spear'),
	('Bow'),
	('Crossbow'),
	('Staff'),
	('Wand');

CREATE TABLE Weapons
(
	"Id" SERIAL NOT NULL,
	"Name" VARCHAR(30) NOT NULL,
	"Type" SMALLINT NOT NULL,
	"Damage" SMALLINT NOT NULL,
	CONSTRAINT "PK_Weapons" PRIMARY KEY ("Id"),
	CONSTRAINT "FK_Weapons_WeaponTypes_Type" FOREIGN KEY ("Type") REFERENCES WeaponTypes ("Id"),
	CONSTRAINT "Damage_Range" CHECK ("Damage" >= 0 AND "Damage" <= 100)
);