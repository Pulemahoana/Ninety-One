using System;
using TopScorers.Core.Models;

namespace TopScorers.Core.Interfaces;

public interface ITestScoreRepository
{
    Task<IEnumerable<TestScore>> GetAllAsync();
    Task<TestScore?> GetByFullNameAsync(string firstName, string secondName);
    Task<IEnumerable<TestScore>> GetTopScorersAsync();
    Task AddAsync(TestScore score);
    Task AddRangeAsync(IEnumerable<TestScore> scores);
    Task ClearAllAsync();
}
