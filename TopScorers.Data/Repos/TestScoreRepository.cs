using Microsoft.EntityFrameworkCore;
using TopScorers.Core.Interfaces;
using TopScorers.Core.Models;
using TopScorers.Data.Context;

namespace TopScorers.Data.Repositories;

public class TestScoreRepository : ITestScoreRepository
{
    private readonly ApplicationDbContext _context;

    public TestScoreRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TestScore>> GetAllAsync()
    {
        return await _context.TestScores.ToListAsync();
    }

    public async Task<TestScore?> GetByFullNameAsync(string firstName, string secondName)
    {
        return await _context.TestScores
            .FirstOrDefaultAsync(s =>
                s.FirstName.ToLower() == firstName.ToLower() &&
                s.SecondName.ToLower() == secondName.ToLower());
    }

    public async Task<IEnumerable<TestScore>> GetTopScorersAsync()
    {
        if (!await _context.TestScores.AnyAsync())
            return Enumerable.Empty<TestScore>();

        var maxScore = await _context.TestScores.MaxAsync(s => s.Score);
        return await _context.TestScores
            .Where(s => s.Score == maxScore)
            .OrderBy(s => s.FirstName)
            .ThenBy(s => s.SecondName)
            .ToListAsync();
    }

    public async Task AddAsync(TestScore score)
    {
        await _context.TestScores.AddAsync(score);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<TestScore> scores)
    {
        await _context.TestScores.AddRangeAsync(scores);
        await _context.SaveChangesAsync();
    }

    public async Task ClearAllAsync()
    {
        _context.TestScores.RemoveRange(_context.TestScores);
        await _context.SaveChangesAsync();
    }
}