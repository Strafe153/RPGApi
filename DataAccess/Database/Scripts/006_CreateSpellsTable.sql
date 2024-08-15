CREATE TABLE SpellTypes
(
    "Id" SMALLSERIAL NOT NULL,
    "Type" VARCHAR(20) NOT NULL,
    CONSTRAINT "PK_SpellTypes" PRIMARY KEY ("Id")
);

INSERT INTO SpellTypes ("Type")
VALUES
    ('Fire'),
    ('Water'),
    ('Earth'),
    ('Wind'),
    ('Ice'),
    ('Thunder'),
    ('Sand'),
    ('Void'),
    ('Bio'),
    ('Heal'),
    ('Summon'),
    ('Nuke'),
    ('Psyo'),
    ('Bless'),
    ('Curse'),
    ('Almighty');

CREATE TABLE Spells
(
    "Id" SERIAL NOT NULL,
    "Name" VARCHAR(30) NOT NULL,
    "Type" SMALLINT NOT NULL,
    "Damage" SMALLINT NOT NULL,
    CONSTRAINT "PK_Spells" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Spells_SpellTypes_Type" FOREIGN KEY ("Type") REFERENCES SpellTypes ("Id"),
    CONSTRAINT "Damage_Range" CHECK ("Damage" >= 0 AND "Damage" <= 100)
)