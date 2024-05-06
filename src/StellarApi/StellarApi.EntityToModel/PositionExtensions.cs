using StellarApi.Entities;
using StellarApi.Model.Geometry;

namespace StellarApi.EntityToModel
{
    /// <summary>
    /// Provides extension methods for converting between PositionEntity and Position.
    /// </summary>
    public static class PositionExtensions
    {
        /// <summary>
        /// Transforms a PositionEntity to a Position.
        /// </summary>
        public static Position? ToModel(this PositionEntity? entity)
            => entity is null || entity.X is null || entity.Y is null || entity.Z is null ? null 
            : new Position((int) entity.X, (int) entity.Y, (int) entity.Z);

        /// <summary>
        /// Transforms a Position to a PositionEntity.
        /// </summary>
        public static PositionEntity ToEntity(this Position? entity)
            => entity is null ? new PositionEntity() : new PositionEntity { X = entity.X, Y = entity.Y, Z = entity.Z };
    }
}
