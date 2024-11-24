@echo off

REM Run this file in the same directory it is located in
cd /d "%~dp0"

REM Uncomment this to ensure there is no instance of Electron running
REM echo Killing all Electron instances...
taskkill /F /IM electron.exe /T

REM Uncomment this to create a log file for debugging
REM electronize start /watch > electronize_output.txt 2>&1

REM Start Electron app
electronize start

REM Navigate to the directory and start Electron manually if needed
REM cd "C:\AProjects\Electron\AIDemos\AIDemos\obj\Host\node_modules\.bin"
REM electron.cmd "..\..\main.js"

REM Optional: Uncomment to publish if needed
REM dotnet publish "C:\Electron\AIDemos\AIDemos\AIDemos.csproj" -r win-x64 -c "Debug" --output "C:\Electron\AIDemos\AIDemos\obj\Host\bin" /p:PublishReadyToRun=true /p:PublishSingleFile=true --no-self-contained

REM Prevent the window from closing immediately
pause

