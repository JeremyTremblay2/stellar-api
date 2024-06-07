using StellarApi.DTOtoModel.Exceptions;
using StellarApi.Model.Space;
using StellarApi.DTOs.Space;
using System.ComponentModel;
using StellarApi.Model.Users;

namespace StellarApi.DTOtoModel
{
    /// <summary>
    /// Extension methods for converting between <see cref="CelestialObject"/>, <see cref="CelestialObjectOutput"/> and <see cref="CelestialObjectInput"/>.
    /// </summary>
    public static class CelestialObjectExtensions
    {
        /// <summary>
        /// Converts a CelestialObjectInput to a CelestialObject.
        /// </summary>
        /// <param name="dto">The CelestialObjectInput to convert.</param>
        /// <returns>The converted CelestialObject.</returns>
        public static CelestialObject ToModel(this CelestialObjectInput dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Type))
            {
                throw new ArgumentException("The type cannot be null or empty.", nameof(dto.Type));
            }
            else if (dto.Type.Equals("Planet"))
            {
                if (!Enum.TryParse(dto.PlanetType ?? "Undefined", out PlanetType planetType))
                {
                    throw new InvalidEnumArgumentException($"The planet type {dto.PlanetType} is not supported.");
                }
                return new Planet(0, dto.Name, dto.Description, dto.Image, PositionExtensions.ToModel(dto.Position), dto.Mass,
                    dto.Temperature, dto.Radius, 0, dto.IsWater ?? false, dto.IsLife ?? false, planetType, dto.IsPublic, null, null, dto.MapId);
                
            }
            else if (dto.Type.Equals("Star"))
            {
                if (!Enum.TryParse(dto.StarType ?? "Undefined", out StarType starType))
                {
                    throw new InvalidEnumArgumentException($"The star type {dto.StarType} is not supported.");
                }
                return new Star(0, dto.Name, dto.Description, dto.Image, PositionExtensions.ToModel(dto.Position),
                    dto.Mass, dto.Temperature, dto.Radius, 0, dto.Brightness ?? 0, starType, dto.IsPublic, null, null, dto.MapId);
            }
            else
            {
                throw new WrongCelestialObjectTypeException(nameof(dto.Type), "The celestial object type is not supported.");
            }
        }

        /// <summary>
        /// Converts a collection of CelestialObjectInput to CelestialObjects.
        /// </summary>
        /// <param name="dtos">The collection of CelestialObjectInput to convert.</param>
        /// <returns>The converted collection of CelestialObjects.</returns>
        public static IEnumerable<CelestialObject> ToModel(this IEnumerable<CelestialObjectInput> dtos)
        {
            return dtos.Select(dto => dto.ToModel()).Where(obj => obj != null).Select(obj => obj!);
        }

        /// <summary>
        /// Converts a CelestialObject to a CelestialObjectOutput.
        /// </summary>
        /// <param name="model">The CelestialObject to convert.</param>
        /// <returns>The converted CelestialObjectOutput.</returns>
        public static CelestialObjectOutput ToDTO(this CelestialObject model)
        {
            Planet? planet = model is Planet ? (Planet)model : null;
            Star? star = model is Star ? (Star)model : null;
            return new CelestialObjectOutput
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Position = model.Position.ToDTO(),
                Image = model.Image,
                Mass = model.Mass,
                Temperature = model.Temperature,
                Radius = model.Radius,
                UserAuthorId = model.UserAuthorId,
                CreationDate = model.CreationDate,
                ModificationDate = model.ModificationDate,
                Type = Enum.Parse<CelestialObjectType>(model.GetType().Name),
                IsPublic = model.IsPublic,
                MapId = model.MapId,
                IsWater = model is Planet ? planet!.IsWater : null,
                IsLife = model is Planet ? planet!.IsLife : null,
                PlanetType = model is Planet ? planet!.PlanetType : null,
                Brightness = model is Star ? star!.Brightness : null,
                StarType = model is Star ? star!.StarType : null
            };
        }

        /// <summary>
        /// Converts a collection of CelestialObjects to CelestialObjectOutput.
        /// </summary>
        /// <param name="models">The collection of CelestialObjects to convert.</param>
        /// <returns>The converted collection of CelestialObjectOutput.</returns>
        public static IEnumerable<CelestialObjectOutput> ToDTO(this IEnumerable<CelestialObject> models)
        {
            return models.Select(model => model.ToDTO()).Where(obj => obj != null).Select(obj => obj!);
        }
    }
}
