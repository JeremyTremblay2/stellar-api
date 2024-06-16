using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using StellarApi.DTOs.Space;
using StellarApi.DTOtoModel;
using StellarApi.Infrastructure.Business;
using StellarApi.Model.Space;
using StellarApi.Repository.Exceptions;

namespace StellarApi.RestApi.Controllers;

/// <summary>
/// API controller for managing space images.
/// </summary>
[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/space-images/")]
public class SpaceImageController : ControllerBase
{
    /// <summary>
    /// The service used by this controller.
    /// </summary>
    private readonly ISpaceImageService _service;

    /// <summary>
    /// Logger used by this controller.
    /// </summary>
    private readonly ILogger<SpaceImageController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpaceImageController"/> class.
    /// </summary>
    /// <param name="service">The service used by this controller.</param>
    /// <param name="logger">The logger used by this controller.</param>
    public SpaceImageController(ISpaceImageService service, ILogger<SpaceImageController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [MapToApiVersion(1)]
    [HttpGet("{id}")]
    [ProducesResponseType<SpaceImageDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<SpaceImageDTO?>> GetSpaceImage(int id)
    {
        _logger.LogInformation($"Getting space image n°{id}.");
        try
        {
            var spaceImage = await _service.GetSpaceImage(id);
            if (spaceImage is null)
            {
                _logger.LogInformation($"Space image n°{id} not found.");
                return NotFound($"Space image n°{id} not found.");
            }

            _logger.LogInformation($"The space image n°{id} was found and fetched successfully.");
            return Ok(spaceImage.ToDTO());
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError(
                $"The space image n°{id} could not be fetched due to an unavailable database. More details: {ex.Message}");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "The database is not available.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while fetching the space image n°{id}. More details: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "An error occurred while fetching the space image.", Details = ex.Message });
        }
    }

    [MapToApiVersion(1)]
    [HttpGet]
    [ProducesResponseType<SpaceImageDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<IEnumerable<SpaceImageDTO>>> GetSpaceImages(int page = 1, int pageSize = 10)
    {
        _logger.LogInformation($"Getting space images from page {page} with a page size of {pageSize}.");

        if (page <= 0)
        {
            _logger.LogInformation("The space images could not be fetched because the page number is invalid.");
            return BadRequest("The page number must be greater than 0.");
        }

        if (pageSize <= 0)
        {
            _logger.LogInformation("The space images could not be fetched because the page size is invalid.");
            return BadRequest("The page size must be greater than 0.");
        }

        try
        {
            var spaceImages = (await _service.GetSpaceImages(page, pageSize)).ToDTO();
            _logger.LogInformation(
                $"The space images from page {page} with a page size of {pageSize} were fetched successfully.");
            return Ok(spaceImages);
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError(
                $"The space images could not be fetched due to an unavailable database. More details: {ex.Message}");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "The database is not available.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while fetching the space images. More details: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "An error occurred while fetching the space images.", Details = ex.Message });
        }
    }

    [MapToApiVersion(1)]
    [HttpGet("today")]
    [ProducesResponseType<SpaceImageDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<SpaceImageDTO>> GetSpaceImageOfTheDay()
    {
        _logger.LogInformation("Getting the space image of the day.");
        try
        {
            var spaceImage = await _service.GetSpaceImageOfTheDay();
            if (spaceImage is null)
            {
                _logger.LogInformation("The space image of the day was not found.");
                return NotFound("The space image of the day was not found.");
            }

            _logger.LogInformation("The space image of the day was found and fetched successfully.");
            return Ok(spaceImage.ToDTO());
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError(
                $"The space image of the day could not be fetched due to an unavailable database. More details: {ex.Message}");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "The database is not available.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                $"An error occurred while fetching the space image of the day from the API. More details: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "An error occurred while fetching the space image of the day.", Details = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"An error occurred while fetching the space image of the day. More details: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "An error occurred while fetching the space image of the day.", Details = ex.Message });
        }
    }
}