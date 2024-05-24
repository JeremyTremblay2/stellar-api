using StellarApi.Entities;
using StellarApi.Helpers;
using StellarApi.Model.Space;

namespace StellarApi.EntityToModel
{
    /// <summary>
    /// Provides extension methods for converting between CelestialObjectEntity and CelestialObject.
    /// </summary>
    public static class CelestialObjectExtensions
    {
        /// <summary>
        /// Converts a CelestialObjectEntity to a CelestialObject.
        /// </summary>
        /// <param name="entity">The entity to transform.</param>
        /// <returns>A model representing the entity object.</returns>
        public static CelestialObject ToModel(this CelestialObjectEntity? entity)
        {
            if (entity is null) return null;
            if (entity.Type.Equals("Planet"))
            {
                var planetType = ValueParserHelpers.TryParseValue<PlanetType>(entity.PlanetType, out var result)
                    ? result
                    : PlanetType.Undefined;
                return new Planet(entity.Id, entity.Name, entity.Description, entity.Image,
                    PositionExtensions.ToModel(entity.Position), entity.Mass,
                    entity.Temperature, entity.Radius, (bool)entity.IsWater, (bool)entity.IsLife, planetType);
            }
            else if (entity.Type.Equals("Star"))
            {
                var starType = ValueParserHelpers.TryParseValue<StarType>(entity.StarType, out var result)
                    ? result
                    : StarType.Undefined;
                return new Star(entity.Id, entity.Name, entity.Description, entity.Image,
                    PositionExtensions.ToModel(entity.Position),
                    entity.Mass, entity.Temperature, entity.Radius, (double)entity.Brightness, starType);
            }
            else
            {
                throw new ArgumentException("The celestial object type is not supported.", nameof(entity.Type));
            }
        }

        /// <summary>
        /// Converts a collection of CelestialObjectEntities to CelestialObjects.
        /// </summary>
        /// <param name="entities">The collection of CelestialObjectEntity to convert.</param>
        /// <returns>The converted collection of CelestialObjects.</returns>
        public static IEnumerable<CelestialObject> ToModel(this IEnumerable<CelestialObjectEntity> entities)
        {
            return entities.Select(entity => entity.ToModel()).Where(obj => obj != null).Select(obj => obj!);
        }

        /// <summary>
        /// Converts a CelestialObject to a CelestialObjectEntity.
        /// </summary>
        /// <param name="model">The model to transform.</param>
        /// <returns>An entity representing the model object.</returns>
        public static CelestialObjectEntity ToEntity(this CelestialObject model)
        {
            Planet? planet = model is Planet ? (Planet)model : null;
            Star? star = model is Star ? (Star)model : null;
            return new CelestialObjectEntity
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Image = model.Image,
                Position = PositionExtensions.ToEntity(model.Position),
                Mass = model.Mass,
                Temperature = model.Temperature,
                Radius = model.Radius,
                CreationDate = model.CreationDate,
                ModificationDate = model.ModificationDate,
                Type = model switch
                {
                    Planet => "Planet",
                    Star => "Star",
                    _ => throw new ArgumentException("The celestial object type is not supported.", nameof(model))
                },
                IsWater = model is Planet ? planet!.IsWater : null,
                IsLife = model is Planet ? planet!.IsLife : null,
                PlanetType = model is Planet ? planet!.PlanetType.ToString() : null,
                Brightness = model is Star ? star!.Brightness : null,
                StarType = model is Star ? star!.StarType.ToString() : null
            };
        }

        /// <summary>
        /// Converts a collection of CelestialObjects to CelestialObjectEntity.
        /// </summary>
        /// <param name="models">The collection of CelestialObjects to convert.</param>
        /// <returns>The converted collection of CelestialObjectEntity.</returns>
        public static IEnumerable<CelestialObjectEntity> ToEntity(this IEnumerable<CelestialObject> models)
        {
            return models.Select(model => model.ToEntity()).Where(obj => obj != null).Select(obj => obj!);
        }
    }
}