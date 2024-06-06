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
    /// (Needs Auth) Retrieves a map by its unique identifier.
    /// </summary>
    /// <remarks>
    ///
    /// This route is used to retrieve a map by its unique identifier.
    ///
    /// A Map is a representation of a celestial map that contains a collection of celestial objects.
    ///
    /// A 404 error will be returned if the map is not found.
    ///
    /// Sample request:
    ///
    ///     GET /api/v1/maps/1
    ///
    /// </remarks>
    /// <param name="id">The unique identifier of the map to retrieve.</param>
    /// <returns>The retrieved map.</returns>
    [MapToApiVersion(1)]
    [HttpGet("{id}")]
    [ProducesResponseType<MapOutput>(StatusCodes.Status200OK)]
    [ProducesResponseType<object>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
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
    /// (Need Auth) Retrieves a collection of maps with pagination.
    /// </summary>
    /// <remarks>
    ///
    /// This route is used to retrieve a collection of maps with pagination.
    ///
    /// A Map is a representation of a celestial map that contains a collection of celestial objects.
    ///
    /// The page and page size parameters are mandatory and must be greater than 0.
    ///
    /// A 400 error will be returned if the page or page size is less than or equal to 0.
    ///
    /// Sample request:
    ///
    ///     GET /api/v1/maps?page=1&pageSize=10
    ///
    /// </remarks>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>The retrieved collection of map.</returns>
    [MapToApiVersion(1)]
    [HttpGet]
    [ProducesResponseType<IEnumerable<MapOutput>>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<IEnumerable<MapOutput>>> GetMaps(int page, int pageSize)
    {
        _logger.LogInformation($"Getting maps from page {page} with a page size of {pageSize}.");

        if (page <= 0)
        {
            _logger.LogInformation(
                $"Maps data for page {page} with {pageSize} items per page could not be fetched because the page was a negative number.");
            return BadRequest("The page number must be greater than 0.");
        }

        if (pageSize <= 0)
        {
            _logger.LogInformation(
                $"Maps data for page {page} with {pageSize} items per page could not be fetched because the page size was not a negative number.");
            return BadRequest("The page size must be greater than 0.");
        }

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
    /// (Needs Auth) Creates a new map.
    /// </summary>
    /// <remarks>
    ///
    /// This route is used to create a new map.
    ///
    /// A Map is a representation of a celestial map that contains a collection of celestial objects.
    ///
    /// The name of the map should not be null or empty and should be less than 100 characters.
    ///
    /// A 400 error will be returned if the map data is invalid.
    ///
    /// Sample request:
    ///
    ///     POST /api/v1/maps/create
    ///     {
    ///         "name": "Map name",
    ///     }
    ///
    /// </remarks>
    /// <param name="map">The map to create.</param>
    /// <returns>>A message indicating if the map was added or not.</returns>
    [MapToApiVersion(1)]
    [HttpPost("create")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> PostMap([FromBody] MapInput map)
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
    /// (Needs Auth) Updates an existing map.
    /// </summary>
    /// <remarks>
    ///
    /// This route is used to update an existing map.
    ///
    /// A map is a representation of a celestial map that contains a collection of celestial objects.
    ///
    /// The name of the map should not be null or empty and should be less than 100 characters.
    ///
    /// A 400 error will be returned if the map data is invalid.
    ///
    /// A 404 error will be returned if the map is not found.
    ///
    /// Sample request:
    ///
    ///     PUT /api/v1/maps/edit/1
    ///     {
    ///         "name": "Edited map name",
    ///     }
    ///
    /// </remarks>
    /// <param name="id">The unique identifier of the map to update.</param>
    /// <param name="map">The map object containing the new data.</param>
    /// <returns>A message indicating if the map was updated or not.</returns>
    [MapToApiVersion(1)]
    [HttpPut("edit/{id}")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> PutMap(int id, [FromBody] MapInput map)
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
    /// (Needs Auth) Deletes a map by its unique identifier.
    /// </summary>
    /// <remarks>
    ///
    /// This route is used to delete a map by its unique identifier.
    ///
    /// A map is a representation of a celestial map that contains a collection of celestial objects.
    ///
    /// A 404 error will be returned if the map is not found.
    ///
    /// Sample request:
    ///
    ///     DELETE /api/v1/maps/delete/1
    ///
    /// </remarks>
    /// <param name="id">The unique identifier of the map to delete.</param>
    /// <returns>A message indicating if the map was deleted or not.</returns>
    [MapToApiVersion(1)]
    [HttpDelete("delete/{id}")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
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

    /// <summary>
    /// (Needs Auth) Adds a celestial object to a map.
    /// </summary>
    /// <remarks>
    ///
    /// This route is used to add a celestial object to a map.
    ///
    /// A map is a representation of a celestial map that contains a collection of celestial objects.
    ///
    /// A 404 error will be returned if the map or the celestial object is not found.
    ///
    /// Sample request:
    ///
    ///     POST /api/v1/maps/1/add/1
    ///
    /// </remarks>
    /// <param name="mapId">The unique identifier of the map.</param>
    /// <param name="celestialObjectId">The unique identifier of the celestial object to add.</param>
    /// <returns>A message indicating if the celestial object was added or not.</returns>
    [MapToApiVersion(1)]
    [HttpPost("{mapId}/add/{celestialObjectId}")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> AddCelestialObject(int mapId, int celestialObjectId)
    {
        _logger.LogInformation(
            $"Add celestial object request for map n°{mapId} and celestial object n°{celestialObjectId}.");
        try
        {
            if (await _service.AddCelestialObject(mapId, celestialObjectId))
            {
                _logger.LogInformation(
                    $"The Celestial Object n°{celestialObjectId} was successfully added to the Map n°{mapId}.");
                return Ok($"The Celestial Object n°{celestialObjectId} was successfully added to the Map n°{mapId}.");
            }
            else
            {
                _logger.LogError(
                    $"The Celestial Object n°{celestialObjectId} could not be added to the Map n°{mapId} due to an unknown error.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"The Celestial Object n°{celestialObjectId} could not be added to the Map n°{mapId} due to an unknown error.");
            }
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogError(
                $"The Celestial Object n°{celestialObjectId} could not be added to the Map n°{mapId} because it was not found. More details: {ex.Message}.");
            return NotFound(ex.Message);
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError(
                $"The Celestial Object n°{celestialObjectId} could not be added to the Map n°{mapId} due to an unavailable database. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"An unexpected error occurred while adding the Celestial Object n°{celestialObjectId} to the Map n°{mapId}. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new
                {
                    Message =
                        $"An unexpected error occurred while adding the Celestial Object n°{celestialObjectId} to the Map n°{mapId}.",
                    Details = ex.Message
                });
        }
    }

    /// <summary>
    /// (Needs Auth) Removes a celestial object from a map.
    /// </summary>
    /// <remarks>
    ///
    /// This route is used to remove a celestial object from a map.
    ///
    /// A map is a representation of a celestial map that contains a collection of celestial objects.
    ///
    /// Sample request:
    ///
    ///     DELETE /api/v1/maps/1/remove/1
    ///
    /// </remarks>
    /// <param name="mapId">The unique identifier of the map.</param>
    /// <param name="celestialObjectId">The unique identifier of the celestial object to remove.</param>
    /// <returns>A message indicating if the celestial object was removed or not.</returns>
    [MapToApiVersion(1)]
    [HttpDelete("{mapId}/remove/{celestialObjectId}")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> RemoveCelestialObject(int mapId, int celestialObjectId)
    {
        _logger.LogInformation(
            $"Remove celestial object request for map n°{mapId} and celestial object n°{celestialObjectId}.");
        try
        {
            if (await _service.RemoveCelestialObject(mapId, celestialObjectId))
            {
                _logger.LogInformation(
                    $"The Celestial Object n°{celestialObjectId} was successfully removed from the Map n°{mapId}.");
                return Ok(
                    $"The Celestial Object n°{celestialObjectId} was successfully removed from the Map n°{mapId}.");
            }
            else
            {
                _logger.LogError(
                    $"The Celestial Object n°{celestialObjectId} could not be removed from the Map n°{mapId} due to an unknown error.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"The Celestial Object n°{celestialObjectId} could not be removed from the Map n°{mapId} due to an unknown error.");
            }
        }
        catch (CelestialObjectNotInMapException ex)
        {
            _logger.LogError(
                $"The Celestial Object n°{celestialObjectId} could not be removed from the Map n°{mapId} because it was not found in the map. More details: {ex.Message}.");
            return BadRequest(ex.Message);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogError(
                $"The Celestial Object n°{celestialObjectId} could not be removed from the Map n°{mapId} because it was not found. More details: {ex.Message}.");
            return NotFound(ex.Message);
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError(
                $"The Celestial Object n°{celestialObjectId} could not be removed from the Map n°{mapId} due to an unavailable database. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"An unexpected error occurred while removing the Celestial Object n°{celestialObjectId} from the Map n°{mapId}. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new
                {
                    Message =
                        $"An unexpected error occurred while removing the Celestial Object n°{celestialObjectId} from the Map n°{mapId}.",
                    Details = ex.Message
                });
        }
    }
}