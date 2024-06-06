using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarApi.DTOs.Space
{
    /// <summary>
    /// Represents the types of the celestial object in space.
    /// </summary>
    public enum CelestialObjectType : int
    {
        /// <summary>
        /// Represents a star.
        /// </summary>
        Star = 1,
        /// <summary>
        /// Represents a planet.
        /// </summary>
        Planet = 2
    }
}
