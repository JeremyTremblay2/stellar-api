namespace StellarApi.Repository.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the database is unavailable.
    /// </summary>
    public class UnavailableDatabaseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnavailableDatabaseException"/> class.
        /// </summary>
        public UnavailableDatabaseException()
            : base("The database is currently unavailable.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnavailableDatabaseException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnavailableDatabaseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnavailableDatabaseException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public UnavailableDatabaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
