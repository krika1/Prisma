using Microsoft.AspNetCore.Mvc;
using Prisma.Core.Extensions;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("users")]
    public class WeatherForecastController : ControllerBase 
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly ApplicationDbContext _database;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ApplicationDbContext database)
        {
            _logger = logger;
            _database = database;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            var users = await _database.Users!.ToListAsync()!;

            return Ok(users);
        }
    }
}
