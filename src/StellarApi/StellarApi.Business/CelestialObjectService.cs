using StellarApi.Infrastructure.Business;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarApi.Business
{
    public class CelestialObjectService : ICelestialObjectService
    {
        private readonly ICelestialObjectRepository _repository;

        public CelestialObjectService(ICelestialObjectRepository repository)
        {
            _repository = repository;
        }

        public Task<bool> DeleteCelestialObject(int id)
        {
            return _repository.RemoveCelestialObject(id);
        }

        public Task<CelestialObject?> GetCelestialObject(int id)
        {
            return _repository.GetCelestialObject(id);
        }

        public Task<IEnumerable<CelestialObject>> GetCelestialObjects(int page, int pageSize)
        {
            return _repository.GetCelestialObjects(page, pageSize);
        }

        public Task<bool> PostCelestialObject(CelestialObject celestialObject)
        {
            return _repository.AddCelestialObject(celestialObject);
        }

        public Task<bool> PutCelestialObject(int id, CelestialObject celestialObject)
        {
            return _repository.EditCelestialObject(id, celestialObject);
        }
    }
}
