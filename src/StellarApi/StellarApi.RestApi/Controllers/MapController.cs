using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarApi.DTOs.Space;
using StellarApi.Infrastructure.Business;
using StellarApi.Model.Space;
using StellarApi.DTOtoModel;
using StellarApi.Repository.Exceptions;

namespace StellarApi.RestApi.Controllers;

/// <summary>
/// API controller for managing maps.
/// </summary>
[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/maps/")]
[Authorize(Roles = "Member, Administrator")]
public class MapController : ControllerBase
{
    /// <summary>
    /// Service used by this controller.
    /// </summary>
    private readonly IMapService _service;

    /// <summary>
    /// Logger used to log information in the controller.
    /// </summary>
    private readonly ILogger<MapController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapController"/> class.
    /// </summary>
    /// <param name="service"></param>
    public MapController(ILogger<MapController> logger, IMapService service)
    {
        _logger = logger;
        _service = service;
    }

    /// <summary>
    /// Retrieves a map by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the map to retrieve.</param>
    /// <returns>The action result containing the retrieved map or a NotFound result if not found.</returns>
    [MapToApiVersion(1)]
    [HttpGet("{id}")]
    public async Task<ActionResult<MapOutput?>> GetMapById(int id)
    {
        _logger.LogInformation($"Getting map n°{id}.");
        try
        {
            var result = await _service.GetMap(id);
            if (result == null)
            {
                _logger.LogWarning($"Map n°{id} was not found.");
                return NotFound();
            }

            _logger.LogInformation($"Map n°{id} was fetched successfully.");
            return Ok(result.ToDTO());
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError(
                $"The Map n°{id} could not be fetched due to an unavailable database. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"An error occurred while fetching the Map n°{id}. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "An unexpected error occurred while fetching the map data.", Details = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a collection of maps with pagination.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>The action result containing the collection of maps.</returns>
    [MapToApiVersion(1)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MapOutput>>> GetMaps(int page, int pageSize)
    {
        _logger.LogInformation($"Getting maps from page {page} with a page size of {pageSize}.");
        try
        {
            var maps = (await _service.GetMaps(page, pageSize)).ToDTO();
            _logger.LogInformation($"Maps from page {page} with a page size of {pageSize} were fetched successfully.");
            return Ok(maps);
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError(
                $"Maps from page {page} with a page size of {pageSize} could not be fetched due to an unavailable database. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"An error occurred while fetching the maps data. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "An unexpected error occurred while fetching the maps data.", Details = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new map.
    /// </summary>
    /// <param name="map">The map to create.</param>
    /// <returns>The action result indicating the success or failure of the creation.</returns>
    [MapToApiVersion(1)]
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult> PostMap(MapInput map)
    {
        _logger.LogInformation($"New map creation request for object: {map}.");
        try
        {
            Map newMap = map.ToModel();

            var wasAdded = await _service.PostMap(newMap);
            if (wasAdded)
            {
                _logger.LogInformation($"The Map n°{newMap.Id} was successfully created.");
                return Ok($"The Map {newMap.Name} was successfully created.");
            }
            else
            {
                _logger.LogError("The Map could not be added. An unknown error occurred.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "The Map could not be added. An unknown error occurred.");
            }
        }
        catch (Exception ex) when (ex is ArgumentNullException or ArgumentException)
        {
            _logger.LogError(
                $"The Map could not be created due to invalid data. More details: {ex.Message}.");
            return BadRequest(ex.Message);
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError(
                $"The Map could not be created due to an unavailable database. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"An unexpected error occurred while creating the Map. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "An unexpected error occurred while creating the Map.", Details = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing map.
    /// </summary>
    /// <param name="id">The unique identifier of the map to update.</param>
    /// <param name="map">The map object containing the new data.</param>
    /// <returns>The action result indicating the success or failure of the operation.</returns>
    [MapToApiVersion(1)]
    [HttpPut]
    public async Task<ActionResult> PutMap(int id, MapInput map)
    {
        _logger.LogInformation($"Update request for map n°{id} with object: {map}.");
        try
        {
            Map updatedMap = map.ToModel();
            var wasUpdated = await _service.PutMap(id, updatedMap);

            if (wasUpdated)
            {
                _logger.LogInformation($"The Map n°{id} was successfully updated.");
                return Ok($"The Map {updatedMap.Name} was successfully updated.");
            }
            else
            {
                _logger.LogError($"The Map n°{id} could not be updated due to an unknown error.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"The Map {updatedMap.Name} could not be updated due to an unknown error.");
            }
        }
        catch (Exception ex) when (ex is ArgumentNullException or ArgumentException)
        {
            _logger.LogError(
                $"The Map n°{id} could not be updated due to invalid data. More details: {ex.Message}.");
            return BadRequest(ex.Message);
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError(
                $"The Map n°{id} could not be updated due to an unavailable database. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"An unexpected error occurred while updating the Map n°{id}. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = $"An unexpected error occurred while updating the Map n°{id}.", Details = ex.Message });
        }
    }


    /// <summary>
    /// Deletes a map by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the map to delete.</param>
    /// <returns>The action result indicating the success or failure of the operation.</returns>
    [MapToApiVersion(1)]
    [HttpDelete()]
    [Route("delete")]
    public async Task<ActionResult> DeleteMap(int id)
    {
        _logger.LogInformation($"Delete request for map n°{id}.");
        try
        {
            if (await _service.DeleteMap(id))
            {
                _logger.LogInformation($"The Map n°{id} was successfully deleted.");
                return Ok($"The Map object n°{id} was successfully deleted.");
            }
            else
            {
                _logger.LogError($"The Map n°{id} could not be deleted due to an unknown error.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"The Map object n°{id} could not be deleted due to an unknown error.");
            }
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(
                $"The Map n°{id} could not be deleted because it was not found. More details: {ex.Message}.");
            return NotFound(ex.Message);
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError(
                $"The Map n°{id} could not be deleted due to an unavailable database. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"An unexpected error occurred while deleting the Map n°{id}. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = $"An unexpected error occurred while deleting the Map n°{id}.", Details = ex.Message });
        }
    }
}