@echo off

REM --------------------------------------------------
REM Monster Trading Cards Game
REM --------------------------------------------------
title Monster Trading Cards Game
echo CURL Testing for Monster Trading Cards Game
echo.

REM --------------------------------------------------
echo 1) Create Users (Registration)
REM Create User
echo 1.1) Create Kienboec
curl -X POST http://127.0.0.1:13001/Register --header "Content-Type: Json" -d "{\"UserName\":\"kienboec\", \"FirstName\":\"Daniel\", \"LastName\":\"Kienboec\", \"Email\":\"FanpostDanielKienboeck@foo.at\", \"Password\":\"daniel\"}"
echo.

echo 1.2) Create Altenhofer
curl -X POST http://127.0.0.1:13001/Register --header "Content-Type: Json" -d "{\"UserName\":\"altenhof\", \"FirstName\":\"Markus\", \"LastName\":\"Altenhofer\", \"Email\":\"FanpostAltenhofer@foo.at\", \"Password\":\"daniel\"}"
echo.

echo should fail
REM Create User
curl -X POST http://127.0.0.1:13001/Register --header "Content-Type: Json" -d "{\"UserName\":\"kienboec\", \"FirstName\":\"Daniel\", \"LastName\":\"Kienboec\", \"Email\":\"FanpostDanielKienboeck@foo.at\", \"Password\":\"123\"}"
echo.

curl -X POST http://127.0.0.1:13001/Register --header "Content-Type: Json" -d "{\"UserName\":\"altenhof\", \"FirstName\":\"Markus\", \"LastName\":\"Altenhofer\", \"Email\":\"FanpostAltenhofer@foo.at\", \"Password\":\"123\"}"
echo.

REM --------------------------------------------------
echo 2) Login Users
curl -X POST http://127.0.0.1:13001/Authentication --header "username: kienboec" --header "password: daniel" --cookie-jar kienboec.kekse
echo.

curl -X POST http://127.0.0.1:13001/Authentication --header "username: altenhof" --header "password: daniel" --cookie-jar altenhof.kekse
echo.

REM --------------------------------------------------

echo 3) create cards

echo 3.1) create fire cards

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Fire\", \"Type\":\"Monster\", \"Name\":\"Fire Goblin\", \"Description\":\"Fire Goblins are mostly feared by new Adventurers.\", \"AttackPoints\":12}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Fire\", \"Type\":\"Monster\", \"Name\":\"Fire Troll\", \"Description\":\"Der mächtige Feuertroll hat schon vielen Abenteurern das Leben gekostet.\", \"AttackPoints\":22}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Fire\", \"Type\":\"Spell\", \"Name\":\"Feuersbrunst\", \"Description\":\"Ivan und seine Gefolgsmänner kannten keine Gnade und brannten jedes Haus nieder.\", \"AttackPoints\":34}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Fire\", \"Type\":\"Monster\", \"Name\":\"Ivan der schreckliche\", \"Description\":\"Ivan terrorisiert das Land mit seiner perversen affinität zu Sprengstoff.\", \"AttackPoints\":40}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Fire\", \"Type\":\"Monster\", \"Name\":\"Roter Drache\", \"Description\":\"Rote Drachen gehören zu den Chromatischen Drachen und sind von Natur aus bösartig. Sie haben große Hörner an ihrem Schädel, die sich nach hinten zum Nacken erstrecken. Kleinere Hörner befinden sich an ihren Wangen und am Kinn. Ein Kamm beginnt am Schädel und zieht sich bis zum Schwanz hinab. Sie stinken nach Rauch und Schwefel. Ihre Schuppen sind von purpurner und scharlachroter Farbe. Ihre Pupillen verbleichen mit dem Alter, bis sie schließlich den Augen das Aussehen von glühender Lava verleihen.\", \"AttackPoints\":70}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Fire\", \"Type\":\"Spell\", \"Name\":\"Feuerschlag\", \"Description\":\"Der Feuerschlag ist eine Technik die von Priestern des Feuertempels von Generation zu Generation weitergegeben wird.\", \"AttackPoints\":30}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Fire\", \"Type\":\"Monster\", \"Name\":\"Feueroger\", \"Description\":\"Der Feueroger lebt in Harmonie mit den Feuertrollen und Feuergoblins. Seine mangelnde Intelligenz gleicht er mit seiner Körperkraft aus.\", \"AttackPoints\":30}"
echo.

