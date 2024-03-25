using StellarApi.Model.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarApi.Infrastructure.Business
{
    public interface ICelestialObjectService
    {
        Task<CelestialObject?> GetCelestialObject(int id);

        Task<IEnumerable<CelestialObject>> GetCelestialObjects(int page, int pageSize);

        Task<bool> PostCelestialObject(CelestialObject celestialObject);

        Task<bool> PutCelestialObject(int id, CelestialObject celestialObject);

        Task<bool> DeleteCelestialObject(int id);
    }
}
