@echo off
cls
echo This test block runs through a simple list of calls that test the elevator's up and down motion.
echo Calls are made at the beginning and then processed.
curl -s -X DELETE http://localhost:8080/car > nul
echo.

echo Sending requests TO floors 2, 7 and 4
curl -H "Content-Type: application/json" -d "{\"Floor\":2}" -X POST http://localhost:8080/car/CallFromCar
echo. = true
curl -H "Content-Type: application/json" -d "{\"Floor\":7}" -X POST http://localhost:8080/car/CallFromCar
echo. = true
curl -H "Content-Type: application/json" -d "{\"Floor\":4}" -X POST http://localhost:8080/car/CallFromCar
echo. = true
echo.

echo Sending a request FROM floor 4 going up - this should be processed at the same time as the other existing requests
curl -H "Content-Type: application/json" -d "{\"Floor\":5,\"Direction\":1}" -X POST http://localhost:8080/car/CallFromFloor
echo Sending a request FROM floor 5 going up - this should be processed at the same time as the other existing requests
curl -H "Content-Type: application/json" -d "{\"Floor\":5,\"Direction\":1}" -X POST http://localhost:8080/car/CallFromFloor
echo. = true
echo Sending a request FROM floor 3 going down - this should be processed after the existing requests
curl -H "Content-Type: application/json" -d "{\"Floor\":3,\"Direction\":-1}" -X POST http://localhost:8080/car/CallFromFloor
echo. = true
echo.

echo Beginning Status
curl http://localhost:8080/car
echo. = [2,4,7]
echo.

echo Process the requests
curl http://localhost:8080/car/Next
echo. = 2
curl http://localhost:8080/car/Next
echo. = 4
curl http://localhost:8080/car/Next
echo. = 5
curl http://localhost:8080/car/Next
echo. = 7
curl http://localhost:8080/car/Next
echo. = 3
curl http://localhost:8080/car/Next
echo. = 0
echo.
echo Current Status - Should be no more requests, return to the lobby
curl localhost:8080/car/status
echo. = {"floor":1,"direction":1}