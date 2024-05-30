namespace StellarApi.Business.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an email format is invalid.
    /// </summary>
    public class InvalidEmailFormatException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEmailFormatException"/> class.
        /// </summary>
        public InvalidEmailFormatException()
            : base("The email format is invalid.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEmailFormatException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidEmailFormatException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEmailFormatException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public InvalidEmailFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEmailFormatException"/> class
        /// with a specified error message, the parameter name, and a reference to the inner exception.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the current exception.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public InvalidEmailFormatException(string paramName, string message, Exception innerException)
            : base(message, paramName, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEmailFormatException"/> class
        /// with a specified error message and the parameter name.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the current exception.</param>
        /// <param name="message">The message that describes the error.</param>
        public InvalidEmailFormatException(string paramName, string message)
            : base(message, paramName)
        {
        }
    }
}
