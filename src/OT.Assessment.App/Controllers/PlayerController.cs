using Microsoft.AspNetCore.Mvc;
using OT.Assessment.App.Exceptions;
using OT.Assessment.App.Services;
using OT.Assessment.Tester.Infrastructure;
namespace OT.Assessment.App.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        

        private readonly RabbitMQService _rabbitMQService;
        private ICasinoWagerService _casinoWagerService;
        private readonly ILogger<PlayerController> _logger;

        public PlayerController(RabbitMQService rabbitMQService, ICasinoWagerService casinoWagerService, ILogger<PlayerController> logger)
        {
            _rabbitMQService = rabbitMQService;
            _casinoWagerService = casinoWagerService;
            _logger = logger;
        }

        // POST api/player/casinowager
        // Endpoint to post a new casino wager

        [HttpPost("casinowager")]
        public IActionResult PostCasinoWager([FromBody] CasinoWager wager)
        {
            try
            {
                if (wager == null)
                {
                    _logger.LogWarning("Wager is null");
                    throw new ValidationException("Wager cannot be null");
                }

                _rabbitMQService.PublishWager(wager);
                return Ok("Wager created successfully.");
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, new { message = "An unexpected error occurred" });
            }
        }

        //GET api/player/{playerId}/wagers
        // Endpoint to get casino wagers by player ID with pagination

        [HttpGet("{playerId}/casino")]
        public async Task<IActionResult> GetCasinoWagers(Guid playerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _casinoWagerService.GetWagersByPlayerIdAsync(playerId, page, pageSize);

                // Create the response object containing the data, page information, total count, and total pages
                var response = new
                {
                    Data = result.Wagers,
                    Page = page,
                    PageSize = pageSize,
                    Total = result.Total,
                    TotalPages = result.TotalPages
                };

                // Return the response with status code 200 OK
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving casino wagers for player ID {PlayerId}", playerId);

                return StatusCode(500, new { message = "An error occurred while retrieving casino wagers. Please try again later." });
            }
        }


        //GET api/player/topSpenders?count=10    
        // Endpoint to get the top spenders

        [HttpGet("topSpenders")]
        public async Task<IActionResult> GetTopSpenders([FromQuery] int count = 10)
        {
            try
            {
                var topSpenders = await _casinoWagerService.GetTopSpendersAsync(count);

                return Ok(topSpenders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the top spenders");

                return StatusCode(500, new { message = "An error occurred while retrieving the top spenders. Please try again later." });
            }
        }

            
    }
}
