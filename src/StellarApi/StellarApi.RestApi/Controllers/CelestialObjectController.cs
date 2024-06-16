using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using StellarApi.Infrastructure.Business;
using StellarApi.Model.Space;
using StellarApi.DTOtoModel;
using Microsoft.AspNetCore.Authorization;
using StellarApi.DTOs.Space;
using StellarApi.Repository.Exceptions;
using StellarApi.Business.Exceptions;
using StellarApi.DTOtoModel.Exceptions;
using System.ComponentModel;
using System.Security.Claims;
using StellarApi.Helpers;

namespace StellarApi.RestApi.Controllers
{
    /// <summary>
    /// API controller for managing celestial objects.
    /// </summary>
    [ApiController]
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/celestial-objects/")]
    public class CelestialObjectController : ControllerBase
    {
        /// <summary>
        /// Service for managing celestial objects.
        /// </summary>
        private readonly ICelestialObjectService _service;

        /// <summary>
        /// Logger used to log information in the controller.
        /// </summary>
        private readonly ILogger<CelestialObjectController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CelestialObjectController"/> class.
        /// </summary>
        /// <param name="logger">The logger used to log information in the controller.</param>
        /// <param name="service">The service for managing celestial objects.</param>
        public CelestialObjectController(ILogger<CelestialObjectController> logger, ICelestialObjectService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// Retrieves a celestial object by its unique identifier.
        /// </summary>
        /// <remarks>
        /// 
        /// This route is used to retrieve a celestial object by its unique identifier.
        /// 
        /// A celestial object is a physical entity in space, such as a planet, star, etc. The fields of the celestial object are empty or not depending on its type. For example, for a Star, the brightness field is filled, while for a Planet, the isWater field is filled. Read carrefully the documentation of the schema to know which fields are filled for each type of celestial object.
        /// 
        /// Public celestial objects can be accessed by any user. Private celestial objects can only be accessed by the author of the celestial object.
        /// 
        /// A 403 error will be returned if the user is not allowed to access the celestial object.
        /// 
        /// A 404 error will be returned if the celestial object is not found.
        /// 
        /// Sample request:
        /// 
        ///     GET /api/v1/celestial-objects/1
        /// 
        /// </remarks>
        /// <param name="id">The unique identifier of the celestial object.</param>
        /// <returns>The retrieved celestial object.</returns>
        [MapToApiVersion(1)]
        [HttpGet("{id}")]
        [ProducesResponseType<CelestialObjectOutput>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<CelestialObjectOutput?>> GetCelestialObjectById(int id)
        {
            _logger.LogInformation($"Fetching celestial object data for celestial object n°{id}.");
            try
            {
                var userId = ClaimsParsingHelper.ParseUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userId is null)
                {
                    _logger.LogError("The user ID of the connected user could not be found in the claims.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "The user ID of the connected user could not be found in the claims, please retry to log in.");
                }

                var result = await _service.GetCelestialObject(id, userId.Value);
                if (result == null)
                {
                    _logger.LogInformation($"Celestial object n°{id} was not found.");
                    return NotFound($"The celestial object n°{id} was not found.");
                }

                _logger.LogInformation($"The celestial object n°{id} was found and fetched successfully.");
                return Ok(result.ToDTO());
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogInformation(
                    $"The celestial object could not be fetch because the user is not allowed to access it. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError(
                    $"The celestial object n°{id} could not be fetched due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"An unexpected error occurred while fetching celestial object data. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "An unexpected error occurred while fetching celestial object data.",
                        Details = ex.Message
                    });
            }
        }

        /// <summary>
        /// (Needs Auth) Retrieves a collection of the private celestial objects of the current connected user with pagination.
        /// </summary>
        /// <remarks>
        ///
        /// This route is used to retrieve a collection of the user's celestial object when he is connected.
        ///
        /// A celestial object is a physical entity in space, such as a planet, star, etc. The fields of the celestial object are empty or not depending on its type. For example, for a Star, the brightness field is filled, while for a Planet, the isWater field is filled. Read carrefully the documentation of the schema to know which fields are filled for each type of celestial object.
        ///
        /// Only personal celestial objects of the user are returned with this route.
        ///
        /// The page and page size parameters are mandatory and must be greater than 0.
        ///
        /// A 400 error will be returned if the page or page size is invalid.
        ///
        /// Sample request:
        ///
        ///     GET /api/v1/celestial-objects/personnal?page=1&amp;pageSize=10
        ///
        /// </remarks>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>The collection of celestial objects retrieve.</returns>
        [MapToApiVersion(1)]
        [HttpGet("personnal")]
        [Authorize(Roles = "Member, Administrator")]
        [ProducesResponseType<IEnumerable<CelestialObjectOutput>>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<IEnumerable<CelestialObjectOutput>>> GetPersonnalCelestialObjects(int page = 1,
            int pageSize = 10)
        {
            _logger.LogInformation(
                $"Fetching personnal celestial object data for page {page} with {pageSize} items per page.");

            if (page <= 0)
            {
                _logger.LogInformation(
                    $"Personnal Celestial object data for page {page} with {pageSize} items per page could not be fetched because the page was a negative number.");
                return BadRequest("The page number must be greater than 0.");
            }

            if (pageSize <= 0)
            {
                _logger.LogInformation(
                    $"Personnal Celestial object data for page {page} with {pageSize} items per page could not be fetched because the page size was not a negative number.");
                return BadRequest("The page size must be greater than 0.");
            }

            try
            {
                var userId = ClaimsParsingHelper.ParseUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userId is null)
                {
                    _logger.LogError("The user ID of the connected user could not be found in the claims.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "The user ID of the connected user could not be found in the claims, please retry to log in.");
                }

                var objects = (await _service.GetCelestialObjects(userId.Value, page, pageSize)).ToDTO();
                _logger.LogInformation(
                    $"Personnal Celestial object data for page {page} with {pageSize} items per page was fetched successfully.");
                return Ok(objects);
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError(
                    $"Personnal Celestial object data for page {page} with {pageSize} items per page could not be fetched due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"An unexpected error occurred while fetching personnal celestial object data. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "An unexpected error occurred while fetching personnal celestial object data.",
                        Details = ex.Message
                    });
            }
        }

        /// <summary>
        /// Retrieves a collection of public celestial objects with pagination.
        /// </summary>
        /// <remarks>
        ///
        /// This route is used to retrieve a collection of public celestial objects with pagination.
        ///
        /// A celestial object is a physical entity in space, such as a planet, star, etc. The fields of the celestial object are empty or not depending on its type. For example, for a Star, the brightness field is filled, while for a Planet, the isWater field is filled. Read carrefully the documentation of the schema to know which fields are filled for each type of celestial object.
        ///
        /// The personnal celestial objects of the user are returned with this route only if they are public. To have access to all the celestial objects of the user, use the route /api/v1/celestial-objects/personnal.
        ///
        /// The page and page size parameters are mandatory and must be greater than 0.
        ///
        /// A 400 error will be returned if the page or page size is invalid.
        ///
        /// Sample request:
        ///
        ///     GET /api/v1/celestial-objects/public?page=1&amp;pageSize=10
        ///
        /// </remarks>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>The collection of celestial objects retrieve.</returns>
        [MapToApiVersion(1)]
        [HttpGet("public")]
        [ProducesResponseType<IEnumerable<CelestialObjectOutput>>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<IEnumerable<CelestialObjectOutput>>> GetPublicCelestialObjects(int page = 1,
            int pageSize = 10)
        {
            _logger.LogInformation(
                $"Fetching public celestial object data for page {page} with {pageSize} items per page.");

            if (page <= 0)
            {
                _logger.LogInformation(
                    $"Public Celestial object data for page {page} with {pageSize} items per page could not be fetched because the page was a negative number.");
                return BadRequest("The page number must be greater than 0.");
            }

            if (pageSize <= 0)
            {
                _logger.LogInformation(
                    $"Public Celestial object data for page {page} with {pageSize} items per page could not be fetched because the page size was not a negative number.");
                return BadRequest("The page size must be greater than 0.");
            }

            try
            {
                var objects = (await _service.GetPublicCelestialObjects(page, pageSize)).ToDTO();
                _logger.LogInformation(
                    $"Public Celestial object data for page {page} with {pageSize} items per page was fetched successfully.");
                return Ok(objects);
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError(
                    $"Public Celestial object data for page {page} with {pageSize} items per page could not be fetched due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"An unexpected error occurred while fetching public celestial object data. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "An unexpected error occurred while fetching public celestial object data.",
                        Details = ex.Message
                    });
            }
        }

        /// <summary>
        /// (Needs Auth) Adds a new celestial object.
        /// </summary>
        /// <remarks>
        ///
        /// This route is used to add a new celestial object.
        ///
        /// A celestial object is a physical entity in space, such as a planet, star, etc. The fields of the celestial object are empty or not depending on its type. For example, for a Star, the brightness field is filled, while for a Planet, the isWater field is filled. Read carrefully the documentation of the schema to know which fields must be filled for each type of celestial object.
        ///
        /// The name of the celestial object should not be empty and should be less than 100 characters. The description should not be empty and should be less than 1000 characters. The mass, temperature, and radius should be positive numbers. The type should be a valid type of celestial object. The star type should be a valid type of star. The planet type should be a valid type of planet. The brightness should be a positive number. The isWater and isLife should be booleans.
        ///
        /// A Map id can be provided to link the celestial object to an existing map. If the map id is not provided, the celestial object will not be linked to any map. The position of the celestial object can be provided if a map id is provided.
        ///
        /// A 400 error will be returned if the celestial object is invalid with incomplete data or invalid fields.
        ///
        /// A 403 error will be returned if the user try to add the celestial object to a map that he is not the author.
        ///
        /// A 404 error will be returned if the provided map is not found.
        ///
        /// Sample request:
        ///
        ///     POST /api/v1/celestial-objects/create
        ///     {
        ///         "name": "Sun",
        ///         "description": "The Sun is the star at the center of the Solar System.",
        ///         "mass": 1989000000000000000000000000000,
        ///         "temperature": 5778,
        ///         "radius": 696340,
        ///         "image": "https://image_sun.png",
        ///         "isPublic": true,
        ///         "position": {
        ///             "x": 20,
        ///             "y": 57,
        ///            "z": 75
        ///         },
        ///         "type": "Star",
        ///         "starType": "YellowDwarf",
        ///         "brightness": 382800000000000000000000000,
        ///         "mapId": 1
        ///     }
        ///
        /// or
        ///
        ///     POST /api/v1/celestial-objects/create
        ///     {
        ///         "name": "Earth",
        ///         "description": "The Earth is the third planet from the Sun and the only astronomical object known to harbor and support life.",
        ///         "mass": 5972200000000000000000000,
        ///         "temperature": 21,
        ///         "radius": 6371,
        ///         "image": "https://image_earth.png",
        ///         "type": "Planet",
        ///         "planetType": "Terrestrial",
        ///         "isWater": true,
        ///         "isLife": true,
        ///         "isPublic": false
        ///     }
        ///
        /// </remarks>
        /// <param name="celestialObject">The celestial object to add.</param>
        /// <returns>A message indicating if the celestial object was added or not.</returns>
        [MapToApiVersion(1)]
        [HttpPost("create")]
        [Authorize(Roles = "Member, Administrator")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<string>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult> PostCelestialObject([FromBody] CelestialObjectInput celestialObject)
        {
            _logger.LogInformation($"New celestial object request for object: {celestialObject}.");
            try
            {
                CelestialObject userObject = celestialObject.ToModel();

                var userId = ClaimsParsingHelper.ParseUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userId is null)
                {
                    _logger.LogError("The user ID of the connected user could not be found in the claims.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "The user ID of the connected user could not be found in the claims, please retry to log in.");
                }

                userObject.UserAuthorId = userId.Value;
                var wasAdded = await _service.PostCelestialObject(userObject);
                if (wasAdded)
                {
                    _logger.LogInformation($"Celestial object {celestialObject.Name} added successfully.");
                    return Ok("Celestial object added successfully.");
                }
                else
                {
                    _logger.LogError($"The celestial object could not be added due to an unknown error.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "The celestial object could not be added due to an unknown error.");
                }
            }
            catch (Exception ex) when (ex is WrongCelestialObjectTypeException or InvalidEnumArgumentException
                                           or ArgumentNullException or ArgumentException or InvalidFieldLengthException)
            {
                _logger.LogInformation(
                    $"The celestial object could not be added due to an invalid field. More details: {ex.Message}.");
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogInformation(
                    $"The celestial object could not be added because the user is not allowed to delete it. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(
                    $"The celestial object could not be edited because the map {celestialObject.MapId} was not found. More details: {ex.Message}.");
                return NotFound(ex.Message);
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError(
                    $"The celestial object could not be added due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"An unexpected error occurred while adding a new celestial object. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "An unexpected error occurred while adding a new celestial object.",
                        Details = ex.Message
                    });
            }
        }

        /// <summary>
        /// (Needs Auth) Updates an existing celestial object.
        /// </summary>
        /// <remarks>
        /// This route is used to edit an existing celestial object.
        ///
        /// A celestial object is a physical entity in space, such as a planet, star, etc. The fields of the celestial object are empty or not depending on its type. For example, for a Star, the brightness field is filled, while for a Planet, the isWater field is filled. Read carrefully the documentation of the schema to know which fields must be filled for each type of celestial object.
        ///
        /// The name of the celestial object should not be empty and should be less than 100 characters. The description should not be empty and should be less than 1000 characters. The mass, temperature, and radius should be positive numbers. The type should be a valid type of celestial object. The star type should be a valid type of star. The planet type should be a valid type of planet. The brightness should be a positive number. The isWater and isLife should be booleans.
        ///
        /// All the fields of the celestial object will be updated except for the ID. The fields that are not present in the request will be set to null.
        ///
        /// A Map id can be provided to link the celestial object to an existing map. If the map id is not provided, the celestial object will not be linked to any map. The position of the celestial object can be provided if a map id is provided. If the celestial object was already linked to a map, it will be unlinked from the previous map and linked to the new map.
        ///
        /// To edit a celestial object, the connected user must be the author of the celestial object.
        ///
        /// A 400 error will be returned if the celestial object is invalid with incomplete data or invalid fields.
        ///
        /// A 403 error will be returned if the user is not allowed to edit the celestial object.
        ///
        /// A 404 error will be returned if the celestial object is not found.
        ///
        /// Sample request:
        ///
        ///     PUT /api/v1/celestial-objects/edit/1
        ///     {
        ///         "name": "Sun",
        ///         "description": "The Sun is the star at the center of the Solar System.",
        ///         "mass": 1989000000000000000000000000000,
        ///         "temperature": 5778,
        ///         "radius": 696340,
        ///         "image": "https://image_sun.png",
        ///         "isPublic": true,
        ///         "position": {
        ///             "x": 20,
        ///             "y": 57,
        ///             "z": 75
        ///         },
        ///         "type": "Star",
        ///         "starType": "YellowDwarf",
        ///         "brightness": 382800000000000000000000000,
        ///         "mapId": 1
        ///     }
        ///
        /// or
        ///
        ///     PUT /api/v1/celestial-objects/edit/1
        ///     {
        ///         "name": "Earth",
        ///         "description": "The Earth is the third planet from the Sun and the only astronomical object known to harbor and support life.",
        ///         "mass": 5972200000000000000000000,
        ///         "temperature": 21,
        ///         "radius": 6371,
        ///         "image": "https://image_earth.png",
        ///         "type": "Planet",
        ///         "planetType": "Terrestrial",
        ///         "isWater": true,
        ///         "isLife": true,
        ///         "isPublic": true
        ///     }
        ///
        /// </remarks>
        /// <param name="id">The unique identifier of the celestial object to update.</param>
        /// <param name="celestialObject">The celestial object with updated information.</param>
        /// <returns>A message indicating if the celestial object was edited or not.</returns>
        [MapToApiVersion(1)]
        [HttpPut("edit/{id}")]
        [Authorize(Roles = "Member, Administrator")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<string>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult> PutCelestialObject(int id, [FromBody] CelestialObjectInput celestialObject)
        {
            _logger.LogInformation($"Editing celestial object n°{id} with new data: {celestialObject}.");
            try
            {
                CelestialObject userObject = celestialObject.ToModel();

                var userId = ClaimsParsingHelper.ParseUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userId is null)
                {
                    _logger.LogError("The user ID of the connected user could not be found in the claims.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "The user ID of the connected user could not be found in the claims, please retry to log in.");
                }

                userObject.UserAuthorId = userId.Value;

                var wasEdited = await _service.PutCelestialObject(id, userObject);
                if (wasEdited)
                {
                    _logger.LogInformation($"Celestial object n°{id} edited successfully.");
                    return Ok("Celestial object edited successfully.");
                }
                else
                {
                    _logger.LogError($"The celestial object n°{id} could not be edited due to an unknown error.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "The celestial object could not be edited due to an unknown error.");
                }
            }
            catch (Exception ex) when (ex is WrongCelestialObjectTypeException ||
                                       ex is InvalidEnumArgumentException ||
                                       ex is ArgumentNullException ||
                                       ex is ArgumentException ||
                                       ex is InvalidFieldLengthException)
            {
                _logger.LogInformation(
                    $"The celestial object n°{id} could not be edited due to an invalid field. More details: {ex.Message}.");
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogInformation(
                    $"The celestial object n°{id} could not be edited because the user is not allowed to delete it. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(
                    $"The celestial object n°{id} could not be edited because it was not found. More details: {ex.Message}.");
                return NotFound(ex.Message);
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError(
                    $"The celestial object n°{id} could not be edited due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"An unexpected error occurred while editing the celestial object's information. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "An unexpected error occurred while editing the celestial object's information.",
                        Details = ex.Message
                    });
            }
        }

        /// <summary>
        /// (Needs Auth) Deletes a celestial object by its unique identifier.
        /// </summary>
        /// <remarks>
        ///
        /// This route is used to delete a celestial object by its unique identifier. The data cannot be recovered once deleted.
        ///
        /// To delete a celestial object, the connected user must be the author of the celestial object.
        ///
        /// A 403 error will be returned if the user is not allowed to delete the celestial object.
        ///
        /// A 404 error will be returned if the celestial object is not found.
        ///
        /// Sample request:
        ///
        ///     DELETE /api/v1/celestial-objects/remove/1
        ///
        /// </remarks>
        /// <param name="id">The unique identifier of the celestial object to delete.</param>
        /// <returns>A message indicating if the celestial object was deleted or not.</returns>
        [MapToApiVersion(1)]
        [HttpDelete("remove/{id}")]
        [Authorize(Roles = "Member, Administrator")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult> DeleteCelestialObject(int id)
        {
            _logger.LogInformation($"Deleting celestial object data for celestial object n°{id}.");
            try
            {
                var userId = ClaimsParsingHelper.ParseUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (userId is null)
                {
                    _logger.LogError("The user ID of the connected user could not be found in the claims.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "The user ID of the connected user could not be found in the claims, please retry to log in.");
                }

                if (await _service.DeleteCelestialObject(id, userId.Value))
                {
                    _logger.LogInformation($"The celestial object n°{id} was successfully deleted.");
                    return Ok($"The Celestial object n°{id} was successfully deleted.");
                }
                else
                {
                    _logger.LogError($"The Celestial object n°{id} could not be deleted due to an unknown error.");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        $"The Celestial object n°{id} could not be deleted due to an unknown error.");
                }
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(
                    $"The celestial object n°{id} could not be deleted because it was not found. More details: {ex.Message}.");
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogInformation(
                    $"The celestial object n°{id} could not be deleted because the user is not allowed to delete it. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError(
                    $"The celestial object n°{id} could not be deleted due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"An unexpected error occurred while deleting celestial object data. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "An unexpected error occurred while deleting celestial object data.",
                        Details = ex.Message
                    });
            }
        }
    }
}