using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using StellarApi.DTOs;
using StellarApi.Infrastructure.Business;
using StellarApi.Model.Space;
using StellarApi.DTOtoModel;
using Microsoft.AspNetCore.Authorization;
using StellarApi.DTOs.Space;

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
            var result = await _service.GetCelestialObject(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result.ToDTO());
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
            var objects = (await _service.GetCelestialObjects(page, pageSize)).ToDTO();
            return Ok(objects);
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
            CelestialObject? userObject = null;
            try
            {
                userObject = celestialObject.ToModel();
                _ = userObject ?? throw new Exception("The object was not created.");
            }
            catch (ArgumentException err)
            {
                return BadRequest(err.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred during the conversion of the object. Please retry.");
            }

            if (await _service.PostCelestialObject(userObject))
            {
                return Ok($"The celestial object {celestialObject.Name} was successfully added.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred during the addition of the object. Please retry.");
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
            CelestialObject? userObject = null;
            try
            {
                userObject = celestialObject.ToModel();
                _ = userObject ?? throw new Exception("The object was not created.");
            }
            catch (ArgumentException err)
            {
                return BadRequest(err.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred during the conversion of the object. Please retry.");
            }

            if (await _service.PutCelestialObject(id, userObject))
            {
                return Ok($"The celestial object {celestialObject.Name} was successfully edited.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknown error occurred during the edition of the object. Please retry.");
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
            if (await _service.DeleteCelestialObject(id))
            {
                return Ok($"The celestial object n°{id} was successfully deleted.");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
