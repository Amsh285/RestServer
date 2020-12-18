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
