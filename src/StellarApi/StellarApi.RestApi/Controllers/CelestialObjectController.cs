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

namespace StellarApi.RestApi.Controllers
{
    /// <summary>
    /// API controller for managing celestial objects.
    /// </summary>
    [ApiController]
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/celestial-objects/")]
    [Authorize(Roles = "Member, Administrator")]
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
        /// (Needs Auth) Retrieves a celestial object by its unique identifier.
        /// </summary>
        /// <remarks>
        /// 
        /// This route is used to retrieve a celestial object by its unique identifier.
        /// 
        /// A celestial object is a physical entity in space, such as a planet, star, etc. The fields of the celestial object are empty or not depending on its type. For example, for a Star, the brightness field is filled, while for a Planet, the isWater field is filled. Read carrefully the documentation of the schema to know which fields are filled for each type of celestial object.
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
        [ProducesResponseType<object>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<CelestialObjectOutput?>> GetCelestialObjectById(int id)
        {
            _logger.LogInformation($"Fetching celestial object data for celestial object n°{id}.");
            try
            {
                var result = await _service.GetCelestialObject(id);
                if (result == null)
                {
                    _logger.LogInformation($"Celestial object n°{id} was not found.");
                    return NotFound();
                }
                _logger.LogInformation($"The celestial object n°{id} was found and fetched successfully.");
                return Ok(result.ToDTO());
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError($"The celestial object n°{id} could not be fetched due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while fetching celestial object data. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while fetching celestial object data.", Details = ex.Message });
            }
        }

