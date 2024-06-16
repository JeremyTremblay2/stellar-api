using System.Text.Json.Serialization;

namespace StellarApi.Repository.DTOs;

/// <summary>
/// Represents a space image response from an API.
/// </summary>
public class SpaceImageResponse
{
    /// <summary>
    /// The title of the space image.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// The description of the space image.
    /// </summary>
    [JsonPropertyName("explanation")]
    public string Explanation { get; set; }

    /// <summary>
    /// The image url of the space image.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// The high definition image url of the space image.
    /// </summary>
    [JsonPropertyName("hdurl")]
    public string HdUrl { get; set; }


    /// <summary>
    /// The date of the space image.
    /// </summary>
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpaceImageResponse"/> class with specified properties.
    /// </summary>
    /// <param name="title">The title of the space image.</param>
    /// <param name="explanation">The description of the space image.</param>
    /// <param name="url">The image url of the space image.</param>
    /// <param name="hdUrl">The high definition image url of the space image.</param>
    /// <param name="date">The date of the space image.</param>
    public SpaceImageResponse(string title, string explanation, string url, string hdUrl, DateTime date)
    {
        Title = title;
        Explanation = explanation;
        Url = url;
        HdUrl = hdUrl;
        Date = date;
    }
}