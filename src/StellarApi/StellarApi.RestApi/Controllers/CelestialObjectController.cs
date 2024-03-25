using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using StellarApi.DTOs;
using StellarApi.Infrastructure.Business;
using StellarApi.Model.Space;
using System.Collections;

namespace StellarApi.RestApi.Controllers
{
    [ApiController]
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/celestial-objects/")]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ICelestialObjectService _service;

        public CelestialObjectController(ICelestialObjectService service)
        {
            _service = service;
        }

        [MapToApiVersion(1)]
        [HttpGet("{id}")]
        public async Task<ActionResult<CelestialObject?>> GetCelestialObjectById(int id)
        {
            var result = await _service.GetCelestialObject(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [MapToApiVersion(1)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CelestialObject>>> GetCelestialObjects(int page, int pageSize)
        {
            return Ok(await _service.GetCelestialObjects(page, pageSize));
        }

        [MapToApiVersion(1)]
        [HttpPost]
        public async Task<ActionResult<bool>> PostCelestialObject(CelestialObject celestialObject)
        {
            if (await _service.PostCelestialObject(celestialObject))
            {
                return Ok($"The celestial object {celestialObject.Name} was succesfully added.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknow error occurs during the addition of the object, please retry.");
            }
        }

        [MapToApiVersion(1)]
        [HttpPut]
        public async Task<ActionResult<bool>> PutCelestialObject(int id, CelestialObject celestialObject)
        {
            if (await _service.PutCelestialObject(id, celestialObject))
            {
                return Ok($"The celestial object {celestialObject.Name} was succesfully edited.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknow error occurs during the edition of the object, please retry.");
            }
        }

        [MapToApiVersion(1)]
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteCelestialObject(int id)
        {
            if (await _service.DeleteCelestialObject(id))
            {
                return Ok($"The celestial object n°{id} was succesfully deleted.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unknow error occurs during the suppression of the object, please retry.");
            }
        }
    }
}
