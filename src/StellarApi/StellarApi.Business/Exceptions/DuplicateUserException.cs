namespace StellarApi.Business.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a duplicate user is detected.
    /// </summary>
    public class DuplicateUserException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateUserException"/> class.
        /// </summary>
        public DuplicateUserException()
            : base("A user with the same identifier already exists.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateUserException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DuplicateUserException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateUserException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public DuplicateUserException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
