@echo off

cd %~dp0/crs-client
call npm install
call npm run clean
call npm run build

cd %~dp0/crs-client-signalr
call npm install
call npm run clean
call npm run build
popd

cd %~dp0/samples/client-webpack
call npm install
call npm run clean
call npm run build

cd %~dp0
