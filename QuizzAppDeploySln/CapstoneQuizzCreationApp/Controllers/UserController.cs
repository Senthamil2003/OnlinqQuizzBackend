using CapstoneQuizzCreationApp.CustomException;
using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models;
using CapstoneQuizzCreationApp.Models.DTO.Folder;
using CapstoneQuizzCreationApp.Models.DTO.ResponseDTO;
using CapstoneQuizzCreationApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneQuizzCreationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("AllTests")]
        [ProducesResponseType(typeof(List<AllTestsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AllTestsDTO>>> GetAllTests()
        {
            try
            {
                var userId = GetUserId();
                _logger.LogInformation("Fetching all tests for user with ID {UserId}.", userId);

                var result = await _userService.GetAllTestsWithUsers(userId);
                _logger.LogInformation("Fetched all tests for user with ID {UserId}.", userId);
                return Ok(result);
            }
            catch (EmptyItemException)
            {
                _logger.LogWarning("No tests found for user with ID {UserId}.", GetUserId());
                return NotFound(new ErrorModel(404, "No tests found."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all tests for user with ID {UserId}.", GetUserId());
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        [HttpGet("History")]
        [ProducesResponseType(typeof(List<AllTestsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AllTestsDTO>>> GetTestHistory()
        {
            try
            {
                var userId = GetUserId();
                _logger.LogInformation("Fetching test history for user with ID {UserId}.", userId);

                var result = await _userService.GetUserHistory(userId);
                _logger.LogInformation("Fetched test history for user with ID {UserId}.", userId);
                return Ok(result);
            }
            catch (EmptyItemException)
            {
                _logger.LogWarning("No test history found for user with ID {UserId}.", GetUserId());
                return NotFound(new ErrorModel(404, "No test history found."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching test history for user with ID {UserId}.", GetUserId());
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        [HttpPost("AddToFavourite")]
        [ProducesResponseType(typeof(UpdateFavouriteDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UpdateFavouriteDTO>> AddFavourite(int TestId)
        {
            try
            {
                var userId = GetUserId();
                _logger.LogInformation("Adding test with ID {TestId} to favorites for user with ID {UserId}.", TestId, userId);

                var result = await _userService.AddToFavourate(userId, TestId);
                _logger.LogInformation("Added test with ID {TestId} to favorites for user with ID {UserId}.", TestId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding test with ID {TestId} to favorites for user with ID {UserId}.", TestId, GetUserId());
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        [HttpPut("RemoveFromFavourite")]
        [ProducesResponseType(typeof(UpdateFavouriteDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UpdateFavouriteDTO>> RemoveFavourite(int FavouriteId)
        {
            try
            {
                var userId = GetUserId();
                _logger.LogInformation("Removing favorite with ID {FavouriteId} for user with ID {UserId}.", FavouriteId, userId);

                var result = await _userService.RemoveFromFavourite(userId, FavouriteId);
                _logger.LogInformation("Removed favorite with ID {FavouriteId} for user with ID {UserId}.", FavouriteId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing favorite with ID {FavouriteId} for user with ID {UserId}.", FavouriteId, GetUserId());
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        [HttpGet("GetMyFavourites")]
        [ProducesResponseType(typeof(List<AllTestsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AllTestsDTO>>> GetAllMyFavourites()
        {
            try
            {
                var userId = GetUserId();
                _logger.LogInformation("Fetching all favorites for user with ID {UserId}.", userId);

                var result = await _userService.GetMyFavourite(userId);
                _logger.LogInformation("Fetched all favorites for user with ID {UserId}.", userId);
                return Ok(result);
            }
            catch (EmptyItemException)
            {
                _logger.LogWarning("No favorites found for user with ID {UserId}.", GetUserId());
                return NotFound(new ErrorModel(404, "No favorites found."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all favorites for user with ID {UserId}.", GetUserId());
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        [HttpGet("GetPopularTests")]
        [ProducesResponseType(typeof(List<AllTestsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AllTestsDTO>>> GetPopularTests()
        {
            try
            {
                var userId = GetUserId();
                _logger.LogInformation("Fetching popular tests for user with ID {UserId}.", userId);

                var result = await _userService.GetPopularTests(userId);
                _logger.LogInformation("Fetched popular tests for user with ID {UserId}.", userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching popular tests for user with ID {UserId}.", GetUserId());
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        private int GetUserId()
        {
            var userIdString = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID.");
            }
            return userId;
        }
    }
}
