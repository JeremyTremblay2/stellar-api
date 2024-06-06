﻿using StellarApi.DTOs.Geometry;

namespace StellarApi.DTOs.Space
{
    /// <summary>
    /// Represents a Data Transfer Object (DTO) for a celestial object in output (all data), including its properties.
    /// </summary>
    public class CelestialObjectOutput
    {
        /// <summary>
        /// Gets or sets the unique identifier of the celestial object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the celestial object.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the celestial object.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the mass of the celestial object.
        /// </summary>
        public double Mass { get; set; }

        /// <summary>
        /// Gets or sets the temperature of the celestial object.
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// Gets or sets the radius of the celestial object.
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Gets or sets the image path of the celestial object.
        /// </summary>
        public string? Image { get; set; }

        /// <summary>
        /// Gets or sets the position of the celestial object.
        /// </summary>
        public Position? Position { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the celestial object.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the last modification date of the celestial object.
        /// </summary>
        public DateTime ModificationDate { get; set; }

        /// <summary>
        /// Gets or sets the type of the celestial object.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the planet type of the celestial object.
        /// </summary>
        public string? PlanetType { get; set; }

        /// <summary>
        /// Gets or sets the possibility of water on the celestial object.
        /// </summary>
        public bool? IsWater { get; set; }

        /// <summary>
        /// Gets or sets the possibility of life on the celestial object.
        /// </summary>
        public bool? IsLife { get; set; }

        /// <summary>
        /// Gets or set the star type of the celestial object.
        /// </summary>
        public string? StarType { get; set; }

        /// <summary>
        /// Gets or sets the brightness of the celestial object.
        /// </summary>
        public double? Brightness { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the map.
        /// </summary>
        public int? MapId { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="CelestialObjectOutput"/> class.
        /// </summary>
        public CelestialObjectOutput()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CelestialObjectOutput"/> class with specified properties.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object.</param>
        /// <param name="name">The name of the celestial object.</param>
        /// <param name="description">The description of the celestial object.</param>
        /// <param name="mass">The mass of the celestial object.</param>
        /// <param name="temperature">The temperature of the celestial object.</param>
        /// <param name="radius">The radius of the celestial object.</param>
        /// <param name="image">The image path of the celestial object.</param>
        /// <param name="creationDate">The creation date of the celestial object.</param>
        /// <param name="modificationDate">The last modification date of the celestial object.</param>
        /// <param name="type">The type of the celestial object.</param>
        /// <param name="mapId">The unique identifier of the map.</param>
        /// <param name="planetType">The type of the planet.</param>
        /// <param name="isWater">The possibility of water on the celestial object.</param>
        /// <param name="isLife">The possibility of life on the celestial object.</param>
        /// <param name="starType">The type of the star.</param>
        /// <param name="brightness">The brightness of the celestial object.</param>
        public CelestialObjectOutput(int id, string name, string description, Position? position, double mass, double temperature, double radius,
            string image, DateTime creationDate, DateTime modificationDate, string type, int? mapId = null, string? planetType = null, bool? isWater = null,
            bool? isLife = null, string? starType = null, double? brightness = null)
        {
            Id = id;
            Name = name;
            Description = description;
            Position = position;
            Mass = mass;
            Temperature = temperature;
            Radius = radius;
            Image = image;
            CreationDate = creationDate;
            ModificationDate = modificationDate;
            Type = type;
            MapId = mapId;
            PlanetType = planetType;
            IsWater = isWater;
            IsLife = isLife;
            StarType = starType;
            Brightness = brightness;
        }
    }
}
