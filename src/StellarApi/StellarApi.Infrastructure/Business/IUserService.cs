﻿using StellarApi.Model.Users;

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
        Task<User?> GetUserById(int id);

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
        /// Gets the total number of users.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation and the total number of users.</returns>
        Task<int> GetUsersCount();

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <returns>A task that represents the asynchronous operation and true if the user was successfully added, false otherwise.</returns>
        Task<bool> PostUser(User user);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The unique identifier of the user to update.</param>
        /// <param name="user">The user to update.</param>
        /// <param name="userAuthorId">The unique identifier of the user who is updating the user.</param>
        /// <param name="shouldUpdateModificationDate">A boolean indicating whether the modification date should be updated.</param>
        /// <returns>A task that represents the asynchronous operation and true if the user was successfully updated, false otherwise.</returns>
        Task<bool> PutUser(int id, User user, int userAuthorId, bool shouldUpdateModificationDate);

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <param name="userAuthorId">The unique identifier of the user who is deleting the user.</param>
        /// <returns>A task that represents the asynchronous operation and true if the user was successfully deleted, false otherwise.</returns>
        Task<bool> DeleteUser(int id, int userAuthorId);
    }
}
