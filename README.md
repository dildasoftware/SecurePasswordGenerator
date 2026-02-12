# Secure Password Generator 🔐

A modern, cryptographically secure password and passphrase generator built with **Blazor WebAssembly** and **C#**.

Designed as a **Progressive Web App (PWA)**, this application runs completely client-side in your browser, ensuring your data never leaves your device. It works offline and can be installed on Desktop (Windows/Mac) and Mobile (Android/iOS).

## ✨ Key Features

### 🔑 Password Generation
- Generate strong random passwords (up to 128 characters).
- Customize character sets (Uppercase, Lowercase, Numbers, Symbols).
- Option to exclude similar characters (e.g., `l`, `1`, `I`, `0`, `O`).

### 🗣️ Passphrase Mode (New)
- Create memorable passphrases using dictionary words (e.g., `correct-horse-battery-staple`).
- Support for **English** 🇬🇧 and **Turkish** 🇹🇷 dictionaries.
- Adjustable word count, separators, and capitalization.

### 🛡️ Security & Analysis
- **100% Client-Side**: No data is sent to any server. All generation happens in your browser.
- **Strength Meter**: Real-time entropy calculation and "Time-to-Crack" estimation based on brute-force scenarios.

### 💾 History & Management
- Save generated passwords locally using **IndexedDB**.
- Add **Labels** and **Tags** to organize your passwords.
- Search, filter, and delete saved entries.

### ⚙️ Advanced Tools
- **Export/Import**: Backup your data to JSON or CSV (Excel compatible).
- **Offline Capable**: Works without internet connection thanks to Service Workers.

## 🚀 Technologies Used

- **Framework**: [Blazor WebAssembly](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) (.NET 8)
- **Language**: C# 12, Razor
- **Styling**: Custom CSS3 with Glassmorphism & Space Theme animations.
- **Storage**: IndexedDB (via JavaScript Interop)
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, Web layers).
- **Design Patterns**: Dependency Injection, SOLID Principles.

## 📦 How to Run

1. Ensure you have the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed.
2. Clone this repository:
   ```bash
   git clone https://github.com/yourusername/SecurePasswordGenerator.git
   ```
3. Navigate to the project folder:
   ```bash
   cd SecurePasswordGenerator
   ```
4. Run the application:
   ```bash
   dotnet run --project SecurePasswordGenerator.Web
   ```
5. Open your browser and go to `http://localhost:5000` (or the port shown in terminal).

## 📸 Screenshots

*(You can add screenshots of your application here)*

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
