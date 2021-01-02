REM --------------------------------------------------
echo 9) Trading Test

curl -X POST http://127.0.0.1:13001/Trade/73FBC767-D80C-4946-A447-A0E599084980 --header "Content-Type: Json" --cookie altenhof.kekse --data-binary @./Response/tradeCardAltenhof.txt
echo.

curl -X POST http://127.0.0.1:13001/Trade/73FBC767-D80C-4946-A447-A0E599084980/offer/745E0D11-60E8-4FB3-A501-323FBC6AF25C  --header "Content-Type: Json" --cookie kienboec.kekse --data-binary @./Response/tradeCardKienboec.txt
echo.

curl -X POST http://127.0.0.1:13001/Trade/73FBC767-D80C-4946-A447-A0E599084980/offer/745E0D11-60E8-4FB3-A501-323FBC6AF25C/accept --cookie altenhof.kekse
echo.
