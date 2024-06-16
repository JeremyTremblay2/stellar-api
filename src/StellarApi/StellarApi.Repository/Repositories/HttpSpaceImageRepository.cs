using System.Text.Json;
using Microsoft.Extensions.Configuration;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;
using StellarApi.Repository.Context;
using StellarApi.Repository.Exceptions;

namespace StellarApi.Repository.Repositories;

/// <summary>
/// Represents a repository for managing space images fetched from an HTTP API.
/// </summary>
public class HttpSpaceImageRepository : IHttpSpaceImageRepository
{
    /// <summary>
    /// Represents the configuration settings for the application.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpaceImageRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for managing space image data.</param>
    public HttpSpaceImageRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    /// <inheritdoc/>
    public async Task<SpaceImage> FetchSpaceImageOfTheDay()
    {
        using var client = new HttpClient();

        var response = await client.GetAsync(_configuration.GetSection("ApiUrls")["AstronomyPictureOfTheDayUrl"]);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var spaceImageResponse = JsonSerializer.Deserialize<SpaceImageResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (spaceImageResponse is null) throw new Exception("The API response could not be deserialized.");

        var spaceImage = new SpaceImage(0, spaceImageResponse.Title, spaceImageResponse.Explanation,
            spaceImageResponse.HdUrl, DateTime.Today);

        if (spaceImage is null) throw new Exception("The space image could not be created.");

        return spaceImage;
    }
}