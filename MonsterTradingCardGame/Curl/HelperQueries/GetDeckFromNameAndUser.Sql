SELECT "Card"."Card_ID", "ElementType", "CardType", "Card"."Name", "Description", "AttackPoints"
FROM public."Deck_Cards"
JOIN public."Deck" ON "Deck"."Deck_ID" = "Deck_Cards"."Deck_ID"
JOIN public."User" ON "User"."User_ID" = "Deck"."User_ID"
JOIN public."Card" ON "Card"."Card_ID" = "Deck_Cards"."Card_ID"
WHERE "Deck"."Name" = 'Test01' AND "User"."UserName" = 'altenhof';