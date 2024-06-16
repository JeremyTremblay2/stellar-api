namespace StellarApi.Repository.Exceptions;

public class SpaceImageFetchingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpaceImageFetchingException"/> class.
    /// </summary>
    public SpaceImageFetchingException()
        : base("An error occurred while fetching the space image of the day.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpaceImageFetchingException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public SpaceImageFetchingException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpaceImageFetchingException"/> class
    /// with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public SpaceImageFetchingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}