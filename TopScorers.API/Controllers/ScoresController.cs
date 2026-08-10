using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopScorers.API.DTOs;
using TopScorers.Core.Interfaces;
using TopScorers.Core.Models;
using TopScorers.Core.Services;

namespace TopScorers.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScoresController : ControllerBase
{
    private readonly ITestScoreRepository _repository;
    private readonly ScoreProcessor _processor;
    private readonly ILogger<ScoresController> _logger;

    public ScoresController(ITestScoreRepository repository, ScoreProcessor processor, ILogger<ScoresController> logger)
    {
        _repository = repository;
        _processor = processor;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> AddScore([FromBody] CreateScoreRequest request)
    {
        try
        {
            _logger.LogInformation($"Adding score for {request.FirstName} {request.SecondName}");

            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.SecondName))
                return BadRequest("First name and second name are required");

            if (request.Score < 0 || request.Score > 100)
                return BadRequest("Score must be between 0 and 100");

            var score = new TestScore
            {
                FirstName = request.FirstName,
                SecondName = request.SecondName,
                Score = request.Score
            };

            await _repository.AddAsync(score);

            var response = new ScoreResponse
            {
                Id = score.Id,
                FirstName = score.FirstName,
                SecondName = score.SecondName,
                Score = score.Score,
                FullName = score.FullName
            };

            return CreatedAtAction(nameof(GetScoreByFullName),
                new { firstName = score.FirstName, secondName = score.SecondName },
                response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding score for {request.FirstName} {request.SecondName}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{firstName}/{secondName}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetScoreByFullName(string firstName, string secondName)
    {
        try
        {
            _logger.LogInformation($"Getting score for {firstName} {secondName}");
            var score = await _repository.GetByFullNameAsync(firstName, secondName);

            if (score == null)
                return NotFound($"No record found for {firstName} {secondName}");

            var response = new ScoreResponse
            {
                Id = score.Id,
                FirstName = score.FirstName,
                SecondName = score.SecondName,
                Score = score.Score,
                FullName = score.FullName
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting score for {firstName} {secondName}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("top")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTopScorers()
    {
        try
        {
            _logger.LogInformation("Getting top scorers");
            var topScorers = await _repository.GetTopScorersAsync();

            if (!topScorers.Any())
                return NotFound("No scores found in the database");

            var topScore = topScorers.First().Score;

            var response = new TopScorersResponse
            {
                TopScorers = topScorers.Select(s => s.FullName).ToList(),
                Score = topScore
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top scorers");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllScores()
    {
        try
        {
            _logger.LogInformation("Getting all scores");
            var scores = await _repository.GetAllAsync();

            var response = scores.Select(s => new ScoreResponse
            {
                Id = s.Id,
                FirstName = s.FirstName,
                SecondName = s.SecondName,
                Score = s.Score,
                FullName = s.FullName
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all scores");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}