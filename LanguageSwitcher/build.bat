@echo off
echo Building Language Switcher Application...
cd LanguageSwitcherApp
dotnet build -c Release
if %ERRORLEVEL% NEQ 0 (
    echo Error building Language Switcher Application
    cd ..
    pause
    exit /b %ERRORLEVEL%
)
cd ..

echo Building Language Switcher Setup...
cd LanguageSwitcherSetup
dotnet build -c Release
if %ERRORLEVEL% NEQ 0 (
    echo Error building Language Switcher Setup
    cd ..
    pause
    exit /b %ERRORLEVEL%
)
cd ..

echo Build complete!
echo.
echo Main application: LanguageSwitcherApp\bin\Release\net6.0-windows\LanguageSwitcherApp.exe
echo Setup application: LanguageSwitcherSetup\bin\Release\net6.0-windows\LanguageSwitcherSetup.exe
echo.
echo Copy both files to a distribution folder to deploy the application.
pause