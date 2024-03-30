/// <summary>
/// Provides utility methods for temperature conversion between Kelvin and Celsius scales.
/// </summary>
public static class TemperatureConverter
{
    // Value used during conversion.
    private const double kelvinToCelsiusOffset = 273.15;

    /// <summary>
    /// Converts temperature from Kelvin to Celsius scale.
    /// </summary>
    /// <param name="kelvin">Temperature value in Kelvin.</param>
    /// <returns>The equivalent temperature value in Celsius.</returns>
    public static double KelvinToCelsius(this double kelvin)
        => kelvin - kelvinToCelsiusOffset;

    /// <summary>
    /// Converts temperature from Celsius to Kelvin scale.
    /// </summary>
    /// <param name="celsius">Temperature value in Celsius.</param>
    /// <returns>The equivalent temperature value in Kelvin.</returns>
    public static double CelsiusToKelvin(this double celsius)
        => celsius + kelvinToCelsiusOffset;
}