echo 3.2) create normal cards

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Normal\", \"Type\":\"Monster\", \"Name\":\"God Emperor Trump\", \"Description\":\"The Emperor of Mankind, often referred to by His faithful as the God-Emperor, the Master of Mankind, or simply the Emperor, is the immortal Perpetual and psyker who serves as the reigning monarch of the Imperium of Man, and is described by the Imperial Ecclesiarchy and the Imperial Cult as the Father, Guardian and God of Humanity.\", \"AttackPoints\":100}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Normal\", \"Type\":\"Monster\", \"Name\":\"Einheitsmatrix\", \"Description\":\"Die Einheitsmatrix oder Identitätsmatrix ist in der Mathematik eine quadratische Matrix, deren Elemente auf der Hauptdiagonale eins und überall sonst null sind. Die Einheitsmatrix ist im Ring der quadratischen Matrizen das neutrale Element bezüglich der Matrizenmultiplikation. Sie ist symmetrisch, selbstinvers, idempotent und hat maximalen Rang. Die Einheitsmatrix ist die Darstellungsmatrix der Identitätsabbildung eines endlichdimensionalen Vektorraums. Sie wird unter anderem bei der Definition des charakteristischen Polynoms einer Matrix, orthogonaler und unitärer Matrizen, sowie in einer Reihe geometrischer Abbildungen verwendet.\", \"AttackPoints\":23}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Normal\", \"Type\":\"Spell\", \"Name\":\"QR-Zerlegung\", \"Description\":\"Die QR-Zerlegung oder QR-Faktorisierung ist ein Begriff aus den mathematischen Teilgebieten der linearen Algebra und Numerik. Man bezeichnet damit die Zerlegung einer Matrix in das Produkt zweier anderer Matrizen, wobei eine orthogonale bzw. unitäre Matrix und eine obere Dreiecksmatrix ist.\", \"AttackPoints\":30}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Normal\", \"Type\":\"Monster\", \"Name\":\"Rotationsmatrix\", \"Description\":\"Eine Drehmatrix oder Rotationsmatrix ist eine reelle, orthogonale Matrix mit Determinante +1. Ihre Multiplikation mit einem Vektor lässt sich interpretieren als Drehung des Vektors im euklidischen Raum oder als passive Drehung des Koordinatensystems, dann mit umgekehrtem Drehsinn.\", \"AttackPoints\":25}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Normal\", \"Type\":\"Monster\", \"Name\":\"Mathemann\", \"Description\":\"Mathemann ist eine Figur in der gleichnamigen Comedy-Animationsserie auf YouTube, die He-Man parodiert und zum Bewältigen seiner Aufgaben, die Kräfte der Mathematik einsetzt.\", \"AttackPoints\":50}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Normal\", \"Type\":\"Monster\", \"Name\":\"Charakteristisches Polynom\", \"Description\":\"Das charakteristische Polynom (CP) ist ein Begriff aus dem mathematischen Teilgebiet der linearen Algebra. Dieses Polynom, das für quadratische Matrizen und Endomorphismen endlichdimensionaler Vektorräume definiert ist, gibt Auskunft über einige Eigenschaften der Matrix oder linearen Abbildung. Die Gleichung, in der das charakteristische Polynom gleich null gesetzt wird, wird manchmal Säkulargleichung genannt. Ihre Lösungen sind die Eigenwerte der Matrix bzw. der linearen Abbildung.\", \"AttackPoints\":33}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Normal\", \"Type\":\"Spell\", \"Name\":\"Gram-Schmidtsches Orthogonalisierungsverfahren\", \"Description\":\"Das Gram-Schmidtsche Orthogonalisierungsverfahren ist ein Algorithmus aus dem mathematischen Teilgebiet der linearen Algebra. Er erzeugt zu jedem System linear unabhängiger Vektoren aus einem Prähilbertraum ein Orthogonalsystem, das denselben Untervektorraum erzeugt.\", \"AttackPoints\":45}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Normal\", \"Type\":\"Monster\", \"Name\":\"Orthonormalbasis\", \"Description\":\"Eine Orthonormalbasis (ONB) oder ein vollständiges Orthonormalsystem (VONS) ist in den mathematischen Gebieten lineare Algebra und Funktionalanalysis eine Menge von Vektoren aus einem Vektorraum mit Skalarprodukt (Innenproduktraum), welche auf die Länge eins normiert und zueinander orthogonal (daher Ortho-normal-basis) sind und deren lineare Hülle dicht im Vektorraum liegt. Im endlichdimensionalen Fall ist dies eine Basis des Vektorraums. Im unendlichdimensionalen Fall handelt es sich nicht um eine Vektorraumbasis im Sinn der linearen Algebra.\", \"AttackPoints\":34}"
echo.

