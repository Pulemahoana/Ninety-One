using System;

using TopScorers.Core.Models;

namespace TopScorers.Core.Services;

public class ScoreProcessor
{
        public (IEnumerable<TestScore> topScorers, int topScore) ProcessScores(IEnumerable<TestScore> scores)
    {
        if (!scores.Any())
            return (Enumerable.Empty<TestScore>(), 0);

        var topScore = scores.Max(s => s.Score);
        var topScorers = scores
            .Where(s => s.Score == topScore)
            .OrderBy(s => s.FullName)
            .ToList();

        return (topScorers, topScore);
    }
}
