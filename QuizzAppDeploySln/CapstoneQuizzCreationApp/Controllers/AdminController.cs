using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models.DTO.Folder;
using CapstoneQuizzCreationApp.Models.DTO.RequestDTO;
using CapstoneQuizzCreationApp.Models.DTO.ResponseDTO;
using CapstoneQuizzCreationApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneQuizzCreationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [EnableCors("MyCors")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AuthController> _logger;


        public AdminController(IAdminService adminService, ILogger<AuthController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }
        [HttpPost("CreateQuestion")]
        [ProducesResponseType(typeof(SuccessCertificationTestCreatedDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessCertificationTestCreatedDTO>> Register([FromForm] CreateQuestionDTO createQuestion)
        {
            try
            {
                _logger.LogInformation("Received a user registration request.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the registration request.");
                    return BadRequest(ModelState);
                }

                SuccessCertificationTestCreatedDTO result = await _adminService.CreateCertificationTest(createQuestion);
                _logger.LogInformation("User registered successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during user registration: {ex.Message}");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpPut("UpdateVisibility")]
        [ProducesResponseType(typeof(TestVisibleResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TestVisibleResponseDTO>> ChangeVisibility(int testId)
        {
            try
            {
                _logger.LogInformation("Received a user registration request.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the registration request.");
                    return BadRequest(ModelState);
                }

                var result = await _adminService.ChangeVisibility(testId);
                _logger.LogInformation("User registered successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during user registration: {ex.Message}");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpGet("GetAllTest")]
        [ProducesResponseType(typeof(List<AdminAllTestDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<AdminAllTestDTO>>> GetAllTest()
        {
            try
            {
                _logger.LogInformation("Received a user registration request.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the registration request.");
                    return BadRequest(ModelState);
                }

                var result = await _adminService.GetAllTest();
                _logger.LogInformation("User registered successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during user registration: {ex.Message}");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }

    }
}
