namespace StellarApi.DTOs.Utils
{
    /// <summary>
    /// Represents a property with a name, value, and type.
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
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
