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
        /// The repository used by this service.
        /// </summary>
        private readonly ICelestialObjectRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CelestialObjectService"/> class.
        /// </summary>
        /// <param name="repository">The repository used for accessing celestial objects.</param>
        public CelestialObjectService(ICelestialObjectRepository repository)
        {
            _repository = repository;
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
            return _repository.AddCelestialObject(celestialObject);
        }

        /// <inheritdoc/>
        public Task<bool> PutCelestialObject(int id, CelestialObject celestialObject)
        {
            return _repository.EditCelestialObject(id, celestialObject);
        }

        /// <inheritdoc/>
        public Task<bool> DeleteCelestialObject(int id)
        {
            return _repository.RemoveCelestialObject(id);
        }
    }
}
