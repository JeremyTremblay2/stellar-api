using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using StellarApi.DTOs;
using StellarApi.Infrastructure.Business;
using StellarApi.Model.Space;
using StellarApi.DTOtoModel;

namespace StellarApi.RestApi.Controllers;

/// <summary>
/// API controller for managing maps.
/// </summary>
[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}/maps/")]
public class MapController : ControllerBase
{
    private readonly IMapService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapController"/> class.
    /// </summary>
    /// <param name="service"></param>
    public MapController(IMapService service)
    {
        _service = service;
    }

    [MapToApiVersion(1)]
    [HttpGet("{id}")]
    public async Task<ActionResult<MapDTO?>> GetMapById(int id)
    {
        var result = await _service.GetMap(id);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result.ToDTO());
    }

    [MapToApiVersion(1)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MapDTO>>> GetMaps(int page, int pageSize)
    {
        var maps = (await _service.GetMaps(page, pageSize)).ToDTO();
        return Ok(maps);
    }

    [MapToApiVersion(1)]
    [HttpPost]
    public async Task<ActionResult> PostMap(MapDTO map)
    {
        Map? newMap = null;
        try
        {
            newMap = map.ToModel();
            _ = newMap ?? throw new ArgumentException("Invalid map data.");
        }
        catch (ArgumentException err)
        {
            return BadRequest(err.Message);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An unknown error occurred during the conversion of the object. Please retry.");
        }

        if (await _service.PostMap(newMap))
        {
            return Ok($"The Map {newMap.Name} was successfully created.");
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An unknown error occurred during the addition of the object. Please retry.");
        }
    }

    [MapToApiVersion(1)]
    [HttpPut]
    public async Task<ActionResult> PutMap(int id, MapDTO map)
    {
        Map? updatedMap = null;
        try
        {
            updatedMap = map.ToModel();
            _ = updatedMap ?? throw new ArgumentException("Invalid map data.");
        }
        catch (ArgumentException err)
        {
            return BadRequest(err.Message);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An unknown error occurred during the conversion of the object. Please retry.");
        }

        if (await _service.PutMap(id, updatedMap))
        {
            return Ok($"The Map {updatedMap.Name} was successfully updated.");
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An unknown error occurred during the update of the object. Please retry.");
        }
    }

    [MapToApiVersion(1)]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMap(int id)
    {
        if (await _service.DeleteMap(id))
        {
            return Ok($"The Map object n°{id} was successfully deleted.");
        }
        else
        {
            return NotFound();
        }
    }
}