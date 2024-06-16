using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarApi.DTOs.Space;
using StellarApi.Infrastructure.Business;
using StellarApi.Model.Space;
using StellarApi.DTOtoModel;
using StellarApi.Repository.Exceptions;
using System.Security.Claims;
using StellarApi.Business.Exceptions;
using StellarApi.Helpers;

namespace StellarApi.RestApi.Controllers;

/// <summary>
/// API controller for managing maps.
/// </summary>
[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/maps/")]
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
    /// <remarks>
    ///
    /// This route is used to retrieve a map by its unique identifier.
    ///
    /// A Map is a representation of a celestial map that contains a collection of celestial objects.
    /// 
    /// Public maps can be accessed by any user. Private maps can only be accessed by the author of the map. Only the public celestial objects in the map are returned in the map if the user is not the request author.
    /// 
    /// A 403 error will be returned if the user is not allowed to access the map.
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
    [ProducesResponseType<string>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<MapOutput?>> GetMapById(int id)
    {
        _logger.LogInformation($"Getting map n°{id}.");
        try
        {
            var userId = ClaimsParsingHelper.ParseUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId is null)
            {
                _logger.LogError("The user ID of the connected user could not be found in the claims.");
                return StatusCode(StatusCodes.Status500InternalServerError, "The user ID of the connected user could not be found in the claims, please retry to log in.");
            }
            var result = await _service.GetMap(id, userId.Value);
            if (result == null)
            {
                _logger.LogWarning($"Map n°{id} was not found.");
                return NotFound($"The map n°{id} was not found.");
            }

            _logger.LogInformation($"Map n°{id} was fetched successfully.");
            return Ok(result.ToDTO());
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogInformation($"The map could not be fetch because the user is not allowed to access it. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError($"The Map n°{id} could not be fetched due to an unavailable database. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while fetching the Map n°{id}. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while fetching the map data.", Details = ex.Message });
        }
    }

    /// <summary>
    /// (Needs Auth) Retrieves a collection of the private maps of the current connected user with pagination.
    /// </summary>
    /// <remarks>
    ///
    /// This route is used to retrieve a collection of the user's maps when he is connected.
    ///
    /// A Map is a representation of a celestial map that contains a collection of celestial objects.
    ///
    /// Only personal maps of the user are returned with this route.
    /// 
    /// The page and page size parameters are mandatory and must be greater than 0.
    ///
    /// A 400 error will be returned if the page or page size is less than or equal to 0.
    ///
    /// Sample request:
    ///
    ///     GET /api/v1/maps/personnal?page=1&amp;pageSize=10
    ///
    /// </remarks>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>The retrieved collection of map.</returns>
    [MapToApiVersion(1)]
    [HttpGet("personnal")]
    [Authorize(Roles = "Member, Administrator")]
    [ProducesResponseType<IEnumerable<MapOutput>>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<IEnumerable<MapOutput>>> GetPersonnalMaps(int page = 1, int pageSize = 10)
    {
        _logger.LogInformation($"Getting personnal maps from page {page} with a page size of {pageSize}.");

        if (page <= 0)
        {
            _logger.LogInformation($"Personnal Maps data for page {page} with {pageSize} items per page could not be fetched because the page was a negative number.");
            return BadRequest("The page number must be greater than 0.");
        }

        if (pageSize <= 0)
        {
            _logger.LogInformation($"Personnal Maps data for page {page} with {pageSize} items per page could not be fetched because the page size was not a negative number.");
            return BadRequest("The page size must be greater than 0.");
        }

        try
        {
            var userId = ClaimsParsingHelper.ParseUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId is null)
            {
                _logger.LogError("The user ID of the connected user could not be found in the claims.");
                return StatusCode(StatusCodes.Status500InternalServerError, "The user ID of the connected user could not be found in the claims, please retry to log in.");
            }
            var maps = (await _service.GetMaps(userId.Value, page, pageSize)).ToDTO();
            _logger.LogInformation($"Personnal Maps from page {page} with a page size of {pageSize} were fetched successfully.");
            return Ok(maps);
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError($"Personnal Maps from page {page} with a page size of {pageSize} could not be fetched due to an unavailable database. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while fetching the personnal maps data. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while fetching the personnal maps data.", Details = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a collection of public maps with pagination.
    /// </summary>
    /// <remarks>
    ///
    /// This route is used to retrieve a collection of public maps with pagination.
    ///
    /// A Map is a representation of a celestial map that contains a collection of celestial objects.
    /// 
    /// The personnal maps of the user are returned with this route only if they are public. To have access to all the maps of the user, use the route /api/v1/maps/personnal.
    ///
    /// The page and page size parameters are mandatory and must be greater than 0.
    ///
    /// A 400 error will be returned if the page or page size is less than or equal to 0.
    ///
    /// Sample request:
    ///
    ///     GET /api/v1/maps/public?page=1&amp;pageSize=10
    ///
    /// </remarks>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>The retrieved collection of map.</returns>
    [MapToApiVersion(1)]
    [HttpGet("public")]
    [ProducesResponseType<IEnumerable<MapOutput>>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<IEnumerable<MapOutput>>> GetPublicMaps(int page = 1, int pageSize = 10)
    {
        _logger.LogInformation($"Getting public maps from page {page} with a page size of {pageSize}.");

        if (page <= 0)
        {
            _logger.LogInformation($"Public Maps data for page {page} with {pageSize} items per page could not be fetched because the page was a negative number.");
            return BadRequest("The page number must be greater than 0.");
        }

        if (pageSize <= 0)
        {
            _logger.LogInformation($"Public Maps data for page {page} with {pageSize} items per page could not be fetched because the page size was not a negative number.");
            return BadRequest("The page size must be greater than 0.");
        }

        try
        {
            var maps = (await _service.GetMaps(page, pageSize)).ToDTO();
            _logger.LogInformation($"Public Maps from page {page} with a page size of {pageSize} were fetched successfully.");
            return Ok(maps);
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError($"Public Maps from page {page} with a page size of {pageSize} could not be fetched due to an unavailable database. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while fetching the public maps data. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while fetching the public maps data.", Details = ex.Message });
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
    ///         "isPublic": true
    ///     }
    ///
    /// </remarks>
    /// <param name="map">The map to create.</param>
    /// <returns>>A message indicating if the map was added or not.</returns>
    [MapToApiVersion(1)]
    [HttpPost("create")]
    [Authorize(Roles = "Member, Administrator")]
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

            var userId = ClaimsParsingHelper.ParseUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId is null)
            {
                _logger.LogError("The user ID of the connected user could not be found in the claims.");
                return StatusCode(StatusCodes.Status500InternalServerError, "The user ID of the connected user could not be found in the claims, please retry to log in.");
            }

            newMap.UserAuthorId = userId.Value;

            var wasAdded = await _service.PostMap(newMap);
            if (wasAdded)
            {
                _logger.LogInformation($"The Map n°{newMap.Id} was successfully created.");
                return Ok($"The Map {newMap.Name} was successfully created.");
            }
            else
            {
                _logger.LogError("The Map could not be added. An unknown error occurred.");
                return StatusCode(StatusCodes.Status500InternalServerError, "The Map could not be added. An unknown error occurred.");
            }
        }
        catch (Exception ex) when (ex is ArgumentNullException or ArgumentException)
        {
            _logger.LogInformation($"The Map could not be created due to invalid data. More details: {ex.Message}.");
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
            _logger.LogError($"An unexpected error occurred while creating the Map. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while creating the Map.", Details = ex.Message });
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
    /// To update a map, the connected user must be the author of the map.
    ///
    /// A 400 error will be returned if the map data is invalid.
    /// 
    /// A 403 error will be returned if the connected user is not the author of the map.
    ///
    /// A 404 error will be returned if the map is not found.
    ///
    /// Sample request:
    ///
    ///     PUT /api/v1/maps/edit/1
    ///     {
    ///         "name": "Edited map name",
    ///         "isPublic": false
    ///     }
    ///
    /// </remarks>
    /// <param name="id">The unique identifier of the map to update.</param>
    /// <param name="map">The map object containing the new data.</param>
    /// <returns>A message indicating if the map was updated or not.</returns>
    [MapToApiVersion(1)]
    [HttpPut("edit/{id}")]
    [Authorize(Roles = "Member, Administrator")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> PutMap(int id, [FromBody] MapInput map)
    {
        _logger.LogInformation($"Update request for map n°{id} with object: {map}.");
        try
        {
            Map updatedMap = map.ToModel();

            var userId = ClaimsParsingHelper.ParseUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId is null)
            {
                _logger.LogError("The user ID of the connected user could not be found in the claims.");
                return StatusCode(StatusCodes.Status500InternalServerError, "The user ID of the connected user could not be found in the claims, please retry to log in.");
            }

            updatedMap.UserAuthorId = userId.Value;

            var wasUpdated = await _service.PutMap(id, updatedMap);

            if (wasUpdated)
            {
                _logger.LogInformation($"The Map n°{id} was successfully updated.");
                return Ok($"The Map {updatedMap.Name} was successfully updated.");
            }
            else
            {
                _logger.LogError($"The Map n°{id} could not be updated due to an unknown error.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"The Map {updatedMap.Name} could not be updated due to an unknown error.");
            }
        }
        catch (Exception ex) when (ex is ArgumentNullException or ArgumentException)
        {
            _logger.LogInformation($"The Map n°{id} could not be updated due to invalid data. More details: {ex.Message}.");
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogInformation($"The map n°{id} could not be deleted because the user is not allowed to delete it. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation($"The Map n°{id} could not be updated because it was not found. More details: {ex.Message}.");
            return NotFound(ex.Message);
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError(
                $"The Map n°{id} could not be updated due to an unavailable database. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An unexpected error occurred while updating the Map n°{id}. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"An unexpected error occurred while updating the Map n°{id}.", Details = ex.Message });
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
    /// To delete a map, the connected user must be the author of the map.
    /// 
    /// A 403 error will be returned if the connected user is not the author of the map.
    ///
    /// A 404 error will be returned if the map is not found.
    ///
    /// Sample request:
    ///
    ///     DELETE /api/v1/maps/remove/1
    ///
    /// </remarks>
    /// <param name="id">The unique identifier of the map to delete.</param>
    /// <returns>A message indicating if the map was deleted or not.</returns>
    [MapToApiVersion(1)]
    [HttpDelete("remove/{id}")]
    [Authorize(Roles = "Member, Administrator")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> DeleteMap(int id)
    {
        _logger.LogInformation($"Delete request for map n°{id}.");
        try
        {
            var userId = ClaimsParsingHelper.ParseUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId is null)
            {
                _logger.LogError("The user ID of the connected user could not be found in the claims.");
                return StatusCode(StatusCodes.Status500InternalServerError, "The user ID of the connected user could not be found in the claims, please retry to log in.");
            }

            if (await _service.DeleteMap(id, userId.Value))
            {
                _logger.LogInformation($"The Map n°{id} was successfully deleted.");
                return Ok($"The Map object n°{id} was successfully deleted.");
            }
            else
            {
                _logger.LogError($"The Map n°{id} could not be deleted due to an unknown error.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"The Map object n°{id} could not be deleted due to an unknown error.");
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogInformation($"The map n°{id} could not be deleted because the user is not allowed to delete it. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation($"The Map n°{id} could not be deleted because it was not found. More details: {ex.Message}.");
            return NotFound(ex.Message);
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError( $"The Map n°{id} could not be deleted due to an unavailable database. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An unexpected error occurred while deleting the Map n°{id}. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"An unexpected error occurred while deleting the Map n°{id}.", Details = ex.Message });
        }
    }

    /// <summary>
    /// (Needs Auth) Adds a celestial object to a map.
    /// </summary>
    /// <remarks>
    ///
    /// This route is used to add a celestial object to a map. Once specified with the map and celestial object unique identifiers, the celestial object will be added to the map.
    /// 
    /// To add a celestial object to a map, the connected user must be the author of the map and the celestial object.
    /// 
    /// A 400 error will be returned if the celestial object is already in the map.
    /// 
    /// A 403 error will be returned if the connected user is not the author of the map or the celestial object.
    /// 
    /// A 404 error will be returned if the map or the celestial object is not found.
    /// 
    /// Sample request:
    ///
    ///     PUT /api/v1/maps/1/add/1
    ///
    /// </remarks>
    /// <param name="mapId">The unique identifier of the map.</param>
    /// <param name="celestialObjectId">The unique identifier of the celestial object to add.</param>
    /// <returns>A message indicating if the celestial object was added or not.</returns>
    [MapToApiVersion(1)]
    [HttpPut("{mapId}/add/{celestialObjectId}")]
    [Authorize(Roles = "Member, Administrator")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> AddCelestialObject(int mapId, int celestialObjectId)
    {
        _logger.LogInformation($"Add celestial object request for map n°{mapId} and celestial object n°{celestialObjectId}.");
        try
        {
            var userId = ClaimsParsingHelper.ParseUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId is null)
            {
                _logger.LogError("The user ID of the connected user could not be found in the claims.");
                return StatusCode(StatusCodes.Status500InternalServerError, "The user ID of the connected user could not be found in the claims, please retry to log in.");
            }

            if (await _service.AddCelestialObject(mapId, celestialObjectId, userId.Value))
            {
                _logger.LogInformation($"The Celestial Object n°{celestialObjectId} was successfully added to the Map n°{mapId}.");
                return Ok($"The Celestial Object n°{celestialObjectId} was successfully added to the Map n°{mapId}.");
            }
            else
            {
                _logger.LogError($"The Celestial Object n°{celestialObjectId} could not be added to the Map n°{mapId} due to an unknown error.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"The Celestial Object n°{celestialObjectId} could not be added to the Map n°{mapId} due to an unknown error.");
            }
        }
        catch (CelestialObjectAlreadyInMapException ex)
        {
            _logger.LogInformation($"The Celestial Object n°{celestialObjectId} could not be added to the Map n°{mapId} because it is already in the map. More details: {ex.Message}.");
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogInformation($"The Celestial Object n°{celestialObjectId} could not be added to the map n°{mapId} because the user is not allowed to link to it. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation($"The Celestial Object n°{celestialObjectId} could not be added to the Map n°{mapId} because it was not found. More details: {ex.Message}.");
            return NotFound(ex.Message);
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError($"The Celestial Object n°{celestialObjectId} could not be added to the Map n°{mapId} due to an unavailable database. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An unexpected error occurred while adding the Celestial Object n°{celestialObjectId} to the Map n°{mapId}. More details: {ex.Message}.");
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
    /// This route is used to remove a celestial object from a map. Once specified with the map and celestial object unique identifiers, the celestial object will be removed from the map but will always exist.
    /// 
    /// To suppress a celestial object from a map, the connected user must be the author of the map and the celestial object.
    /// 
    /// A 400 error will be returned if the celestial object is not in the map.
    /// 
    /// A 403 error will be returned if the connected user is not the author of the map or the celestial object.
    /// 
    /// A 404 error will be returned if the map or the celestial object is not found.
    ///
    /// Sample request:
    ///
    ///     PUT /api/v1/maps/1/remove/1
    ///
    /// </remarks>
    /// <param name="mapId">The unique identifier of the map.</param>
    /// <param name="celestialObjectId">The unique identifier of the celestial object to remove.</param>
    /// <returns>A message indicating if the celestial object was removed or not.</returns>
    [MapToApiVersion(1)]
    [HttpPut("{mapId}/remove/{celestialObjectId}")]
    [Authorize(Roles = "Member, Administrator")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<string>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> RemoveCelestialObject(int mapId, int celestialObjectId)
    {
        _logger.LogInformation($"Remove celestial object request for map n°{mapId} and celestial object n°{celestialObjectId}.");
        try
        {
            var userId = ClaimsParsingHelper.ParseUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId is null)
            {
                _logger.LogError("The user ID of the connected user could not be found in the claims.");
                return StatusCode(StatusCodes.Status500InternalServerError, "The user ID of the connected user could not be found in the claims, please retry to log in.");
            }
            if (await _service.RemoveCelestialObject(mapId, celestialObjectId, userId.Value))
            {
                _logger.LogInformation($"The Celestial Object n°{celestialObjectId} was successfully removed from the Map n°{mapId}.");
                return Ok($"The Celestial Object n°{celestialObjectId} was successfully removed from the Map n°{mapId}.");
            }
            else
            {
                _logger.LogError($"The Celestial Object n°{celestialObjectId} could not be removed from the Map n°{mapId} due to an unknown error.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"The Celestial Object n°{celestialObjectId} could not be removed from the Map n°{mapId} due to an unknown error.");
            }
        }
        catch (CelestialObjectNotInMapException ex)
        {
            _logger.LogInformation($"The Celestial Object n°{celestialObjectId} could not be removed from the Map n°{mapId} because it was not found in the map. More details: {ex.Message}.");
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogInformation($"The Celestial Object n°{celestialObjectId} could not be removed from the map n°{mapId} because the user is not allowed to suppress it. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation($"The Celestial Object n°{celestialObjectId} could not be removed from the Map n°{mapId} because it was not found. More details: {ex.Message}.");
            return NotFound(ex.Message);
        }
        catch (UnavailableDatabaseException ex)
        {
            _logger.LogError($"The Celestial Object n°{celestialObjectId} could not be removed from the Map n°{mapId} due to an unavailable database. More details: {ex.Message}.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An unexpected error occurred while removing the Celestial Object n°{celestialObjectId} from the Map n°{mapId}. More details: {ex.Message}.");
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