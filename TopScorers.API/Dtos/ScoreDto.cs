namespace TopScorers.API.DTOs;

public class CreateScoreRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public int Score { get; set; }
}

public class ScoreResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public int Score { get; set; }
    public string FullName { get; set; } = string.Empty;
}

public class TopScorersResponse
{
    public List<string> TopScorers { get; set; } = new();
    public int Score { get; set; }
}