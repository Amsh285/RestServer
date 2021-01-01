CREATE TABLE "User" (
  "User_ID" SERIAL PRIMARY KEY,
  "UserName" varchar(20) NOT NULL,
  "FirstName" varchar(30),
  "LastName" varchar(30),
  "Email" varchar(30) NOT NULL,
  "Password" bytea NOT NULL,
  "Salt" bytea NOT NULL,
  "HashAlgorithm" varchar(30) NOT NULL,
  "Coins" int NOT NULL,
  "Rating" int NOT NULL,
  "GamesPlayed" int NOT NULL,
  "Winrate" int NOT NULL
);

CREATE TABLE "Deck" (
  "Deck_ID" SERIAL PRIMARY KEY,
  "User_ID" int NOT NULL,
  "Name" varchar(50) NOT NULL
);

CREATE TABLE "Deck_Cards" (
  "Deck_Cards_ID" SERIAL PRIMARY KEY,
  "Deck_ID" int NOT NULL,
  "Card_ID" int NOT NULL
);

CREATE TABLE "CardLibrary" (
  "CardLibrary_ID" SERIAL PRIMARY KEY,
  "User_ID" int NOT NULL,
  "Card_ID" int NOT NULL,
  "Quantity" int NOT NULL
);

CREATE TABLE "Card" (
  "Card_ID" SERIAL PRIMARY KEY,
  "ElementType" varchar(20) NOT NULL,
  "CardType" varchar(20) NOT NULL,
  "Name" varchar(50) NOT NULL,
  "Description" varchar(2000),
  "AttackPoints" int
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

CREATE TABLE "BattleLog" (
  "BattleLog_ID" SERIAL PRIMARY KEY,
  "Match_ID" uuid NOT NULL,
  "Deck_ID_1" int NOT NULL,
  "Deck_ID_2" int NOT NULL,
  "User_ID_Winner" int,
  "BattleResult" varchar(100),
  "Turns" int NOT NULL,
  "CreationDate" timestamp NOT NULL
);

CREATE TABLE "BattleLogEntry" (
  "BattleLogEntry_ID" SERIAL PRIMARY KEY,
  "BattleLog_ID" int NOT NULL,
  "Card_ID_1" int NOT NULL,
  "Card_ID_2" int NOT NULL,
  "DeckState_1" varchar(3000),
  "DeckState_2" varchar(3000),
  "RoundDescription" varchar(3000),
  "Order" int NOT NULL
);

ALTER TABLE "Deck" ADD FOREIGN KEY ("User_ID") REFERENCES "User" ("User_ID");

ALTER TABLE "Deck_Cards" ADD FOREIGN KEY ("Deck_ID") REFERENCES "Deck" ("Deck_ID");

ALTER TABLE "Deck_Cards" ADD FOREIGN KEY ("Card_ID") REFERENCES "Card" ("Card_ID");

ALTER TABLE "CardLibrary" ADD FOREIGN KEY ("User_ID") REFERENCES "User" ("User_ID");

ALTER TABLE "CardLibrary" ADD FOREIGN KEY ("Card_ID") REFERENCES "Card" ("Card_ID");

ALTER TABLE "BoosterPack" ADD FOREIGN KEY ("User_ID") REFERENCES "User" ("User_ID");

ALTER TABLE "BoosterPack_Cards" ADD FOREIGN KEY ("BoosterPack_ID") REFERENCES "BoosterPack" ("BoosterPack_ID");

ALTER TABLE "BoosterPack_Cards" ADD FOREIGN KEY ("Card_ID") REFERENCES "Card" ("Card_ID");

ALTER TABLE "UserSession" ADD FOREIGN KEY ("User_ID") REFERENCES "User" ("User_ID");

ALTER TABLE "BattleLog" ADD FOREIGN KEY ("Deck_ID_1") REFERENCES "Deck" ("Deck_ID");

ALTER TABLE "BattleLog" ADD FOREIGN KEY ("Deck_ID_2") REFERENCES "Deck" ("Deck_ID");

ALTER TABLE "BattleLog" ADD FOREIGN KEY ("User_ID_Winner") REFERENCES "User" ("User_ID");

ALTER TABLE "BattleLogEntry" ADD FOREIGN KEY ("BattleLog_ID") REFERENCES "BattleLog" ("BattleLog_ID");

ALTER TABLE "BattleLogEntry" ADD FOREIGN KEY ("Card_ID_1") REFERENCES "Card" ("Card_ID");

ALTER TABLE "BattleLogEntry" ADD FOREIGN KEY ("Card_ID_2") REFERENCES "Card" ("Card_ID");
