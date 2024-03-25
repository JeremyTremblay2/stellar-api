using StellarApi.Model.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarApi.Infrastructure.Repository
{
    public interface ICelestialObjectRepository
    {
        Task<CelestialObject?> GetCelestialObject(int id);

        Task<IEnumerable<CelestialObject>> GetCelestialObjects(int page, int pageSize);

        Task<bool> AddCelestialObject(CelestialObject celestialObject);

        Task<bool> EditCelestialObject(int id, CelestialObject celestialObject);

        Task<bool> RemoveCelestialObject(int id);
    }
}
