using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TopScorers.Core.Interfaces;
using TopScorers.Core.Services;
using TopScorers.Data.Context;
using TopScorers.Data.Repositories;

namespace TopScorers.Console;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();

        // Ensure database is created before using it
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
            System.Console.WriteLine("Database ensured created.");
        }

        var repo = serviceProvider.GetRequiredService<ITestScoreRepository>();
        var parser = new CsvParser();
        var processor = new ScoreProcessor();

        try
        {
            System.Console.WriteLine("Top Scorers Application");
            System.Console.WriteLine("=======================\n");

            // Read CSV file
            string csvFilePath = args.Length > 0 ? args[0] : "TestData.csv";

            if (!File.Exists(csvFilePath))
            {
                System.Console.WriteLine($"Error: File '{csvFilePath}' not found.");
                System.Console.WriteLine("Usage: TopScorers.Console.exe [path-to-csv-file]");
                return;
            }

            System.Console.WriteLine($"Reading file: {csvFilePath}");
            var csvContent = await File.ReadAllTextAsync(csvFilePath);

            // Parse CSV
            System.Console.WriteLine("Parsing CSV data...");
            var scores = parser.ParseCsv(csvContent);

            if (scores.Count == 0)
            {
                System.Console.WriteLine("No valid data found in the CSV file.");
                return;
            }

            // Save to database
            System.Console.WriteLine($"Saving {scores.Count} records to database...");
            await repo.ClearAllAsync();
            await repo.AddRangeAsync(scores);

            // Process scores
            System.Console.WriteLine("Processing scores...");
            var (topScorers, topScore) = processor.ProcessScores(scores);

            // Output results
            System.Console.WriteLine("\n=== Top Scorers ===");
            foreach (var scorer in topScorers)
            {
                System.Console.WriteLine($"{scorer.FullName}");
            }
            System.Console.WriteLine($"Score: {topScore}");

            System.Console.WriteLine($"\nDatabase: {GetDatabasePath()}");

            // Show API instructions
            System.Console.WriteLine("\n=== API Available ===");
            System.Console.WriteLine("To start the API, run: cd TopScorers.API && dotnet run");
            System.Console.WriteLine("API endpoints:");
            System.Console.WriteLine("  POST /api/scores - Add a new score");
            System.Console.WriteLine("  GET /api/scores/{firstName}/{secondName} - Get score by name");
            System.Console.WriteLine("  GET /api/scores/top - Get top scorers");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error: {ex.Message}");
            System.Console.WriteLine(ex.StackTrace);
        }
    }

    static void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ITestScoreRepository, TestScoreRepository>();
    }

    static string GetDatabasePath()
    {
        var dbPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..", "..", "..",
            "TopScorers.Data",
            "test_scores.db");
        return Path.GetFullPath(dbPath);
    }
}