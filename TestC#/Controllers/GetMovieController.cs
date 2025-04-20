using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using TestC_.Services;
using TestC_.Models;
using TestC_.Interface;

namespace TestC_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("GetMovieInformation")]
        public IActionResult GetMovieInformation([FromQuery] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest(new { ErrorCode = 400, Message = "Search term cannot be empty" });
                }

                var results = _movieService.SearchMovies(name);
                return results.Any() ? Ok(results) : NotFound(new { ErrorCode = 404, Message = "No movie found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorCode = 500, Message = "An unexpected error occurred", Details = ex.Message });
            }
        }
    }
}
