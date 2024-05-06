using StellarApi.DTOs;
using StellarApi.Model.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarApi.DTOtoModel
{
    public static class PositionExtensions
    {
        /// <summary>
        /// Converts a PositionDTO to a Position.
        /// </summary>
        /// <param name="model">The model to transform.</param>
        /// <returns>A DTO representing the model object.</returns>
        public static PositionDTO? ToDTO(this Position? model)
            => model is null ? null :new PositionDTO(model.X, model.Y, model.Z);

        /// <summary>
        /// Converts a Position to a PositionDTO.
        /// </summary>
        /// <param name="dto">The DTO to transform.</param>
        /// <returns>A model representing the DTO object.</returns>
        public static Position? ToModel(this PositionDTO? dto)
            => dto is null ? null : new Position(dto.X, dto.Y, dto.Z);
    }
}
