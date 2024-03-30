namespace StellarApi.DTOs
{
    /// <summary>
    /// Represents a property with a name, value, and type.
    /// </summary>
    public class PropertyDTO
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
        /// Initializes a new instance of the <see cref="PropertyDTO"/> class.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="type">The type of the property.</param>
        public PropertyDTO(string name, string value, string type)
        {
            Name = name;
            Value = value;
            Type = type;
        }
    }
}
