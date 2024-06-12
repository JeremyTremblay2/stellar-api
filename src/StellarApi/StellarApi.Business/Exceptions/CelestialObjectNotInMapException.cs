namespace StellarApi.Repository.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a specified celestial object is not found in the map.
    /// </summary>
    public class CelestialObjectNotInMapException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CelestialObjectNotInMapException"/> class.
        /// </summary>
        public CelestialObjectNotInMapException()
            : base("The specified celestial object was not found in the map.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CelestialObjectNotInMapException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CelestialObjectNotInMapException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CelestialObjectNotInMapException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public CelestialObjectNotInMapException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CelestialObjectNotInMapException"/> class
        /// with a specified error message and the name of the entity that caused this exception.
        /// </summary>
        /// <param name="mapId">The unique identifier of the map.</param>
        /// <param name="celestialObjectId">The unique identifier of the celestial object.</param>
        /// <param name="message">The message that describes the error.</param>
        public CelestialObjectNotInMapException(int mapId, int celestialObjectId, string message = null)
            : base($"The celestial object '{celestialObjectId}' was not found in the map '{mapId}'. {message}")
        {
        }
    }
}
