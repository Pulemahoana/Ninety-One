using System;
using System.Text;
using TopScorers.Core.Models;

namespace TopScorers.Console;

public class CsvParser
{
    public List<TestScore> ParseCsv(string csvContent)
    {
        var result = new List<TestScore>();
        var lines = SplitLines(csvContent);

        if (lines.Length == 0)
            throw new ArgumentException("CSV file is empty");

        // Parse header
        var headers = ParseCsvLine(lines[0]);
        var firstNameIndex = -1;
        var secondNameIndex = -1;
        var scoreIndex = -1;

        for (int i = 0; i < headers.Length; i++)
        {
            var header = headers[i].Trim();
            if (header.Equals("First Name", StringComparison.OrdinalIgnoreCase))
                firstNameIndex = i;
            else if (header.Equals("Second Name", StringComparison.OrdinalIgnoreCase))
                secondNameIndex = i;
            else if (header.Equals("Score", StringComparison.OrdinalIgnoreCase))
                scoreIndex = i;
        }

        if (firstNameIndex == -1 || secondNameIndex == -1 || scoreIndex == -1)
            throw new ArgumentException("Invalid CSV format: Required columns not found");

        // Parse data rows
        for (int row = 1; row < lines.Length; row++)
        {
            if (string.IsNullOrWhiteSpace(lines[row]))
                continue;

            var fields = ParseCsvLine(lines[row]);

            // Handle rows with missing fields
            if (fields.Length <= Math.Max(scoreIndex, Math.Max(firstNameIndex, secondNameIndex)))
                continue;

            var firstName = fields[firstNameIndex].Trim();
            var secondName = fields[secondNameIndex].Trim();

            if (!int.TryParse(fields[scoreIndex].Trim(), out int score))
                continue;

            result.Add(new TestScore
            {
                FirstName = firstName,
                SecondName = secondName,
                Score = score
            });
        }

        return result;
    }

    private string[] SplitLines(string content)
    {
        return content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
    }

    private string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        var currentField = new StringBuilder();
        var inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    // Handle escaped quote
                    currentField.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(currentField.ToString());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }

        result.Add(currentField.ToString());
        return result.ToArray();
    }
}