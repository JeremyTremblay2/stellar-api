using Microsoft.Extensions.Logging;
using StellarApi.Business.Exceptions;
using StellarApi.Infrastructure.Business;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;

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
        /// <param name="repository">The repository used for accessing celestial objects.</param>
        public CelestialObjectService(ILogger<CelestialObjectService> logger, ICelestialObjectRepository repository, IMapRepository mapRepository)
        {
            _logger = logger;
            _repository = repository;
            _mapRepository = mapRepository;
        }

        /// <inheritdoc/>
        public Task<CelestialObject?> GetCelestialObject(int id)
        {
            return _repository.GetCelestialObject(id);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<CelestialObject>> GetCelestialObjects(int page, int pageSize)
        {
            return _repository.GetCelestialObjects(page, pageSize);
        }

        /// <inheritdoc/>
        public Task<bool> PostCelestialObject(CelestialObject celestialObject)
        {
            CheckCelestialObjectData(celestialObject);
            celestialObject.CreationDate = DateTime.Now;
            celestialObject.ModificationDate = DateTime.Now;
            return _repository.AddCelestialObject(celestialObject);
        }

        /// <inheritdoc/>
        public Task<bool> PutCelestialObject(int id, CelestialObject celestialObject)
        {
            CheckCelestialObjectData(celestialObject);
            celestialObject.ModificationDate = DateTime.Now;
            return _repository.EditCelestialObject(id, celestialObject);
        }

        /// <inheritdoc/>
        public Task<bool> DeleteCelestialObject(int id)
        {
            return _repository.RemoveCelestialObject(id);
        }

        /// <summary>
        /// Checks the data of a celestial object to ensure it is valid.
        /// </summary>
        /// <param name="celestialObject">The celestial object to check.</param>
        /// <exception cref="ArgumentNullException">If the celestial object is null.</exception>
        /// <exception cref="ArgumentException">If any of the fields are invalid.</exception>
        /// <exception cref="InvalidFieldLengthException">If any of the fields are too long.</exception>
        private void CheckCelestialObjectData(CelestialObject celestialObject)
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
        }
    }
}
