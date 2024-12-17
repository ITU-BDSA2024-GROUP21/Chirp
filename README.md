# Nooter
Nooter is a clone of Twitter, where you can post noots, follow other users, among other things.
This webapplication is hosted on Microsoft Azure, where you can access it with the following link:
https://bdsagroup021chirprazor.azurewebsites.net/

![image](https://github.com/user-attachments/assets/ce6ca2ed-9bc7-4cf3-871b-76e03dbf6cdb)


# Running Nooter locally
## Cloning Repository
To clone our project there are two ways. 

### First way
You to go to our Github repository then click on the code button. and choose the download as zip option. Then you need to open a terminal. Then you go into your Chirp.Web folder, now you write dotnet run and then click the link and your now on our website Nooter locally.

### Second way
The other option is to go to your terminal, in the folder you whish to clone it to and write the following comando  
`git clone https://github.com/ITU-BDSA2024-GROUP21/Chirp.git`

## Running the program
To Run our program you will need to have dowloaded tools like .NET. Once these tools have been downloaded you can follow these simple steps

1. Open your terminal
2. Go into the directory until you're in the folder Chirp.Web
3. Then write `dotnet run`
4. Click on the link https://localhost:5273
5. Then you should be able to see Nooter

# Running test cases
There are two ways to run the test locally on your computer. The first way you need to have two separate terminals open. The first needs to go into the Chirp.Web folder here you need to write `dotnet run`. Now that the website is up and running, you then need to go into the test folder in the other terminal. Here you run `dotnet test` to run our test. This runs both the normal test and the Playwright test. 

The other way to run our tests is to open our project in your chosen code editor, go into the PersonalProjectPath.cs class and edit the string path to your local path into the project. Now open your terminal go into the test folder, write `dotnet test` and all the test should be running.
