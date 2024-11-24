@echo off

REM Run this file in the same directory it is located in
cd /d "%~dp0"

taskkill /F /IM Electron.exe /T
taskkill /F /IM AINetProfit.exe /T
taskkill /F /IM ePowerEvent.exe /T

;npm install

REM Prevent the window from closing immediately
;pause


@echo off
setlocal

:: Replace <yourAppName> with the name of your Electron app
set "appName=<yourAppName>"

:: Set path to the app's cache directory
set "cachePath=%APPDATA%\%appName%"

echo Clearing cache for %appName% at %cachePath%

:: Check if the directory exists, then delete all contents
if exist "%cachePath%" (
    rd /s /q "%cachePath%"
    echo Cache cleared successfully.
) else (
    echo Cache directory not found: %cachePath%
)

endlocal
pause
