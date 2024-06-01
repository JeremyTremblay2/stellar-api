namespace StellarApi.Repository.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a specified entity is not found.
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// </summary>
        public EntityNotFoundException()
            : base("The specified entity was not found.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class
        /// with a specified error message and the name of the entity that caused this exception.
        /// </summary>
        /// <param name="entityName">The name of the entity that was not found.</param>
        /// <param name="message">The message that describes the error.</param>
        public EntityNotFoundException(string entityName, string message)
            : base($"{entityName}: {message}")
        {
        }
    }
}
