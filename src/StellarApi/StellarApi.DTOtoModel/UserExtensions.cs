﻿using StellarApi.Model.Users;
using StellarApi.DTOs;

namespace DTOtoModel
{
    /// <summary>
    /// Extension methods for converting between <see cref="User"/> and <see cref="UserDTO"/>.
    /// </summary>
    public static class UserExtensions
    {
        /// <summary>
        /// Converts a <see cref="UserDTO"/> to a <see cref="User"/>.
        /// </summary>
        /// <param name="dto">The DTO to transform.</param>
        /// <returns>The new User object.</returns>
        public static User ToModel(this UserDTO? dto)
        {
            if (dto is null)
                return null;

            if (!Enum.TryParse(dto.Role, out Role role))
                throw new ArgumentException("Invalid role value.");

            return new User(dto.Id, dto.Email, dto.Username, dto.Password, role, dto.RefreshToken, dto.RefreshTokenExpiryTime, dto.CreationDate, dto.ModificationDate);
        }

        /// <summary>
        /// Converts a <see cref="User"/> to a <see cref="UserDTO"/>.
        /// </summary>
        /// <param name="model">The User to transform.</param>
        /// <returns>The new DTO User object.</returns>
        public static UserDTO ToDTO(this User? model)
        {
            if (model is null)
                return null;

            return new UserDTO(model.Id, model.Email, model.Username, model.Password, model.Role.ToString(), model.RefreshToken, 
                model.RefreshTokenExpiryTime, model.CreationDate, model.ModificationDate);
        }

        /// <summary>
        /// Converts a collection of <see cref="UserDTO"/> to a collection of <see cref="User"/>.
        /// </summary>
        /// <param name="dtos">The list of DTOs to transform.</param>
        /// <returns>A new list of User.</returns>
        public static IEnumerable<User> ToModel(this IEnumerable<UserDTO> dtos)
            => dtos.Select(dto => dto.ToModel());

        /// <summary>
        /// Converts a collection of <see cref="User"/> to a collection of <see cref="UserDTO"/>.
        /// </summary>
        /// <param name="models">The list of User to transform.</param>
        /// <returns>The new list of User DTO.</returns>
        public static IEnumerable<UserDTO> ToDTO(this IEnumerable<User> models)
            => models.Select(model => model.ToDTO());
    }
}
