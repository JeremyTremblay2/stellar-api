using Microsoft.EntityFrameworkCore;
using StellarApi.Infrastructure.Business;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Users;

namespace StellarApi.Business
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepository">The repository used by this service.</param>
        public UserService(IUserRepository userRepository)
        {
            this._repository = userRepository;
        }

        /// <inheritdoc/>
        public async Task<User> GetUserById(int id)
        {
            return await _repository.GetUserById(id);
        }

        /// <inheritdoc/>
        public async Task<User?> GetUserByEmail(string email)
        {
            return await _repository.GetUserByEmail(email);
        }        

        /// <inheritdoc/>
        public async Task<IEnumerable<User>> GetUsers(int page, int pageSize)
        {
            return await _repository.GetUsers(page, pageSize);
        }

        /// <inheritdoc/>
        public async Task<bool> PostUser(User user)
        {
            return await _repository.AddUser(user);
        }

        /// <inheritdoc/>
        public async Task<bool> PutUser(User user)
        {
            return await _repository.EditUser(user);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteUser(int id)
        {
            return await _repository.RemoveUser(id);
        }
    }
}
