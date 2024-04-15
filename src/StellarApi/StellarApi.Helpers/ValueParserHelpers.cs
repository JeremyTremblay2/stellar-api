namespace StellarApi.Helpers
{
    /// <summary>
    /// Represents a class that can be helpful during the process of value parsing.
    /// </summary>
    public static class ValueParserHelpers
    {
        /// <summary>
        /// Tries to parse the value to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to parse to.</typeparam>
        /// <param name="value">The value to parse.</param>
        /// <param name="result">When this method returns, contains the parsed value if the conversion succeeded, or the default value if the conversion failed.</param>
        /// <returns>True if the value was parsed successfully; otherwise, false.</returns>
        public static bool TryParseValue<T>(string value, out T result) where T : struct
        {
            result = default;
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
