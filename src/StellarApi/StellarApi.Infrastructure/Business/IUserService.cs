using StellarApi.Model.Users;

namespace StellarApi.Infrastructure.Business
{
    /// <summary>
    /// Represents a service for managing users.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets a user by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>A task that represents the asynchronous operation and the user with the specified unique identifier.</returns>
        Task<User> GetUserById(int id);

        /// <summary>
        /// Gets a user by its email.
        /// </summary>
        /// <param name="email">The user's mail.</param>
        /// <returns>The corresponding user.</returns>
        public Task<User?> GetUserByEmail(string email);

        /// <summary>
        /// Gets a list of users depending on the page and the page size.
        /// </summary>
        /// <param name="page">The page of the user search.</param>
        /// <param name="pageSize">The size of the page where to search.</param>
        /// <returns>A task that represents the asynchronous operation and a list of users based on the specified page and page size.</returns>
        Task<IEnumerable<User>> GetUsers(int page, int pageSize);

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <returns>A task that represents the asynchronous operation and true if the user was successfully added, false otherwise.</returns>
        Task<bool> PostUser(User user);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>A task that represents the asynchronous operation and true if the user was successfully updated, false otherwise.</returns>
        Task<bool> PutUser(User user);

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>A task that represents the asynchronous operation and true if the user was successfully deleted, false otherwise.</returns>
        Task<bool> DeleteUser(int id);
    }
}
