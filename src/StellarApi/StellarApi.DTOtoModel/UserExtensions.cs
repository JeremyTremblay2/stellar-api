using StellarApi.Model.Users;
using StellarApi.DTOs.Users;

namespace DTOtoModel
{
    /// <summary>
    /// Extension methods for converting between <see cref="User"/> and <see cref="MinimalUserOutput"/>.
    /// </summary>
    public static class UserExtensions
    {
        /// <summary>
        /// Converts a <see cref="UserInput"/> to a <see cref="User"/>.
        /// </summary>
        /// <param name="dto">The DTO to transform.</param>
        /// <returns>The new User object.</returns>
        public static User ToModel(this UserInput? dto)
        {
            if (dto is null)
                return null;

            return new User(0, dto.Email, dto.Username, dto.Password, Role.Member);
        }

        /// <summary>
        /// Converts a <see cref="User"/> to a <see cref="MinimalUserOutput"/>.
        /// </summary>
        /// <param name="model">The User to transform.</param>
        /// <returns>The new DTO User object.</returns>
        public static MinimalUserOutput ToMinimalDTO(this User? model)
        {
            if (model is null)
                return null;

            return new MinimalUserOutput(model.Id, model.Username, model.Role.ToString(), model.CreationDate, model.ModificationDate);
        }

        /// <summary>
        /// Converts a <see cref="User"/> to a <see cref="MaximalUserOutput"/>.
        /// </summary>
        /// <param name="model">The User to transform.</param>
        /// <returns>The new DTO User object.</returns>
        public static MaximalUserOutput ToMaximalDTO(this User? model)
        {
            if (model is null)
                return null;

            return new MaximalUserOutput(model.Id, model.Username, model.Email, model.Role.ToString(), model.CreationDate, model.ModificationDate);
        }

        /// <summary>
        /// Converts a collection of <see cref="MinimalUserOutput"/> to a collection of <see cref="User"/>.
        /// </summary>
        /// <param name="dtos">The list of DTOs to transform.</param>
        /// <returns>A new list of User.</returns>
        public static IEnumerable<User> ToModel(this IEnumerable<UserInput> dtos)
            => dtos.Select(dto => dto.ToModel());

        /// <summary>
        /// Converts a collection of <see cref="User"/> to a collection of <see cref="MinimalUserOutput"/>.
        /// </summary>
        /// <param name="models">The list of User to transform.</param>
        /// <returns>The new list of User DTO.</returns>
        public static IEnumerable<MinimalUserOutput> ToMinimalDTO(this IEnumerable<User> models)
            => models.Select(model => model.ToMinimalDTO());
    }
}
