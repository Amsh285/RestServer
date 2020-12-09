CREATE TABLE "User" (
  "User_ID" SERIAL PRIMARY KEY,
  "UserName" varchar(20) NOT NULL,
  "FirstName" varchar(30),
  "LastName" varchar(30),
  "Email" varchar(30) NOT NULL,
  "Password" bytea NOT NULL,
  "Salt" bytea NOT NULL,
  "HashAlgorithm" varchar(30) NOT NULL,
  "Coins" int NOT NULL
);

CREATE TABLE "Deck" (
  "Deck_ID" SERIAL PRIMARY KEY,
  "User_ID" int NOT NULL,
  "Name" varchar(50) NOT NULL
);

CREATE TABLE "DeckContent" (
  "DeckContent_ID" SERIAL PRIMARY KEY,
  "Deck_ID" int NOT NULL,
  "Card_ID" int NOT NULL,
  "Quantity" int NOT NULL
);

CREATE TABLE "CardLibrary" (
  "CardLibrary_ID" SERIAL PRIMARY KEY,
  "User_ID" int NOT NULL,
  "Card_ID" int NOT NULL,
  "Quantity" int NOT NULL
);

CREATE TABLE "Card" (
  "Card_ID" SERIAL PRIMARY KEY,
  "Element_ID" int NOT NULL,
  "CardType" varchar(9) NOT NULL,
  "Name" varchar(50) NOT NULL,
  "Description" varchar(2000),
  "ImagePath" varchar,
  "AttackPoints" int,
  "HealthPoints" int
);

CREATE TABLE "Element" (
  "Element_ID" SERIAL PRIMARY KEY,
  "Name" varchar(40) NOT NULL
);

CREATE TABLE "CardCategory" (
  "CardCategory_ID" SERIAL PRIMARY KEY,
  "Name" varchar(40) NOT NULL
);

CREATE TABLE "Card_Categories" (
  "Card_Categories_ID" SERIAL PRIMARY KEY,
  "Card_ID" int NOT NULL,
  "CardCategory_ID" int NOT NULL
);

CREATE TABLE "CardPropertyBag" (
  "CardPropertyBag_ID" SERIAL PRIMARY KEY,
  "Card_ID" int NOT NULL,
  "Key" varchar(255) NOT NULL,
  "Value" varchar(255) NOT NULL
);

CREATE TABLE "CardInteraction" (
  "CardInteraction_ID" SERIAL PRIMARY KEY,
  "Card_ID" int NOT NULL,
  "Code" varchar(9) NOT NULL
);

CREATE TABLE "BoosterPack" (
  "BoosterPack_ID" SERIAL PRIMARY KEY,
  "User_ID" int NOT NULL,
  "CardCount" int NOT NULL
);

CREATE TABLE "BoosterPack_Cards" (
  "BoosterPack_Cards_ID" SERIAL PRIMARY KEY,
  "BoosterPack_ID" int NOT NULL,
  "Card_ID" int NOT NULL
);

CREATE TABLE "UserSession" (
  "UserSession_ID" SERIAL PRIMARY KEY,
  "User_ID" int NOT NULL,
  "Token" uuid NOT NULL,
  "CreationDate" timestamp NOT NULL,
  "ExpirationDate" timestamp NOT NULL
);

CREATE TABLE "Game" (
  "Game_ID" SERIAL PRIMARY KEY,
  "GameType" varchar(9) NOT NULL,
  "UserOnTurn_Game_User_ID" int NOT NULL,
  "CreationDate" timestamp NOT NULL,
  "TimeoutDate" timestamp NOT NULL
);

CREATE TABLE "Game_User" (
  "Game_User_ID" SERIAL PRIMARY KEY,
  "Deck_ID" int NOT NULL,
  "User_ID" int NOT NULL,
  "Game_ID" int NOT NULL,
  "HealthPoints" int NOT NULL
);

CREATE TABLE "Playfield_Slots" (
  "Playfield_Slots_ID" SERIAL PRIMARY KEY,
  "Game_User_ID" int NOT NULL,
  "CardInstance_ID" int,
  "SlotNumber" int NOT NULL
);

CREATE TABLE "CardInstance" (
  "CardInstance_ID" SERIAL PRIMARY KEY,
  "Card_ID" int NOT NULL,
  "AttackPoints" int NOT NULL,
  "HealthPoints" int NOT NULL
);

ALTER TABLE "Deck" ADD FOREIGN KEY ("User_ID") REFERENCES "User" ("User_ID");

ALTER TABLE "DeckContent" ADD FOREIGN KEY ("Deck_ID") REFERENCES "Deck" ("Deck_ID");

ALTER TABLE "DeckContent" ADD FOREIGN KEY ("Card_ID") REFERENCES "Card" ("Card_ID");

ALTER TABLE "CardLibrary" ADD FOREIGN KEY ("User_ID") REFERENCES "User" ("User_ID");

ALTER TABLE "CardLibrary" ADD FOREIGN KEY ("Card_ID") REFERENCES "Card" ("Card_ID");

ALTER TABLE "Card" ADD FOREIGN KEY ("Element_ID") REFERENCES "Element" ("Element_ID");

ALTER TABLE "Card_Categories" ADD FOREIGN KEY ("CardCategory_ID") REFERENCES "CardCategory" ("CardCategory_ID");

ALTER TABLE "Card_Categories" ADD FOREIGN KEY ("Card_ID") REFERENCES "Card" ("Card_ID");

ALTER TABLE "CardPropertyBag" ADD FOREIGN KEY ("Card_ID") REFERENCES "Card" ("Card_ID");

ALTER TABLE "CardInteraction" ADD FOREIGN KEY ("Card_ID") REFERENCES "Card" ("Card_ID");

ALTER TABLE "BoosterPack" ADD FOREIGN KEY ("User_ID") REFERENCES "User" ("User_ID");

ALTER TABLE "BoosterPack_Cards" ADD FOREIGN KEY ("BoosterPack_ID") REFERENCES "BoosterPack" ("BoosterPack_ID");

ALTER TABLE "BoosterPack_Cards" ADD FOREIGN KEY ("Card_ID") REFERENCES "Card" ("Card_ID");

ALTER TABLE "UserSession" ADD FOREIGN KEY ("User_ID") REFERENCES "User" ("User_ID");

ALTER TABLE "Game" ADD FOREIGN KEY ("UserOnTurn_Game_User_ID") REFERENCES "Game_User" ("Game_User_ID");

ALTER TABLE "Game_User" ADD FOREIGN KEY ("Deck_ID") REFERENCES "Deck" ("Deck_ID");

ALTER TABLE "Game_User" ADD FOREIGN KEY ("User_ID") REFERENCES "User" ("User_ID");

ALTER TABLE "Game_User" ADD FOREIGN KEY ("Game_ID") REFERENCES "Game" ("Game_ID");

ALTER TABLE "Playfield_Slots" ADD FOREIGN KEY ("Game_User_ID") REFERENCES "Game_User" ("Game_User_ID");

ALTER TABLE "Playfield_Slots" ADD FOREIGN KEY ("CardInstance_ID") REFERENCES "CardInstance" ("CardInstance_ID");

ALTER TABLE "CardInstance" ADD FOREIGN KEY ("Card_ID") REFERENCES "Card" ("Card_ID");
