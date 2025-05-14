@echo off
echo Building Language Switcher...
dotnet build LanguageSwitcher.csproj -c Release
if %ERRORLEVEL% NEQ 0 (
    echo Error building Language Switcher
    pause
    exit /b %ERRORLEVEL%
)

echo Building Language Switcher Setup...
dotnet build LanguageSwitcherSetup.csproj -c Release
if %ERRORLEVEL% NEQ 0 (
    echo Error building Language Switcher Setup
    pause
    exit /b %ERRORLEVEL%
)

echo Build complete! Files are in bin\Release\net6.0-windows
echo.
echo You can now install the application by running LanguageSwitcherSetup.exe
pause