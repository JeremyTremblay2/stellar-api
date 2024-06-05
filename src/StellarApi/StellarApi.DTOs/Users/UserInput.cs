using System.ComponentModel.DataAnnotations;

namespace StellarApi.DTOs.Users
{
    /// <summary>
    /// Input data transfer object for a user.
    /// </summary>
    public class UserInput
    {
        /// <summary>
        /// Gets the email of the user.
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets the password of the user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="UserInput"/> class.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        public UserInput(string email, string username, string password)
        {
            Email = email;
            Username = username;
            Password = password;
        }
    }
}
