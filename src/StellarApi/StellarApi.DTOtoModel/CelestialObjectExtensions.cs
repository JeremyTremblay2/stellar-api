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
                return new Planet(dto.Id, dto.Name, dto.Description, dto.Image, PositionExtensions.ToModel(dto.Position), dto.Mass, 
                    dto.Temperature, dto.Radius, dto.IsWater ?? false, dto.IsLife ?? false, Enum.Parse<PlanetType>(dto.PlanetType ?? "Undefined"));
            }
            else if (dto.Type.Equals("Star"))
            {
                return new Star(dto.Id, dto.Name, dto.Description, dto.Image, PositionExtensions.ToModel(dto.Position),
                    dto.Mass, dto.Temperature, dto.Radius, dto.Brightness ?? 1, Enum.Parse<StarType>(dto.StarType ?? "Undefined"));
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
            Planet? planet = model is Planet ? (Planet)model : null;
            Star? star = model is Star ? (Star)model : null;
            return new CelestialObjectDTO
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Position = model.Position.ToDTO(),
                Image = model.Image,
                Mass = model.Mass,
                Temperature = model.Temperature,
                Radius = model.Radius,
                CreationDate = model.CreationDate,
                ModificationDate = model.ModificationDate,
                Type = model.GetType().Name,
                IsWater = model is Planet ? planet!.IsWater : null,
                IsLife = model is Planet ? planet!.IsLife : null,
                PlanetType = model is Planet ? planet!.PlanetType.ToString() : null,
                Brightness = model is Star ? star!.Brightness : null,
                StarType = model is Star ? star!.StarType.ToString() : null
            };
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
