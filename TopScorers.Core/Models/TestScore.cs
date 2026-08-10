using System;
namespace TopScorers.Core.Models;

    public class TestScore
{
	public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; }    = string.Empty;
    public int Score { get; set; }

    public string FullName => $"{FirstName} {SecondName}".Trim();

}
