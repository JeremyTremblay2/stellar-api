using Microsoft.Extensions.Logging;
using StellarApi.Business.Exceptions;
using StellarApi.Infrastructure.Business;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;
using StellarApi.Repository.Exceptions;

namespace StellarApi.Business
{
    /// <summary>
    /// Service class for managing celestial objects.
    /// </summary>
    public class CelestialObjectService : ICelestialObjectService
    {
        /// <summary>
        /// Constant that represents the maximum length of a name.
        /// </summary>
        private const int MaxLengthName = 100;

        /// <summary>
        /// Constant that represents the maximum length of a description.
        /// </summary>
        private const int MaxLengthDescription = 1000;

        /// <summary>
        /// The repository used by this service.
        /// </summary>
        private readonly ICelestialObjectRepository _repository;

        /// <summary>
        /// The repository used for managing maps.
        /// </summary>
        private readonly IMapRepository _mapRepository;

        /// <summary>
        /// Logger used by this service.
        /// </summary>
        private readonly ILogger<CelestialObjectService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CelestialObjectService"/> class.
        /// </summary>
        /// <param name="logger">The logger used by this service.</param>
        /// <param name="repository">The repository used for accessing celestial objects.</param>
        /// <param name="mapRepository">The repository used for accessing maps.</param>
        public CelestialObjectService(ILogger<CelestialObjectService> logger, ICelestialObjectRepository repository, IMapRepository mapRepository)
        {
            _logger = logger;
            _repository = repository;
            _mapRepository = mapRepository;
        }

        /// <inheritdoc/>
        public async Task<CelestialObject?> GetCelestialObject(int id, int? userRequestId)
        {
            var celestialObject = await _repository.GetCelestialObject(id);
            if (celestialObject != null && (!celestialObject.IsPublic && (userRequestId == null || celestialObject.UserAuthorId != userRequestId)))
            {
                throw new UnauthorizedAccessException($"You are not allowed to access the celestial object n°{id} because this is not yours.");
            }
            return celestialObject;
        }

        /// <inheritdoc/>
        public Task<IEnumerable<CelestialObject>> GetCelestialObjects(int userId, int page, int pageSize)
        {
            return _repository.GetCelestialObjects(userId, page, pageSize);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<CelestialObject>> GetPublicCelestialObjects(int page, int pageSize)
        {
            return _repository.GetPublicCelestialObjects(page, pageSize);
        }

        /// <inheritdoc/>
        public async Task<bool> PostCelestialObject(CelestialObject celestialObject)
        {
            await CheckCelestialObjectData(celestialObject);
            celestialObject.CreationDate = DateTime.Now;
            celestialObject.ModificationDate = DateTime.Now;
            return await _repository.AddCelestialObject(celestialObject);
        }

        /// <inheritdoc/>
        public async Task<bool> PutCelestialObject(int id, CelestialObject celestialObject)
        {
            var existingCelestialObject = await _repository.GetCelestialObject(id);
            if (existingCelestialObject != null && existingCelestialObject.UserAuthorId != celestialObject.UserAuthorId)
            {
                throw new UnauthorizedAccessException($"You are not allowed to modify the celestial object n°{id} because this is not yours.");
            }
            await CheckCelestialObjectData(celestialObject);
            celestialObject.ModificationDate = DateTime.Now;
            return await _repository.EditCelestialObject(id, celestialObject);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteCelestialObject(int celestialObjectId, int userAuthorId)
        {
            var existingCelestialObject = await _repository.GetCelestialObject(celestialObjectId);
            if (existingCelestialObject != null && existingCelestialObject.UserAuthorId != userAuthorId)
            {
                throw new UnauthorizedAccessException($"You are not allowed to delete the celestial object n°{celestialObjectId} because this is not yours.");
            }
            return await _repository.RemoveCelestialObject(celestialObjectId);
        }

        /// <summary>
        /// Checks the data of a celestial object to ensure it is valid.
        /// </summary>
        /// <param name="celestialObject">The celestial object to check.</param>
        /// <exception cref="ArgumentNullException">If the celestial object is null.</exception>
        /// <exception cref="ArgumentException">If any of the fields are invalid.</exception>
        /// <exception cref="InvalidFieldLengthException">If any of the fields are too long.</exception>
        private async Task CheckCelestialObjectData(CelestialObject celestialObject)
        {
            if (celestialObject == null)
            {
                _logger.LogWarning("The celestial object was null while checking its data.");
                throw new ArgumentNullException(nameof(celestialObject));
            }
            if (string.IsNullOrWhiteSpace(celestialObject.Name))
            {
                throw new ArgumentException("The name cannot be null or empty.", nameof(celestialObject.Name));
            }
            if (celestialObject.Name.Length > MaxLengthName)
            {
                throw new InvalidFieldLengthException($"The name address cannot be greater than {MaxLengthName} characters.", nameof(celestialObject.Name));
            }
            if (string.IsNullOrWhiteSpace(celestialObject.Description))
            {
                throw new ArgumentException("The description cannot be null or empty.", nameof(celestialObject.Description));
            }
            if (celestialObject.Description.Length > MaxLengthDescription)
            {
                throw new InvalidFieldLengthException($"The username cannot be greater than {MaxLengthDescription} characters.", nameof(celestialObject.Description));
            }
            if (celestialObject.Mass <= 0)
            {
                throw new ArgumentException("The mass cannot be negative or null.", nameof(celestialObject.Mass));
            }
            if (celestialObject.Temperature < -273)
            {
                throw new ArgumentException("The temperature cannot be less than 273 degrees.", nameof(celestialObject.Temperature));
            }
            if (celestialObject.Radius <= 0)
            {
                throw new ArgumentException("The radius cannot be negative or null.", nameof(celestialObject.Radius));
            }
            if (celestialObject is Star star)
            {
                if (star.Brightness <= 0)
                {
                    throw new ArgumentException("The brightness cannot be negative or null.", nameof(star.Brightness));
                }
            }
            if (celestialObject.MapId == null && celestialObject.Position is not null)
            {
                throw new ArgumentException("The celestial object cannot have a position without a map.", nameof(celestialObject.Position));
            }
            if (celestialObject.MapId != null)
            {
                var map = await _mapRepository.GetMap(celestialObject.MapId.Value);
                if (map == null)
                {
                    throw new EntityNotFoundException(celestialObject.MapId.Value.ToString(), "The provided map was not found.");
                }
                if (map.UserAuthorId != celestialObject.UserAuthorId)
                {
                    throw new UnauthorizedAccessException($"You are not allowed to add a celestial object to the map n°{celestialObject.MapId} because this map is not yours.");
                }
            }
        }
    }
}
