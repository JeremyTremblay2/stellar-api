using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using StellarApi.DTOs;
using StellarApi.Infrastructure.Business;
using StellarApi.Model.Space;
using StellarApi.DTOtoModel;

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
        private readonly ICelestialObjectService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="CelestialObjectController"/> class.
        /// </summary>
        /// <param name="service">The service for managing celestial objects.</param>
        public CelestialObjectController(ICelestialObjectService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a celestial object by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object.</param>
        /// <returns>The action result containing the retrieved celestial object or a NotFound result if not found.</returns>
        [MapToApiVersion(1)]
        [HttpGet("{id}")]
        public async Task<ActionResult<CelestialObjectDTO?>> GetCelestialObjectById(int id)
        {
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
        public async Task<ActionResult<IEnumerable<CelestialObjectDTO>>> GetCelestialObjects(int page, int pageSize)
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
        public async Task<ActionResult<bool>> PostCelestialObject(CelestialObjectDTO celestialObject)
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
        public async Task<ActionResult<bool>> PutCelestialObject(int id, CelestialObjectDTO celestialObject)
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
        public async Task<ActionResult<bool>> DeleteCelestialObject(int id)
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
