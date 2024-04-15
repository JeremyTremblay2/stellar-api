using System.Globalization;

namespace StellarApi.Model.Space;

public class Map : IEquatable<Map>, IComparable<Map>, IComparable
{
    /// <summary>
    /// The unique identifier of the map.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// The name of the map.
    /// </summary>
    private string name;

    public string Name
    {
        get => name;
        private set { name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value); }
    }

    /// <summary>
    /// The celestial objects in the map.
    /// </summary>
    public IEnumerable<CelestialObject> CelestialObjects { get; private set; }

    /// <summary>
    /// The creation date of the map.
    /// </summary>
    public DateTime? CreationDate { get; private set; }

    /// <summary>
    /// The modification date of the map.
    /// </summary>
    public DateTime? ModificationDate { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Map"/> class with specified properties.
    /// </summary>
    /// <param name="id">The unique identifier of the map.</param>
    /// <param name="name">The name of the map.</param>
    /// <param name="creationDate">The creation date of the map.</param>
    /// <param name="modificationDate">The last modification date of the map.</param>
    /// <exception cref="ArgumentNullException">Throw when the name of the object is null or empty.</exception>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="modificationDate"/> is in the future or before the <paramref name="creationDate"/>.</exception>
    public Map(string name, int id, DateTime? creationDate, DateTime? modificationDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "The name of the object cannot be null or empty.");

        if (modificationDate.HasValue)
        {
            if (modificationDate.Value > DateTime.UtcNow)
                throw new ArgumentException("The modification date cannot be in the future.", nameof(modificationDate));
            else if (creationDate.HasValue && modificationDate.Value < creationDate.Value)
                throw new ArgumentException("The modification date cannot be before the creation date.",
                    nameof(modificationDate));
            ModificationDate = modificationDate.Value;
        }
        else
            ModificationDate = DateTime.UtcNow;

        Id = id;
        Name = name;
        CelestialObjects = new List<CelestialObject>();
        CreationDate = creationDate;
        ModificationDate = modificationDate;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => Id.GetHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == typeof(Map) && Equals((Map)obj);
    }

    /// <inheritdoc/>
    public bool Equals(Map? other)
    {
        if (other == null)
            return false;

        return Id == other.Id;
    }

    /// <inheritdoc/>
    public int CompareTo(object? obj)
    {
        if (obj == null) return 1;
        if (obj is not Map)
            throw new ArgumentException("Object is not a Map", nameof(obj));
        return CompareTo((Map)obj);
    }

    /// <inheritdoc/>
    public int CompareTo(Map? other)
    {
        if (other == null) return 1;

        return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }

    public void AddCelestialObject(CelestialObject celestialObject)
    {
        if (celestialObject == null)
            throw new ArgumentNullException(nameof(celestialObject), "The celestial object cannot be null.");

        if (CelestialObjects.Contains(celestialObject))
            throw new ArgumentException("The celestial object is already in the map.", nameof(celestialObject));

        CelestialObjects.Append(celestialObject);
    }

    public void RemoveCelestialObject(CelestialObject celestialObject)
    {
        if (celestialObject == null)
            throw new ArgumentNullException(nameof(celestialObject), "The celestial object cannot be null.");

        if (!CelestialObjects.Contains(celestialObject))
            throw new ArgumentException("The celestial object is not in the map.", nameof(celestialObject));

        CelestialObjects = CelestialObjects.Where(c => !c.Equals(celestialObject));
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Id} - Map {Name}, Celestial Objects: {CelestialObjects.Count()}, CreationDate: {CreationDate}, ModificationDate: {ModificationDate}";
    }
}