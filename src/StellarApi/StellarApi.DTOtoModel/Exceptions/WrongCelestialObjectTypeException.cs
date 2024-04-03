using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarApi.DTOtoModel.Exceptions
{
    /// <summary>
    /// Exception thrown when an incorrect celestial object type is provided.
    /// </summary>
    public class WrongCelestialObjectTypeException : ArgumentNullException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrongCelestialObjectTypeException"/> class
        /// with a specified error message and the name of the parameter that causes this exception.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public WrongCelestialObjectTypeException(string paramName, string message)
            : base(paramName, message)
        {
        }
    }
}