echo 3.3) create water cards

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Water\", \"Type\":\"Monster\", \"Name\":\"Water Goblin\", \"Description\":\"Water Goblins are mostly feared by new Adventurers.\", \"AttackPoints\":12}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Water\", \"Type\":\"Monster\", \"Name\":\"Wasserschlange\", \"Description\":\"Wasserschlangen können meistens in Sumpfgebieten aufgefunden werden wo sie Ahnunglosen Abenteurern auflaufern und sie vergiften.\", \"AttackPoints\":16}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Water\", \"Type\":\"Spell\", \"Name\":\"Wasserschlag\", \"Description\":\"Der Wasserschlag ist eine Technik die von Priestern des Wassertempels von Generation zu Generation weitergegeben wird.\", \"AttackPoints\":33}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Water\", \"Type\":\"Monster\", \"Name\":\"Blauer Drache\", \"Description\":\"Blaue Drachen sind Chromatische Drachen und damit von Natur aus böse. Ihr auffallendstes Merkmal sind die großen und markant geformten Ohren, sowie das große Horn auf ihrer Schnauze. Ihre azurfarbenen Schuppen glänzen im Sonnenlicht und es umgibt sie stets ein Hauch von Ozon. Sie sind eitel und sehr auf ihr Territorium bedacht.\", \"AttackPoints\":64}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Water\", \"Type\":\"Spell\", \"Name\":\"Frostschlag\", \"Description\":\"Rache wird am liebsten eiskalt serviert.\", \"AttackPoints\":36}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Water\", \"Type\":\"Monster\", \"Name\":\"Eiskönigin\", \"Description\":\"Die Königstochter Anna begibt sich auf die Suche nach ihrer älteren Schwester Elsa, der Eiskönigin. Anna wird auf ihrer abenteuerlichen Reise von dem Bergsteigspezialisten und Naturburschen Kristoff sowie dem Rentier Sven unterstützt. Gemeinsam wollen sie Elsa finden, die dafür verantwortlich ist, dass das Königreich Arendelle nun im ewigen Eis gefangen ist. Die drei Abenteurer müssen sich auf ihrer Reise gegen die Elemente behaupten.\", \"AttackPoints\":55}"
echo.

curl -X POST http://127.0.0.1:13001/Card --header "Content-Type: Json" -d "{\"Element\":\"Water\", \"Type\":\"Spell\", \"Name\":\"Poseidons Kiss\", \"Description\":\"Everyone fears it...\", \"AttackPoints\":100}"
echo.


REM --------------------------------------------------
echo 4) Buy Boosters
echo 4) altenhof Buy Boosters

curl -X POST http://127.0.0.1:13001/Shop/Booster --cookie altenhof.kekse
echo.

curl -X POST http://127.0.0.1:13001/Shop/Booster --cookie altenhof.kekse
echo.

curl -X POST http://127.0.0.1:13001/Shop/Booster --cookie altenhof.kekse
echo.

