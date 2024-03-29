﻿using System;
using System.Globalization;

namespace StellarApi.Model.Space
{
    /// <summary>
    /// Represents a celestial object.
    /// </summary>
    public abstract class CelestialObject : IEquatable<CelestialObject>, IComparable<CelestialObject>, IComparable
    {
        /// <summary>
        /// Gets the unique identifier of the celestial object.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The name of the celestial object.
        /// </summary>
        private string name;

        /// <summary>
        /// Gets or sets the name of the celestial object.
        /// </summary>
        public string Name 
        { 
            get => name; 
            private set
            {
                name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
            }
        }

        /// <summary>
        /// Gets or sets the description of the celestial object.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets or sets the image path of the celestial object.
        /// </summary>
        public string Image { get; private set; }

        /// <summary>
        /// Gets or sets the mass of the celestial object.
        /// </summary>
        public double Mass { get; private set; }

        /// <summary>
        /// Gets or sets the temperature of the celestial object.
        /// </summary>
        public double Temperature { get; private set; }

        /// <summary>
        /// Gets or sets the radius of the celestial object.
        /// </summary>
        public double Radius { get; private set; }

        /// <summary>
        /// Gets the creation date of the celestial object.
        /// </summary>
        public DateTime CreationDate { get; private set; }

        /// <summary>
        /// Gets the last modification date of the celestial object.
        /// </summary>
        public DateTime ModificationDate { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CelestialObject"/> class with specified properties.
        /// </summary>
        /// <param name="id">The unique identifier of the celestial object.</param>
        /// <param name="name">The name of the celestial object.</param>
        /// <param name="description">The description of the celestial object.</param>
        /// <param name="image">The image path of the celestial object.</param>
        /// <param name="mass">The mass of the celestial object.</param>
        /// <param name="temperature">The temperature of the celestial object.</param>
        /// <param name="radius">The radius of the celestial object.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="radius"/> or <paramref name="mass"/> is less than or equal to 0.</exception>
        public CelestialObject(int id, string name, string description, string image, double mass, double temperature, double radius)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), "The name of the object cannot be null or empty.");

            if (radius <= 0)
                throw new ArgumentException("The radius of the object cannot be less than or equal to 0.", nameof(radius));

            if (mass <= 0)
                throw new ArgumentException("The mass of the object cannot be less than or equal to 0.", nameof(mass));

            Id = id;
            Name = name;
            Description = description;
            Image = image;
            Mass = mass;
            Temperature = temperature;
            Radius = radius;
            CreationDate = DateTime.UtcNow;
            ModificationDate = CreationDate;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => Id.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(CelestialObject)) return false;
            return Equals((CelestialObject) obj);
        }

        /// <inheritdoc/>
        public bool Equals(CelestialObject? other)
        {
            if (other == null) return false;
            return Id == other.Id;
        }

        /// <inheritdoc/>
        public int CompareTo(CelestialObject? other)
        {
            if (other == null) return 1;

            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            if (obj is not CelestialObject)
                throw new ArgumentException("Object is not a CelestialObject", nameof(obj));
            return CompareTo((CelestialObject)obj);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Id} - {Name}, (Description: {Description}), Mass: {Mass}, Temperature: {Temperature}, Radius: {Radius}, Image: {Image}, CreationDate: {CreationDate}, ModificationDate: {ModificationDate}";
        }
    }
}