        /// <summary>
        /// (Needs Auth) Retrieves a collection of celestial objects with pagination.
        /// </summary>
        /// <remarks>
        /// 
        /// This route is used to retrieve a collection of celestial objects with pagination.
        /// 
        /// A celestial object is a physical entity in space, such as a planet, star, etc. The fields of the celestial object are empty or not depending on its type. For example, for a Star, the brightness field is filled, while for a Planet, the isWater field is filled. Read carrefully the documentation of the schema to know which fields are filled for each type of celestial object.
        /// 
        /// The page and page size parameters are mandatory and must be greater than 0.
        /// 
        /// A 400 error will be returned if the page or page size is invalid.
        /// 
        /// Sample request:
        /// 
        ///     GET /api/v1/celestial-objects?page=1&amp;pageSize=10
        /// 
        /// </remarks>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>The collection of celestial objects retrieve.</returns>
        [MapToApiVersion(1)]
        [HttpGet]
        [ProducesResponseType<IEnumerable<CelestialObjectOutput>>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<IEnumerable<CelestialObjectOutput>>> GetCelestialObjects(int page, int pageSize)
        {
            _logger.LogInformation($"Fetching celestial object data for page {page} with {pageSize} items per page.");

            if (page <= 0)
            {
                _logger.LogInformation($"Celestial object data for page {page} with {pageSize} items per page could not be fetched because the page was a negative number.");
                return BadRequest("The page number must be greater than 0.");
            }
            if (pageSize <= 0)
            {
                _logger.LogInformation($"Celestial object data for page {page} with {pageSize} items per page could not be fetched because the page size was not a negative number.");
                return BadRequest("The page size must be greater than 0.");
            }

            try
            {
                var objects = (await _service.GetCelestialObjects(page, pageSize)).ToDTO();
                _logger.LogInformation($"Celestial object data for page {page} with {pageSize} items per page was fetched successfully.");
                return Ok(objects);
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError($"Celestial object data for page {page} with {pageSize} items per page could not be fetched due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while fetching celestial object data. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while fetching celestial object data.", Details = ex.Message });
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
        /// A 400 error will be returned if the celestial object is invalid with incomplete data or invalid fields.
        /// 
        /// Sample request:
        /// 
        ///     POST /api/v1/celestial-objects/add
        ///     {
        ///         "id": 0,
        ///         "name": "Sun",
        ///         "description": "The Sun is the star at the center of the Solar System.",
        ///         "mass": 1989000000000000000000000000000,
        ///         "temperature": 5778,
        ///         "radius": 696340,
        ///         "image": "https://image_sun.png",
        ///         "position": {
        ///             "x": 20,
        ///             "y": 57,
        ///            "z": 75
        ///         },
        ///         "type": "Star",
        ///         "starType": "YellowDwarf",
        ///         "brightness": 382800000000000000000000000
        ///     }
        ///    
        /// or
        /// 
        ///     POST /api/v1/celestial-objects/add
        ///     {
        ///         "id": 0,
        ///         "name": "Earth",
        ///         "description": "The Earth is the third planet from the Sun and the only astronomical object known to harbor and support life.",
        ///         "mass": 5972200000000000000000000,
        ///         "temperature": 21,
        ///         "radius": 6371,
        ///         "image": "https://image_earth.png",
        ///         "position": {
        ///             "x": 124,
        ///             "y": 77,
        ///             "z": 4
        ///         },
        ///         "type": "Planet",
        ///         "planetType": "Terrestrial",
        ///         "isWater": true,
        ///         "isLife": true
        ///     }
        /// 
        /// </remarks>
        /// <param name="celestialObject">The celestial object to add.</param>
        /// <returns>A message indicating if the celestial object was added or not.</returns>
        [MapToApiVersion(1)]
        [HttpPost]
        [Route("add")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult> PostCelestialObject([FromBody] CelestialObjectInput celestialObject)
        {
            _logger.LogInformation($"New celestial object request for object: {celestialObject}.");
            try
            {
                CelestialObject userObject = celestialObject.ToModel();

                var wasAdded = await _service.PostCelestialObject(userObject);
                if (wasAdded)
                {
                    _logger.LogInformation($"Celestial object {celestialObject.Name} added successfully.");
                    return Ok("Celestial object added successfully.");
                }
                else
                {
                    _logger.LogError($"The celestial object could not be added due to an unknown error.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "The celestial object could not be added due to an unknown error.");
                }
            }
            catch (Exception ex) when (ex is WrongCelestialObjectTypeException ||
                                   ex is InvalidEnumArgumentException ||
                                   ex is ArgumentNullException ||
                                   ex is ArgumentException ||
                                   ex is InvalidFieldLengthException)
            {
                _logger.LogInformation($"The celestial object could not be added due to an invalid field. More details: {ex.Message}.");
                return BadRequest(ex.Message);
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError($"The celestial object could not be added due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while adding a new celestial object. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while adding a new celestial object.", Details = ex.Message });
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
        /// A 400 error will be returned if the celestial object is invalid with incomplete data or invalid fields.
        /// 
        /// A 404 error will be returned if the celestial object is not found.
        /// 
        /// Sample request:
        /// 
        ///     PUT /api/v1/celestial-objects/edit/1
        ///     {
        ///         "id": 0,
        ///         "name": "Sun",
        ///         "description": "The Sun is the star at the center of the Solar System.",
        ///         "mass": 1989000000000000000000000000000,
        ///         "temperature": 5778,
        ///         "radius": 696340,
        ///         "image": "https://image_sun.png",
        ///         "position": {
        ///             "x": 20,
        ///             "y": 57,
        ///             "z": 75
        ///         },
        ///         "type": "Star",
        ///         "starType": "YellowDwarf",
        ///         "brightness": 382800000000000000000000000
        ///     }
        ///    
        /// or
        /// 
        ///     PUT /api/v1/celestial-objects/edit/1
        ///     {
        ///         "id": 0,
        ///         "name": "Earth",
        ///         "description": "The Earth is the third planet from the Sun and the only astronomical object known to harbor and support life.",
        ///         "mass": 5972200000000000000000000,
        ///         "temperature": 21,
        ///         "radius": 6371,
        ///         "image": "https://image_earth.png",
        ///         "position": {
        ///             "x": 124,
        ///             "y": 77,
        ///             "z": 4
        ///         },
        ///         "type": "Planet",
        ///         "planetType": "Terrestrial",
        ///         "isWater": true,
        ///         "isLife": true
        ///     }
        /// 
        /// </remarks>
        /// <param name="id">The unique identifier of the celestial object to update.</param>
        /// <param name="celestialObject">The celestial object with updated information.</param>
        /// <returns>A message indicating if the celestial object was edited or not.</returns>
        [MapToApiVersion(1)]
        [HttpPut("edit/{id}")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult> PutCelestialObject(int id, [FromBody] CelestialObjectInput celestialObject)
        {
            _logger.LogInformation($"Editing celestial object n°{id} with new data: {celestialObject}.");
            try
            {
                CelestialObject userObject = celestialObject.ToModel();

                var wasEdited = await _service.PutCelestialObject(id, userObject);
                if (wasEdited)
                {
                    _logger.LogInformation($"Celestial object n°{id} edited successfully.");
                    return Ok("Celestial object edited successfully.");
                }
                else
                {
                    _logger.LogError($"The celestial object n°{id} could not be edited due to an unknown error.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "The celestial object could not be edited due to an unknown error.");
                }
            }
            catch (Exception ex) when (ex is WrongCelestialObjectTypeException || 
                                   ex is InvalidEnumArgumentException ||  
                                   ex is ArgumentNullException ||
                                   ex is ArgumentException ||
                                   ex is InvalidFieldLengthException)
            {
                _logger.LogInformation($"The celestial object n°{id} could not be edited due to an invalid field. More details: {ex.Message}.");
                return BadRequest(ex.Message);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation($"The celestial object n°{id} could not be edited because it was not found. More details: {ex.Message}.");
                return NotFound(ex.Message);
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError($"The celestial object n°{id} could not be edited due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while editing the celestial object's information. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while editing the celestial object's information.", Details = ex.Message });
            }
        }

        /// <summary>
        /// (Needs Auth) Deletes a celestial object by its unique identifier.
        /// </summary>
        /// <remarks>
        /// 
        /// This route is used to delete a celestial object by its unique identifier. The data cannot be recovered once deleted.
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
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType<string>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult> DeleteCelestialObject(int id)
        {
            _logger.LogInformation($"Deleting celestial object data for celestial object n°{id}.");
            try
            {
                if (await _service.DeleteCelestialObject(id))
                {
                    _logger.LogInformation($"The celestial object n°{id} was successfully deleted.");
                    return Ok($"The Celestial object n°{id} was successfully deleted.");
                }
                else
                {
                    _logger.LogError($"The Celestial object n°{id} could not be deleted due to an unknown error.");
                    return StatusCode(StatusCodes.Status500InternalServerError, $"The Celestial object n°{id} could not be deleted due to an unknown error.");
                }
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation($"The celestial object n°{id} could not be deleted because it was not found. More details: {ex.Message}.");
                return NotFound(ex.Message);
            }
            catch (UnavailableDatabaseException ex)
            {
                _logger.LogError($"The celestial object n°{id} could not be deleted due to an unavailable database. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while deleting celestial object data. More details: {ex.Message}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while deleting celestial object data.", Details = ex.Message });
            }
        }
    }
}
