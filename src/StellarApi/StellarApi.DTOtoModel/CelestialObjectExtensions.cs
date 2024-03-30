using StellarApi.DTOs;
using StellarApi.DTOtoModel.Exceptions;
using StellarApi.Model.Space;

namespace StellarApi.DTOtoModel
{
    public static class CelestialObjectExtensions
    {
        public static CelestialObject ToModel(this CelestialObjectDTO dto)
        {
            if (dto.Type.Equals("Star"))
            {
                // Handle correctly type when added.
                return new CelestialObject(dto.Id, dto.Name, dto.Description, dto.Image, dto.Mass, dto.Temperature,
                    dto.Radius, dto.CreationDate, dto.ModificationDate);
            }
            else if (dto.Type.Equals("Planet"))
            {
                var isWater = GetPropertyValue<bool>(dto.Metadata, "isWater");
                var isLife = GetPropertyValue<bool>(dto.Metadata, "isLife");
                var planetType = GetPropertyValue<PlanetType>(dto.Metadata, "planetType");
                return new Planet(dto.Id, dto.Name, dto.Description, dto.Image, dto.Mass, dto.Temperature, dto.Radius, 
                    isWater ?? false, isLife ?? false, planetType ?? PlanetType.Undefined);
            }
            else
            {
                throw new WrongCelestialObjectTypeException(nameof(dto.Type), "The celestial object type is not supported.");
            }
        }

        public static IEnumerable<CelestialObject> ToModel(this IEnumerable<CelestialObjectDTO> dtos)
        {
            return dtos.Select(dto => dto.ToModel()).Where(obj => obj != null).Select(obj => obj!);
        }

        public static CelestialObjectDTO ToDTO(this CelestialObject model)
        {
            var dto = new CelestialObjectDTO
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Image = model.Image,
                Mass = model.Mass,
                Temperature = model.Temperature,
                Radius = model.Radius,
                CreationDate = model.CreationDate,
                ModificationDate = model.ModificationDate
            };

            if (model is Planet planet)
            {
                dto.Type = "Planet";
                dto.Metadata = new List<PropertyDTO>
                {
                    new PropertyDTO { Name = "isWater", Type = "bool", Value = planet.IsWater.ToString() },
                    new PropertyDTO { Name = "isLife", Type = "bool", Value = planet.IsLife.ToString() },
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

        public static IEnumerable<CelestialObjectDTO> ToDTO(this IEnumerable<CelestialObject> models)
        {
            return models.Select(model => model.ToDTO()).Where(obj => obj != null).Select(obj => obj!);
        }

        private static T? GetPropertyValue<T>(IEnumerable<PropertyDTO> properties, string propertyName) where T : struct
        {
            foreach (var prop in properties)
            {
                if (prop.Name == propertyName && prop.Type.ToLower() == typeof(T).Name.ToLower())
                {
                    if (TryParseValue<T>(prop.Value, out T result))
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
        private static bool TryParseValue<T>(string value, out T result) where T : struct
        {
            result = default(T);
            try
            {
                if (typeof(T).IsEnum)
                {
                    result = (T)Enum.Parse(typeof(T), value);
                }
                else
                {
                    result = (T)Convert.ChangeType(value, typeof(T));
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
