CREATE TABLE MountTypes
(
    "Id" SMALLSERIAL NOT NULL,
    "Type" VARCHAR(20) NOT NULL,
    CONSTRAINT "PK_MountTypes" PRIMARY KEY ("Id")
);

INSERT INTO MountTypes ("Type")
VALUES
    ('Horse'),
    ('Donkey'),
    ('Camel'),
    ('Wolf'),
    ('Hog'),
    ('Bear'),
    ('Rhino'),
    ('Hippo'),
    ('Elephant'),
    ('Goat'),
    ('Wisent');

CREATE TABLE Mounts
(
    "Id" SERIAL NOT NULL,
    "Name" VARCHAR(30) NOT NULL,
    "Type" SMALLINT NOT NULL,
    "Speed" SMALLINT NOT NULL,
    CONSTRAINT "PK_Mounts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Mounts_MountTypes_Type" FOREIGN KEY ("Type") REFERENCES MountTypes ("Id"),
    CONSTRAINT "Speed_Range" CHECK ("Speed" >= 0 AND "Speed" <= 100)
);