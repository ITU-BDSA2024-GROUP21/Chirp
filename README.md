# Nooter
Nooter is a Twitter clone where you can share noots, follow other users, customize your personal bio and so much more!
This web application can be run on localhost, but it is also hosted and automatically deployed to Microsoft Azure, and can be accessed on:
https://bdsagroup021chirprazor.azurewebsites.net/.

![image](https://github.com/user-attachments/assets/ce6ca2ed-9bc7-4cf3-871b-76e03dbf6cdb)

Nooter is a version of the Chirp-project from the BDSA2024 course on the IT-University of Copenhagen. 

# How to run Nooter locally
## Step 0: Prerequisites

1. [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
   - Required to build and run the application.

2. SQLite
   - System-level installation may be required for some platforms:
     - Linux: `sudo apt install sqlite3`
     - macOS: `brew install sqlite`
     - Windows: Typically pre-installed.
3. [Powershell](https://github.com/PowerShell/PowerShell)
   - Required for installing Playwright browsers to run the Playwright test suite

## Step 1: Clone the repository

In the terminal of your choice, navigate to the directory where you want Nooter to be located. Then, copy and paste the following command: 
```bash
git clone https://github.com/ITU-BDSA2024-GROUP21/Chirp.git
```

If this fails, navigate to the online repository: `https://github.com/ITU-BDSA2024-GROUP21/Chirp`. Click the green "code" button, and choose the "Download Zip" option. Move the downloaded zip-file to the directory of your choosing and unzip it.

## Step 2: Running Nooter 

The application is built on .NET, which makes running the program fairly easy. Typing `cd Chirp` in the terminal used in [Step #1](#step-1-clone-the-repository) should navigate to the root of your local clone of Nooter. If not, navigate to it, in a terminal of your choice. Then run the following command:
```bash
dotnet run --project src/Chirp.Razor/Chirp.Web
```

This will host the local Nooter clone on localhost. The default port is: [https://localhost:5273](https://localhost:5273). You can now utilize the functionality of Nooter. However, please note that it operates on a local SQLite database, meaning that you will not be able to interact with other users in real-time. Neither will you be able to use your GitHub account to register, as that requires organization secrets. Both of these things are however possible on the [Official Nooter Website](https://bdsagroup021chirprazor.azurewebsites.net/).


# How to run tests

### Assertion testing
   
To run the assertion test suite, you need to navigate to the root of your local clone of Nooter in a terminal of your choice. Then, run the following command:
```bash
dotnet test test/Chirp.Razor.Tests
```

### Playwright testing
   - In order to run the Playwright test suite, you need to have one of two prerequisites:
    
       1. Have the program running locally in the background, as described in ["Running Nooter"](#step-2-running-nooter)
       2. Change the defaultPath variable in `PersonalProjectPath.cs` to the string of the absolute path of `src/Chirp.Razor/Chirp.Web` on your computer. `PersonalProjectPath.cs` is located in `test/Chirp.Razor.Tests/PlaywrightTests`.
          
   - Once either of these prerequisites are met, simply run this command from the root of your local clone of Nooter:
     ```bash
     dotnet test test/Chirp.Razor.Tests/PlaywrightTests
     ```

# Functionality 

The website is intuitive and exploration is encouraged. The functionality includes but is not limited to:

- Registering as a user
    - Register using a GitHub acccount
    - Register as an original account
- Sharing Noots (messages)
- Deleting Noots
- Following other users
- Having a personal timeline with only noots from your own and followed accounts
- Your own background-Pingu
- Creating a bio
- Viewing other accounts and their Noots and bio
- Getting all your personal information in accordance with GDPR regulation
- Deleting your account

## License

[MIT](https://choosealicense.com/licenses/mit/)
