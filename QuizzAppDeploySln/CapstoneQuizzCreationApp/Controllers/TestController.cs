using CapstoneQuizzCreationApp.Interfaces;
using CapstoneQuizzCreationApp.Models.DTO.Folder;
using CapstoneQuizzCreationApp.Models.DTO.RequestDTO;
using CapstoneQuizzCreationApp.Models.DTO.ResponseDTO;
using CapstoneQuizzCreationApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneQuizzCreationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="User")]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testservice;
        private readonly ILogger<TestController> _logger;
        public TestController(ITestService testservice, ILogger<TestController> logger)
        {
            _testservice = testservice;
            _logger = logger;
        }

        [HttpGet("StartTest")]
        [ProducesResponseType(typeof(StartTestDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StartTestDTO>> StartTest(int CertificateTestId)
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var UserId = Convert.ToInt32(userstring);
                _logger.LogInformation($"User {UserId} attempting to start test {CertificateTestId}");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"Invalid model state for starting test {CertificateTestId} by user {UserId}");
                    return BadRequest(new ErrorModel(400, "Invalid input data"));
                }

                StartTestDTO result = await _testservice.StartTest(CertificateTestId, UserId);
                _logger.LogInformation($"User {UserId} successfully started test {CertificateTestId}");
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Unauthorized access attempt to start test {CertificateTestId}: {ex.Message}");
                return Unauthorized(new ErrorModel(401, "Unauthorized access"));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while user was starting test {CertificateTestId}: {ex.Message}");
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred"));
            }
        }

        [HttpGet("TestPreview")]
        [ProducesResponseType(typeof(TestPreviewDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TestPreviewDTO>> TestPreview(int CertificateTestId)
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var UserId = Convert.ToInt32(userstring);
                _logger.LogInformation($"User {UserId} requesting preview for test {CertificateTestId}");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"Invalid model state for test preview request for test {CertificateTestId} by user {UserId}");
                    return BadRequest(new ErrorModel(400, "Invalid input data"));
                }

                var result = await _testservice.TestPreviewPage(CertificateTestId, UserId);
                _logger.LogInformation($"Test preview for test {CertificateTestId} successfully retrieved for user {UserId}");
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Unauthorized access attempt to preview test {CertificateTestId}: {ex.Message}");
                return Unauthorized(new ErrorModel(401, "Unauthorized access"));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while retrieving preview for test {CertificateTestId} for user {ex.Message}");
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred"));
            }
        }

        [HttpGet("ResumeTest")]
        [ProducesResponseType(typeof(QuestionWithExpiryDate), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuestionWithExpiryDate>> ResumeTest(int SubmissionId)
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var UserId = Convert.ToInt32(userstring);
                _logger.LogInformation($"User {UserId} attempting to resume test submission {SubmissionId}");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"Invalid model state for resuming test submission {SubmissionId} by user {UserId}");
                    return BadRequest(new ErrorModel(400, "Invalid input data"));
                }

                QuestionWithExpiryDate result = await _testservice.ResumeTest(SubmissionId, UserId);
                _logger.LogInformation($"User {UserId} successfully resumed test submission {SubmissionId}");
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Unauthorized access attempt to resume test submission {SubmissionId}: {ex.Message}");
                return Unauthorized(new ErrorModel(401, "Unauthorized access"));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while user was starting test   {SubmissionId}: {ex.Message}");
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred"));
            }
        }

        [HttpPost("SynchronizeTestData")]
        [ProducesResponseType(typeof(SuccessSynchronieDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SuccessSynchronieDTO>> Synchronize(List<SynchronousDataDTO> data)
        {
            try
            {
                _logger.LogInformation($"Attempting to synchronize test data. Number of items: {data.Count}");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for test data synchronization");
                    return BadRequest(new ErrorModel(400, "Invalid input data"));
                }

                SuccessSynchronieDTO result = await _testservice.SynchronizeDb(data);
                _logger.LogInformation($"Test data synchronized successfully. Items processed: {data.Count}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred during test data synchronization: {ex.Message}");
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred during synchronization"));
            }
        }

        [HttpPost("SubmitTest")]
        [ProducesResponseType(typeof(TestResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TestResultDTO>> SubmitTest(SubmissionAnswerDTO data)
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var UserId = Convert.ToInt32(userstring);
                data.UserId = UserId;
                _logger.LogInformation($"User {UserId} attempting to submit test");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"Invalid model state for test submission by user {UserId}");
                    return BadRequest(new ErrorModel(400, "Invalid input data"));
                }

                TestResultDTO result = await _testservice.SubmitAnswer(data);
                _logger.LogInformation($"User {UserId} successfully submitted test");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred during test submission for user: {ex.Message}");
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred during test submission"));
            }
        }

        [HttpGet("GetLeaderboard")]
        [ProducesResponseType(typeof(List<LeaderBoardDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<LeaderBoardDTO>>> Leaderboard(int TestId)
        {
            try
            {
                _logger.LogInformation($"Retrieving leaderboard for test {TestId}");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"Invalid model state for leaderboard request for test {TestId}");
                    return BadRequest(new ErrorModel(400, "Invalid input data"));
                }

                List<LeaderBoardDTO> result = await _testservice.GetLeaderBoard(TestId);
                _logger.LogInformation($"Leaderboard for test {TestId} retrieved successfully");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while retrieving leaderboard for test {TestId}: {ex.Message}");
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred while retrieving the leaderboard"));
            }
        }

        [HttpGet("GetStats")]
        [ProducesResponseType(typeof(TestStatsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TestStatsDTO>> GetStats(int TestId)
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var UserId = Convert.ToInt32(userstring);
                _logger.LogInformation($"Retrieving stats for test {TestId} for user {UserId}");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"Invalid model state for stats request for test {TestId} by user {UserId}");
                    return BadRequest(new ErrorModel(400, "Invalid input data"));
                }

                TestStatsDTO result = await _testservice.GetTestStats(UserId, TestId);
                _logger.LogInformation($"Stats for test {TestId} retrieved successfully for user {UserId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while retrieving stats for test {TestId} for user: {ex.Message}");
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred while retrieving test stats"));
            }
        }

        [HttpGet("GetSubmissions")]
        [ProducesResponseType(typeof(List<TestSubmissionDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<TestSubmissionDTO>>> GetSubmissions(int TestId)
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var UserId = Convert.ToInt32(userstring);
                _logger.LogInformation($"Retrieving submissions for test {TestId} for user {UserId}");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"Invalid model state for submissions request for test {TestId} by user {UserId}");
                    return BadRequest(new ErrorModel(400, "Invalid input data"));
                }

                List<TestSubmissionDTO> result = await _testservice.GetUserSubmission(UserId, TestId);
                _logger.LogInformation($"Submissions for test {TestId} retrieved successfully for user {UserId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while retrieving submissions for test {TestId} for user: {ex.Message}");
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred while retrieving test submissions"));
            }
        }

        [HttpGet("GetTestCertificate")]
        [ProducesResponseType(typeof(CertificateDataDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CertificateDataDTO>> GetTestCertificate(int TestId)
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var UserId = Convert.ToInt32(userstring);
                _logger.LogInformation($"Retrieving certificate data for test {TestId} for user {UserId}");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"Invalid model state for certificate data request for test {TestId} by user {UserId}");
                    return BadRequest(new ErrorModel(400, "Invalid input data"));
                }

                CertificateDataDTO result = await _testservice.GetCertificateData(UserId, TestId);
                _logger.LogInformation($"Certificate data for test {TestId} retrieved successfully for user {UserId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while retrieving certificate data for test {TestId} for user: {ex.Message}");
                return StatusCode(500, new ErrorModel(500, "An unexpected error occurred while retrieving certificate data"));
            }
        }
    }
}
