# Language Switcher

A simple Windows application that allows quick switching between installed keyboard languages using the left and right Ctrl keys.

## Features

- Switch to the first language with a single left Ctrl key press
- Switch to the second language with a single right Ctrl key press
- System tray icon showing the current selected language
- Option to run at Windows startup
- Simple interface with language information

## Requirements

- Windows 10 or later
- Node.js 14+ (for development only)

## Installation

### Option 1: Download the installer

1. Download the latest installer from the releases page
2. Run the installer and follow the prompts
3. The application will start automatically after installation

### Option 2: Build from source

1. Clone this repository
2. Install dependencies:
   ```
   npm install
   ```
3. Build the installer:
   ```
   npm run build
   ```
4. The installer will be created in the `dist` folder

## Usage

After installation:

- The application runs in the system tray (notification area)
- Press the **left Ctrl key** once to switch to the first language in your Windows language list
- Press the **right Ctrl key** once to switch to the second language in your Windows language list
- Click the tray icon to see the current language and access application settings
- Right-click the tray icon for additional options, including the ability to quit the application

## Development

### Setup development environment

```
npm install
```

### Run the application in development mode

```
npm start
```

### Build installers

```
npm run build
```

## How it works

The application uses Electron's global shortcuts to capture Ctrl key presses and PowerShell commands to interact with Windows language settings. It uses the following Windows PowerShell cmdlets:

- `Get-WinUserLanguageList` - To get the list of installed languages
- `Set-WinUserLanguageList` - To switch to a specific language

## License

MIT