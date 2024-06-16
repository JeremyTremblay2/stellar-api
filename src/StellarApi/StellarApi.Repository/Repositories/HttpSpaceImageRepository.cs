using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;
using StellarApi.Repository.DTOs;
using StellarApi.Repository.Exceptions;

namespace StellarApi.Repository.Repositories;

/// <summary>
/// Represents a repository for managing space images fetched from an HTTP API.
/// </summary>
public class HttpSpaceImageRepository : IHttpSpaceImageRepository
{
    /// <summary>
    /// Logger used by this repository.
    /// </summary>
    private readonly ILogger<HttpSpaceImageRepository> _logger;

    /// <summary>
    /// Represents the configuration settings for the application.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpaceImageRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger used by this repository.</param>
    /// <param name="context">The database context for managing space image data.</param>
    public HttpSpaceImageRepository(ILogger<HttpSpaceImageRepository> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }


    /// <inheritdoc/>
    public async Task<SpaceImage> FetchSpaceImageOfTheDay()
    {
        using var client = new HttpClient();

        SpaceImageResponse? spaceImageResponse = null;
        try
        {
            var response = await client.GetAsync(_configuration.GetSection("ApiUrls")["AstronomyPictureOfTheDayUrl"]);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            spaceImageResponse = JsonSerializer.Deserialize<SpaceImageResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the space image of the day from the API.");
            throw new SpaceImageFetchingException(
                "An error occurred while fetching the space image of the day from the API.", ex);
        }
        catch (Exception ex) when (ex is ArgumentNullException or NotSupportedException or JsonException)
        {
            _logger.LogError(ex, "An error occurred while deserializing the space image response.");
            throw new SpaceImageFetchingException("An error occurred while deserializing the space image response.",
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the space image of the day.");
            throw new SpaceImageFetchingException("An error occurred while fetching the space image of the day.", ex);
        }

        var spaceImage = new SpaceImage(0, spaceImageResponse.Title, spaceImageResponse.Explanation,
            spaceImageResponse.HdUrl is null ? spaceImageResponse.Url : spaceImageResponse.HdUrl, DateTime.Today);

        if (spaceImage is null) throw new Exception("The space image could not be created.");

        return spaceImage;
    }
}