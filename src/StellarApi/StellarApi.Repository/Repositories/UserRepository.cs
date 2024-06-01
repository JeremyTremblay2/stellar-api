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
        /// Represents the database context for managing user data.
        /// </summary>
        private readonly SpaceDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="context">The database context for managing user data.</param>
        public UserRepository(SpaceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new user to the repository.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <returns>A boolean indicating whether the user was successfully added.</returns>
        public async Task<bool> AddUser(User user)
        {
            if (_context.Users is null) return false;

            await _context.Users.AddAsync(user.ToEntity());
            return await _context.SaveChangesAsync() == 1;
        }

        /// <summary>
        /// Edits an existing user in the repository.
        /// </summary>
        /// <param name="user">The user to edit.</param>
        /// <returns>A boolean indicating whether the user was successfully edited.</returns>
        public async Task<bool> EditUser(User user)
        {
            if (_context.Users is null) return false;
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
                return false;
            var entity = user.ToEntity();
            existingUser.Email = entity.Email;
            existingUser.Username = entity.Username;
            existingUser.Password = entity.Password;
            existingUser.Role = entity.Role;
            existingUser.RefreshToken = entity.RefreshToken;
            existingUser.RefreshTokenExpiryTime = entity.RefreshTokenExpiryTime;
            existingUser.CreationDate = entity.CreationDate;
            existingUser.ModificationDate = entity.ModificationDate;
            _context.Users.Update(existingUser);
            return await _context.SaveChangesAsync() == 1;
        }

        /// <summary>
        /// Retrieves a user by their ID from the repository.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user with the specified ID, or null if not found.</returns>
        public async Task<User> GetUserById(int id)
        {
            if (_context.Users is null) return null;
            return (await _context.Users.FindAsync(id)).ToModel();
        }

        /// <summary>
        /// Retrieves a user by his email from the repository.
        /// </summary>
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns>The user with the specified email, or null if not found.</returns>
        public async Task<User?> GetUserByEmail(string email)
        {
            if (_context.Users is null) return null;
            return _context.Users.FirstOrDefault(u => u.Email == email)?.ToModel();
        }

        /// <summary>
        /// Retrieves a paged list of users from the repository.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of users per page.</param>
        /// <returns>A paged list of users.</returns>
        public async Task<IEnumerable<User>> GetUsers(int page, int pageSize)
        {
            if (_context.Users is null) return new List<User>();
            return (await _context.Users
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
            if (_context.Users is null) return false;
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;
            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() == 1;
        }
    }
}
