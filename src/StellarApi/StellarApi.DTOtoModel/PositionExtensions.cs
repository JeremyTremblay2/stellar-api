using StellarApi.DTOs.Geometry;

namespace StellarApi.DTOtoModel
{
    /// <summary>
    /// Represents a way to transform <see cref="Model.Geometry.Position"/> objects into <see cref="Position"/> objects.
    /// </summary>
    public static class PositionExtensions
    {
        /// <summary>
        /// Converts a PositionDTO to a Position.
        /// </summary>
        /// <param name="model">The model to transform.</param>
        /// <returns>A DTO representing the model object.</returns>
        public static Position? ToDTO(this Model.Geometry.Position? model)
            => model is null ? null :new Position(model.X, model.Y, model.Z);

        /// <summary>
        /// Converts a Position to a PositionDTO.
        /// </summary>
        /// <param name="dto">The DTO to transform.</param>
        /// <returns>A model representing the DTO object.</returns>
        public static Model.Geometry.Position? ToModel(this Position? dto)
            => dto is null ? null : new Model.Geometry.Position(dto.X, dto.Y, dto.Z);
    }
}
