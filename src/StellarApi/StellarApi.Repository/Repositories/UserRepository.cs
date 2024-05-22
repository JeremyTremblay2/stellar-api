using Microsoft.EntityFrameworkCore;
using StellarApi.EntityToModel;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Users;
using StellarApi.Repository.Context;

namespace StellarApi.Repository.Repositories
{
    /// <summary>
    /// Represents a repository for managing user data.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// Adds a new user to the repository.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <returns>A boolean indicating whether the user was successfully added.</returns>
        public async Task<bool> AddUser(User user)
        {
            SpaceDbContext context = new();
            if (context.Users is null) return false;

            await context.Users.AddAsync(user.ToEntity());
            return await context.SaveChangesAsync() == 1;
        }

        /// <summary>
        /// Edits an existing user in the repository.
        /// </summary>
        /// <param name="user">The user to edit.</param>
        /// <returns>A boolean indicating whether the user was successfully edited.</returns>
        public async Task<bool> EditUser(User user)
        {
            SpaceDbContext context = new();
            if (context.Users is null) return false;
            var existingUser = await context.Users.FindAsync(user.Id);
            if (existingUser == null)
                return false;
            context.ChangeTracker.Clear();
            context.Users.Update(user.ToEntity());
            return await context.SaveChangesAsync() == 1;
        }

        /// <summary>
        /// Retrieves a user by their ID from the repository.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user with the specified ID, or null if not found.</returns>
        public async Task<User> GetUserById(int id)
        {
            SpaceDbContext context = new();
            if (context.Users is null) return null;
            return (await context.Users.FindAsync(id)).ToModel();
        }

        /// <summary>
        /// Retrieves a user by his email from the repository.
        /// </summary>
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns>The user with the specified email, or null if not found.</returns>
        public async Task<User?> GetUserByEmail(string email)
        {
            SpaceDbContext context = new();
            if (context.Users is null) return null;

            return context.Users.FirstOrDefault(u => u.Email == email)?.ToModel();
        }

        /// <summary>
        /// Retrieves a paged list of users from the repository.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of users per page.</param>
        /// <returns>A paged list of users.</returns>
        public async Task<IEnumerable<User>> GetUsers(int page, int pageSize)
        {
            SpaceDbContext context = new();
            if (context.Users is null) return new List<User>();
            return (await context.Users
                .Skip(page * pageSize)
                .Take(pageSize)
                .Select(u => u.ToModel())
                .ToListAsync());
        }

        /// <summary>
        /// Removes a user from the repository.
        /// </summary>
        /// <param name="id">The ID of the user to remove.</param>
        /// <returns>A boolean indicating whether the user was successfully removed.</returns>
        public async Task<bool> RemoveUser(int id)
        {
            SpaceDbContext context = new();
            if (context.Users is null) return false;
            var user = await context.Users.FindAsync(id);
            if (user == null)
                return false;
            context.Users.Remove(user);
            return await context.SaveChangesAsync() == 1;
        }
    }
}
