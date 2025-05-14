@echo off
echo Creating distribution package...

if not exist dist mkdir dist

echo Copying main application...
copy LanguageSwitcherApp\bin\Release\net6.0-windows\LanguageSwitcherApp.exe dist\LanguageSwitcher.exe
if %ERRORLEVEL% NEQ 0 (
    echo Error copying main application
    pause
    exit /b %ERRORLEVEL%
)

echo Copying setup application...
copy LanguageSwitcherSetup\bin\Release\net6.0-windows\LanguageSwitcherSetup.exe dist\LanguageSwitcherSetup.exe
if %ERRORLEVEL% NEQ 0 (
    echo Error copying setup application
    pause
    exit /b %ERRORLEVEL%
)

echo Copying necessary DLLs...
copy LanguageSwitcherApp\bin\Release\net6.0-windows\*.dll dist\
if %ERRORLEVEL% NEQ 0 (
    echo Error copying DLLs
    pause
    exit /b %ERRORLEVEL%
)

echo Distribution package created in the 'dist' folder.
echo.
echo To install the application:
echo 1. Copy all files from the 'dist' folder to the desired location
echo 2. Run LanguageSwitcherSetup.exe as administrator
pause