CREATE TABLE PlayerRoles
(
    "Id" SMALLSERIAL NOT NULL,
    "Role" VARCHAR (20) NOT NULL,
    CONSTRAINT "PK_PlayerRoles" PRIMARY KEY ("Id")
);

INSERT INTO PlayerRoles ("Role")
VALUES
    ('Admin'),
    ('Player');

CREATE TABLE Players
(
    "Id" SERIAL NOT NULL,
    "Name" VARCHAR(30) NOT NULL,
    "Role" SMALLINT NOT NULL,
    "PasswordHash" BYTEA NOT NULL,
    "PasswordSalt" BYTEA NOT NULL,
    "RefreshToken" VARCHAR(128),
    "RefreshTokenExpiryDate" TIMESTAMP WITHOUT TIME ZONE,
    CONSTRAINT "PK_Players" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Players_PlayerRoles_Role" FOREIGN KEY ("Role") REFERENCES PlayerRoles ("Id")
);

CREATE UNIQUE INDEX "IX_Players_Name" ON Players ("Name");