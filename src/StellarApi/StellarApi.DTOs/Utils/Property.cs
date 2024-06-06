using Swashbuckle.AspNetCore.Annotations;

namespace StellarApi.DTOs.Utils
{
    /// <summary>
    /// Represents a property with a name, value, and type.
    /// </summary>
    [SwaggerSchema("A property, containing information about a property of an object.", ReadOnly = true)]
    public class Property
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        [SwaggerSchema(Description = "The name of the property.", Nullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        [SwaggerSchema(Description = "The value of the property.", Nullable = false)]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        [SwaggerSchema(Description = "The type of the property.", Nullable = false)]
        public string Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="type">The type of the property.</param>
        public Property(string name, string type, string value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        /// <summary>
        /// Create a new instance of the <see cref="Property"/> class without values.
        /// </summary>
        public Property()
        {

        }
    }
}
