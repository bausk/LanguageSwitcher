@echo off
echo Installing Language Switcher dependencies...
npm install

echo Building Language Switcher installer...
npm run build

echo.
echo Build complete! 
echo The installer can be found in the 'dist' folder.
echo.
pause