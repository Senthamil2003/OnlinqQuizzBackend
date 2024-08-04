using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models.DTO.Folder;
using CapstoneQuizzCreationApp.Models.DTO.RequestDTO;
using CapstoneQuizzCreationApp.Models.DTO.ResponseDTO;
using log4net.Core;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneQuizzCreationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyCors")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        /// <param name="userLoginDTO">The user login credentials.</param>
        /// <returns>A success response with the JWT token on successful authentication.</returns>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(SuccessLoginDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessLoginDTO>> Login(LoginDTO userLoginDTO)
        {
            try
            {
                _logger.LogInformation("Received a user login request for user: {Username}", userLoginDTO.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the login request: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(new ErrorModel(400, "Invalid login data."));
                }

                var result = await _authService.Login(userLoginDTO);
                _logger.LogInformation("User '{Username}' authenticated successfully.", userLoginDTO.Email);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Authentication failed for user: {Username}. Reason: {Message}", userLoginDTO.Email, ex.Message);
                return Unauthorized(new ErrorModel(401, "Invalid credentials."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user authentication for user: {Username}.", userLoginDTO.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userDTO">The user registration details.</param>
        /// <returns>A success response on successful registration.</returns>
        [HttpPost("Register")]
        [ProducesResponseType(typeof(SuccessRegisterDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessRegisterDTO>> Register(RegisterDTO userDTO)
        {
            try
            {
                _logger.LogInformation("Received a user registration request for user: {Username}", userDTO.Name);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the registration request: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return BadRequest(new ErrorModel(400, "Invalid registration data."));
                }

                SuccessRegisterDTO result = await _authService.Register(userDTO);
                _logger.LogInformation("User '{Username}' registered successfully.", userDTO.Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user registration for user: {Username}.", userDTO.Name);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, "An error occurred while processing your request."));
            }
        }

        [HttpGet("validate")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public IActionResult ValidateToken()
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token is missing in the request.");
                return Unauthorized(new ErrorModel(401, "Token is missing."));
            }

            var (isValid, role) = _authService.ValidateUserTokenAndGetRole(token);
            if (isValid)
            {
                _logger.LogInformation("Token validated successfully. Role: {Role}", role);
                return Ok(new { Message = "Token is valid.", Role = role });
            }
            else
            {
                _logger.LogWarning("Token validation failed. Token: {Token}", token);
                return Unauthorized(new ErrorModel(401, "Token is invalid."));
            }
        }
    }
}