curl -X POST http://127.0.0.1:13001/Shop/Booster --cookie altenhof.kekse
echo.

echo 4) kienboec Buy Boosters

curl -X POST http://127.0.0.1:13001/Shop/Booster --cookie kienboec.kekse
echo.

curl -X POST http://127.0.0.1:13001/Shop/Booster --cookie kienboec.kekse
echo.

curl -X POST http://127.0.0.1:13001/Shop/Booster --cookie kienboec.kekse
echo.

echo 4) should Fail

curl -X POST http://127.0.0.1:13001/Shop/Booster --cookie altenhof.kekse
echo.

REM --------------------------------------------------
echo 5) Open Boosters

echo 5) Open Boosters altenhof

curl -X POST http://127.0.0.1:13001/User/OpenFirstBoosterPackage --cookie altenhof.kekse
echo.

curl -X POST http://127.0.0.1:13001/User/OpenFirstBoosterPackage --cookie altenhof.kekse
echo.

curl -X POST http://127.0.0.1:13001/User/OpenFirstBoosterPackage --cookie altenhof.kekse
echo.

curl -X POST http://127.0.0.1:13001/User/OpenFirstBoosterPackage --cookie altenhof.kekse
echo.

echo 5) Open Boosters kienboec

curl -X POST http://127.0.0.1:13001/User/OpenFirstBoosterPackage --cookie kienboec.kekse
echo.

curl -X POST http://127.0.0.1:13001/User/OpenFirstBoosterPackage --cookie kienboec.kekse
echo.

curl -X POST http://127.0.0.1:13001/User/OpenFirstBoosterPackage --cookie kienboec.kekse
echo.

echo 5) Should fail

curl -X POST http://127.0.0.1:13001/User/OpenFirstBoosterPackage --cookie altenhof.kekse
echo.

curl -X POST http://127.0.0.1:13001/User/OpenFirstBoosterPackage --cookie kienboec.kekse
echo.

REM --------------------------------------------------
echo 6) Show Aquired Cards altenhof

curl -X GET http://127.0.0.1:13001/User/ShowCardLibrary --cookie altenhof.kekse
echo.

echo 6) Show Aquired Cards kienboec

curl -X GET http://127.0.0.1:13001/User/ShowCardLibrary --cookie kienboec.kekse
echo.

REM --------------------------------------------------
echo 7) Generate 'optimized' altenhof

curl -X POST http://127.0.0.1:13001/User/AutoGenerateDeck --header "name: Test01" --cookie altenhof.kekse
echo.
curl -X POST http://127.0.0.1:13001/User/AutoGenerateDeck --header "name: Deck01" --cookie altenhof.kekse
echo.

echo 7) Generate 'optimized' kienboec

curl -X POST http://127.0.0.1:13001/User/AutoGenerateDeck --header "name: Deck01" --cookie kienboec.kekse
echo.

echo 7) should fail

curl -X POST http://127.0.0.1:13001/User/AutoGenerateDeck --header "name: Deck01" --cookie altenhof.kekse
echo.
curl -X POST http://127.0.0.1:13001/User/AutoGenerateDeck --header "name: Deck01" --cookie kienboec.kekse
echo.

REM --------------------------------------------------

echo 8) Query Deck01 from altenhof

curl -X GET http://127.0.0.1:13001/User/Deck/Deck01 --cookie altenhof.kekse
echo.

echo 8) Query Deck01 from kienboec

curl -X GET http://127.0.0.1:13001/User/Deck/Deck01 --cookie kienboec.kekse
echo.

REM --------------------------------------------------

echo 9) Queue for duels

echo 9) Queue for duels altenhof
start /b "altenhof battle" curl -X POST http://127.0.0.1:13001/Battle --header "deckName: Deck01" --cookie altenhof.kekse

echo 9) Queue for duels kienboec
start /b "kienboec battle" curl -X POST http://127.0.0.1:13001/Battle --header "deckName: Deck01" --cookie kienboec.kekse
ping localhost -n 10 >NUL 2>NUL
