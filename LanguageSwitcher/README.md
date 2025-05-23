# Language Switcher

A simple Windows application that allows you to quickly switch between installed languages using the left and right Ctrl keys.

## Features

- Switch to first language with left Ctrl key press
- Switch to second language with right Ctrl key press
- System tray icon showing current selected language
- Automatically starts with Windows

## Requirements

- Windows 10 or later (may work on earlier versions but not tested)
- .NET 6.0 Runtime (or later)
- At least two keyboard layouts/languages installed in Windows

## Project Structure

- `LanguageSwitcherApp/` - The main application
- `LanguageSwitcherSetup/` - The setup utility
- `build.bat` - Script to build both applications
- `make_dist.bat` - Script to create a distribution package

## Build Instructions

1. Install .NET 6.0 SDK or later from https://dotnet.microsoft.com/download
2. Open a command prompt and navigate to the LanguageSwitcher directory
3. Run `build.bat` to build both applications
4. Run `make_dist.bat` to create a distribution package in the 'dist' folder

## Installation

1. Copy all files from the 'dist' folder to a permanent location (e.g., C:\Program Files\LanguageSwitcher)
2. Run `LanguageSwitcherSetup.exe` as administrator to add the application to Windows startup

## Usage

- After installation, the application runs in the system tray
- Press the left Ctrl key to switch to the first language
- Press the right Ctrl key to switch to the second language
- Double-click the tray icon to see information about the current language
- Right-click the tray icon to access the menu with the Quit option

## Notes

- The application uses the order of languages as installed in Windows
- The first language is the first one in your Windows language list
- The second language is the second one in your Windows language list
- If you have only one language installed, both keys will switch to that language
- The application uses the default system icon; you can replace it with a custom icon if desired