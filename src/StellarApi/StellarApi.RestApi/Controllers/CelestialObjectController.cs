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
        /// <param name="id">The unique identifier of the celestial object.</param>
        /// <returns>The action result containing the retrieved celestial object or a NotFound result if not found.</returns>
        [MapToApiVersion(1)]
        [HttpGet("{id}")]
        public async Task<ActionResult<CelestialObjectOutput?>> GetCelestialObjectById(int id)
        {
            _logger.LogInformation($"Retrieving celestial object n°{id}.");
            try
            {
                var result = await _service.GetCelestialObject(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result.ToDTO());
            }
            catch (UnavailableDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while fetching celestial object data.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a collection of celestial objects with pagination.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>The action result containing the collection of celestial objects.</returns>
        [MapToApiVersion(1)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CelestialObjectOutput>>> GetCelestialObjects(int page, int pageSize)
        {
            try
            {
                var objects = (await _service.GetCelestialObjects(page, pageSize)).ToDTO();
                return Ok(objects);
            }
            catch (UnavailableDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while fetching celestial object data.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Adds a new celestial object.
        /// </summary>
        /// <param name="celestialObject">The celestial object to add.</param>
        /// <returns>The action result indicating the success or failure of the operation.</returns>
        [MapToApiVersion(1)]
        [HttpPost]
        [Route("add")]
        public async Task<ActionResult> PostCelestialObject([FromBody] CelestialObjectInput celestialObject)
        {
            try
            {
                CelestialObject userObject = celestialObject.ToModel();

                var wasAdded = await _service.PostCelestialObject(userObject);
                if (wasAdded)
                {
                    return Ok("Celestial object added successfully.");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "The celestial object could not be added due to an unknown error.");
                }
            }
            catch (Exception ex) when (ex is WrongCelestialObjectTypeException ||
                                   ex is InvalidEnumArgumentException ||
                                   ex is ArgumentNullException ||
                                   ex is ArgumentException ||
                                   ex is InvalidFieldLengthException)
            {
                return BadRequest(ex.Message);
            }
            catch (UnavailableDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while adding a new celestial object.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing celestial object.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object to update.</param>
        /// <param name="celestialObject">The celestial object with updated information.</param>
        /// <returns>The action result indicating the success or failure of the operation.</returns>
        [MapToApiVersion(1)]
        [HttpPut]
        [Route("edit")]
        public async Task<ActionResult> PutCelestialObject(int id, [FromBody] CelestialObjectInput celestialObject)
        {
            try
            {
                CelestialObject userObject = celestialObject.ToModel();

                var wasEdited = await _service.PutCelestialObject(id, userObject);
                if (wasEdited)
                {
                    return Ok("Celestial object edited successfully.");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "The celestial object could not be edited due to an unknown error.");
                }
            }
            catch (Exception ex) when (ex is WrongCelestialObjectTypeException || 
                                   ex is InvalidEnumArgumentException ||  
                                   ex is ArgumentNullException ||
                                   ex is ArgumentException ||
                                   ex is InvalidFieldLengthException)
            {
                return BadRequest(ex.Message);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnavailableDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while editing the celestial object's information.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a celestial object by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object to delete.</param>
        /// <returns>The action result indicating the success or failure of the operation.</returns>
        [MapToApiVersion(1)]
        [HttpDelete]
        [Route("remove")]
        public async Task<ActionResult> DeleteCelestialObject(int id)
        {
            try
            {
                if (await _service.DeleteCelestialObject(id))
                {
                    return Ok($"The Celestial object n°{id} was successfully deleted.");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, $"The Celestial object n°{id} could not be deleted due to an unknown error.");
                }
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnavailableDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while deleting celestial object data.", Details = ex.Message });
            }
        }
    }
}
