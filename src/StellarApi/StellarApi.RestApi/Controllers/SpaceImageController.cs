using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using StellarApi.DTOs.Space;
using StellarApi.DTOtoModel;
using StellarApi.Infrastructure.Business;
using StellarApi.Model.Space;
using StellarApi.Repository.Exceptions;
using System.Net.Http.Headers;

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

    /// <summary>
    /// Retrieves a space image by its unique identifier.
    /// </summary>
    /// <remarks>
    ///
    /// This route is used to retrieve a space image by its unique identifier.
    ///
    /// A space image is representation of a celestial object captured by a camera or other imaging device. The space image contains information about the celestial object, such as its name, description, and shooting date.
    ///
    /// A 404 error will be returned if the map is not found.
    ///
    /// Sample request:
    ///
    ///     GET /api/v1/space-images/1
    ///
    /// </remarks>
    /// <param name="id">The unique identifier of the space image.</param>
    /// <returns>A space image with the specified unique identifier if found.</returns>
    [MapToApiVersion(1)]
    [HttpGet("{id}")]
    [ProducesResponseType<SpaceImageOutput>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<SpaceImageOutput?>> GetSpaceImage(int id)
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

    /// <summary>
    /// Retrieves a collection of space images with pagination.
    /// </summary>
    /// <remarks>
    ///
    /// This route is used to retrieve a collection of space images with pagination.
    ///
    /// A space image is representation of a celestial object captured by a camera or other imaging device. The space image contains information about the celestial object, such as its name, description, and shooting date.
    ///
    /// The page and page size parameters are mandatory and must be greater than 0.
    ///
    /// A 400 error will be returned if the page or page size is less than or equal to 0.
    ///
    /// Sample request:
    ///
    ///     GET /api/v1/space-images?page=1&amp;pageSize=10
    ///
    /// </remarks>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A collection of space images with pagination.</returns>
    [MapToApiVersion(1)]
    [HttpGet]
    [ProducesResponseType<SpaceImageOutput>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<IEnumerable<SpaceImageOutput>>> GetSpaceImages(int page = 1, int pageSize = 10)
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
            var totalSpaceImages = await _service.GetSpaceImageCount();
            var firstItemIndex = (page - 1) * pageSize;
            var lastItemIndex = firstItemIndex + spaceImages.Count() - 1;
            Response.Headers["Content-Range"] = $"space-images {firstItemIndex}-{lastItemIndex}/{totalSpaceImages}";
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

    /// <summary>
    /// Retrieves a space image by its shooting date or the current date if null.
    /// </summary>
    /// <remarks>
    ///
    /// This route is used to retrieve a space image by its shooting date or the current date if null.
    ///
    /// A space image is representation of a celestial object captured by a camera or other imaging device. The space image contains information about the celestial object, such as its name, description, and shooting date.
    ///
    /// If a date is specified, the space image of the specified date will be retrieved if found. The date parameter is optional. If null, the space image of the current date will be retrieved.
    ///
    /// A 404 error will be returned if the space image is not found.
    ///
    /// Sample requests:
    ///
    ///     GET /api/v1/space-images/date
    ///
    ///     GET /api/v1/space-images/date/2022-01-30
    ///
    /// </remarks>
    /// <param name="date">The shooting date of the space image.</param>
    /// <returns>A space image with the specified shooting date if found.</returns>
    [MapToApiVersion(1)]
    [HttpGet]
    [Route("date")]
    [Route("date/{date?}")]
    [ProducesResponseType<SpaceImageOutput>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<SpaceImageOutput>> GetSpaceImageByDate(DateTime? date = null)
    {
        _logger.LogInformation($"Getting space image of the date {DisplayDate(date)}.");
        try
        {
            var spaceImage = await _service.GetSpaceImage(date);
            if (spaceImage is null)
            {
                _logger.LogInformation($"The space image of the date {DisplayDate(date)} was not found.");
                return NotFound($"The space image of the date {DisplayDate(date)} was not found.");
            }

            _logger.LogInformation($"The space image of the date {DisplayDate(date)} was found and fetched successfully.");
            return Ok(spaceImage.ToDTO());
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError(
                $"The space image of the date {DisplayDate(date)} could not be fetched due to an unavailable database. More details: {ex.Message}");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "The database is not available.");
        }
        catch (SpaceImageFetchingException ex)
        {
            _logger.LogError(
                $"An error occurred while fetching the space image of the date {DisplayDate(date)} from the API. More details: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = $"An error occurred while fetching the space image of the date {DisplayDate(date)}.", Details = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"An error occurred while fetching the space image of the date {DisplayDate(date)}. More details: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = $"An error occurred while fetching the space image of the date {DisplayDate(date)}.", Details = ex.Message });
        }
    }


    /// <summary>
    /// Displays the date in a readable format.
    /// </summary>
    /// <param name="date">The date to display.</param>
    /// <returns>The date in a readable format.</returns>
    private string DisplayDate(DateTime? date)
    {
        return date is null ? DateTime.Now.ToString("yyyy-MM-dd") : date.Value.ToString("yyyy-MM-dd");
    }
}