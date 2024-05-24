using StellarApi.Model.Users;
using StellarApi.Entities;

namespace StellarApi.EntityToModel
{
    /// <summary>
    /// Extension methods for converting between <see cref="User"/> and <see cref="UserEntity"/>.
    /// </summary>
    public static class UserExtensions
    {
        /// <summary>
        /// Converts a <see cref="UserEntity"/> to a <see cref="User"/>.
        /// </summary>
        /// <param name="entity">The Entity to convert.</param>
        /// <returns>The new User object.</returns>
        public static User ToModel(this UserEntity? entity)
        {
            if (entity is null)
                return null;

            if (!Enum.TryParse(entity.Role, out Role role))
                throw new ArgumentException("Invalid role value.");

            return new User(entity.Id, entity.Email, entity.Username, entity.Password, role, entity.RefreshToken, 
                entity.RefreshTokenExpiryTime, entity.CreationDate, entity.ModificationDate);
        }

        /// <summary>
        /// Converts a <see cref="User"/> to a <see cref="UserEntity"/>.
        /// </summary>
        /// <param name="model">The model to convert.</param>
        /// <returns>The new entity User object.</returns>
        public static UserEntity ToEntity(this User? model)
            => model is null ? null : new UserEntity
            {
                Id = model.Id,
                Email = model.Email,
                Username = model.Username,
                Password = model.Password,
                Role = model.Role.ToString(),
                RefreshToken = model.RefreshToken,
                RefreshTokenExpiryTime = model.RefreshTokenExpiryTime,
                CreationDate = model.CreationDate,
                ModificationDate = model.ModificationDate
            };

        /// <summary>
        /// Converts a collection of <see cref="UserEntity"/> to a collection of <see cref="User"/>.
        /// </summary>
        /// <param name="entities">The entities to be converted.</param> 
        /// <returns>A new list of User objects.</returns>
        public static IEnumerable<User> ToModel(this IEnumerable<UserEntity> entities)
            => entities.Select(entity => entity.ToModel());

        /// <summary>
        /// Converts a collection of <see cref="User"/> to a collection of <see cref="UserEntity"/>.
        /// </summary>
        /// <param name="models">The User objects to be converted.</param>
        /// <returns>A new list of UserEntity object.</returns>
        public static IEnumerable<UserEntity> ToEntity(this IEnumerable<User> models)
            => models.Select(model => model.ToEntity());
    }
}
