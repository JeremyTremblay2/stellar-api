using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarApi.DTOtoModel.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the type of a celestial object is incorrect.
    /// </summary>
    public class WrongCelestialObjectTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrongCelestialObjectTypeException"/> class.
        /// </summary>
        public WrongCelestialObjectTypeException()
            : base("The type of the celestial object is incorrect.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrongCelestialObjectTypeException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public WrongCelestialObjectTypeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrongCelestialObjectTypeException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public WrongCelestialObjectTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrongCelestialObjectTypeException"/> class
        /// with a specified error message and the name of the parameter that caused this exception.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="message">The message that describes the error.</param>
        public WrongCelestialObjectTypeException(string paramName, string message)
            : base($"{paramName}: {message}")
        {
        }
    }
}
