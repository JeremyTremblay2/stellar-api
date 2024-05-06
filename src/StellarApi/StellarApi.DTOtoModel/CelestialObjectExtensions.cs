using StellarApi.DTOs;
using StellarApi.DTOtoModel.Exceptions;
using StellarApi.Model.Space;
using StellarApi.Helpers;

namespace StellarApi.DTOtoModel
{
    /// <summary>
    /// Extension methods for converting between CelestialObject and CelestialObjectDTO.
    /// </summary>
    public static class CelestialObjectExtensions
    {
        /// <summary>
        /// Converts a CelestialObjectDTO to a CelestialObject.
        /// </summary>
        /// <param name="dto">The CelestialObjectDTO to convert.</param>
        /// <returns>The converted CelestialObject.</returns>
        public static CelestialObject ToModel(this CelestialObjectDTO dto)
        {
            if (dto.Type.Equals("Planet"))
            {
                var isWater = GetPropertyValue<bool>(dto.Metadata, "isWater");
                var isLife = GetPropertyValue<bool>(dto.Metadata, "isLife");
                var planetType = GetPropertyValue<PlanetType>(dto.Metadata, "planetType");
                return new Planet(dto.Id, dto.Name, dto.Description, dto.Image, PositionExtensions.ToModel(dto.Position), dto.Mass, 
                    dto.Temperature, dto.Radius, isWater ?? false, isLife ?? false, planetType ?? PlanetType.Undefined);
            }
            else if (dto.Type.Equals("Star"))
            {
                // Handle correctly type when added.
                return new CelestialObject(dto.Id, dto.Name, dto.Description, dto.Image, PositionExtensions.ToModel(dto.Position),
                    dto.Mass, dto.Temperature, dto.Radius, dto.CreationDate, dto.ModificationDate);
            }
            else
            {
                throw new WrongCelestialObjectTypeException(nameof(dto.Type), "The celestial object type is not supported.");
            }
        }

        /// <summary>
        /// Converts a collection of CelestialObjectDTOs to CelestialObjects.
        /// </summary>
        /// <param name="dtos">The collection of CelestialObjectDTOs to convert.</param>
        /// <returns>The converted collection of CelestialObjects.</returns>
        public static IEnumerable<CelestialObject> ToModel(this IEnumerable<CelestialObjectDTO> dtos)
        {
            return dtos.Select(dto => dto.ToModel()).Where(obj => obj != null).Select(obj => obj!);
        }

        /// <summary>
        /// Converts a CelestialObject to a CelestialObjectDTO.
        /// </summary>
        /// <param name="model">The CelestialObject to convert.</param>
        /// <returns>The converted CelestialObjectDTO.</returns>
        public static CelestialObjectDTO ToDTO(this CelestialObject model)
        {
            var dto = new CelestialObjectDTO
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Image = model.Image,
                Position = model.Position.ToDTO(),
                Mass = model.Mass,
                Temperature = model.Temperature,
                Radius = model.Radius,
                CreationDate = (DateTime)model.CreationDate,
                ModificationDate = (DateTime)model.ModificationDate
            };

            if (model is Planet planet)
            {
                dto.Type = "Planet";
                dto.Metadata = new List<PropertyDTO>
                {
                    new PropertyDTO { Name = "isWater", Type = "boolean", Value = planet.IsWater.ToString() },
                    new PropertyDTO { Name = "isLife", Type = "boolean", Value = planet.IsLife.ToString() },
                    new PropertyDTO { Name = "planetType", Type = "PlanetType", Value = planet.PlanetType.ToString() }
                };
            }
            else
            {
                dto.Type = "Star";
                // Handle correctly type when added.
            }

            return dto;
        }

        /// <summary>
        /// Converts a collection of CelestialObjects to CelestialObjectDTOs.
        /// </summary>
        /// <param name="models">The collection of CelestialObjects to convert.</param>
        /// <returns>The converted collection of CelestialObjectDTOs.</returns>
        public static IEnumerable<CelestialObjectDTO> ToDTO(this IEnumerable<CelestialObject> models)
        {
            return models.Select(model => model.ToDTO()).Where(obj => obj != null).Select(obj => obj!);
        }

        /// <summary>
        /// Gets the value of a property from a collection of PropertyDTOs.
        /// </summary>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="properties">The collection of PropertyDTOs.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the property if found; otherwise, null.</returns>
        private static T? GetPropertyValue<T>(IEnumerable<PropertyDTO> properties, string propertyName) where T : struct
        {
            foreach (var prop in properties)
            {
                if (prop.Name == propertyName && prop.Type.Equals(typeof(T).Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (ValueParserHelpers.TryParseValue(prop.Value, out T result))
                    {
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }
    }
}
