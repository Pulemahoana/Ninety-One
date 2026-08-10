# Ninety-One
Assessment

Setup Instructions:
1. Clone or Download the repository to your local machine.
 - git clone https://github.com/Pulemahoana/Ninety-One.git
 - cd TopScorers
2. Build the solution 
 - dotnet restore
 - dotnet build
3. Verify the build
 - dotnet build --no-restore

Run the application
1. - cd TopScorers.Console 
   - dotnet run
2. To use different CSV files, replace the existing CSV files in the 
   TopScorers.Console/CSVFiles directory with your own CSV files. 
   Ensure that the new CSV files have the same structure as the original files.
3. Starting the API :
   - Navigate to the TopScorers.API directory cd TopScorers.API
   - Run the following command to start the API:
	 dotnet run
4. Verify the API is running by opening a web browser and navigating to http://localhost:5284/swagger. 
   You should see the Swagger UI, which allows you to interact with the API endpoints.
5. Technologies Used:
 - .NET 8.0 Application framework
 - SQLite - Lightweight database
 - Entity Framework Core - ORM for database operations
 - ASP.NET Core Web API - REST API framework
 - JWT - Authentication
 - Swagger/OpenAPI - API documentation
 - C# - Programming language
