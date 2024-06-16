using Microsoft.Extensions.Logging;
using StellarApi.Infrastructure.Business;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;
using StellarApi.Repository.Exceptions;

namespace StellarApi.Business;

/// <summary>
/// Service class for managing space images.
/// </summary>
public class SpaceImageService : ISpaceImageService
{
    /// <summary>
    /// The repository used by this service.
    /// </summary>
    private readonly ISpaceImageRepository _repository;

    /// <summary>
    /// The repository used for fetching space images from the API.
    /// </summary>
    private readonly IHttpSpaceImageRepository _httpRepository;

    /// <summary>
    /// Logger used by this service.
    /// </summary>
    private readonly ILogger<SpaceImageService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpaceImageService"/> class.
    /// </summary>
    /// <param name="logger">The logger used by this service.</param>
    /// <param name="repository">The repository used for accessing space images.</param>
    /// <param name="httpRepository">The repository used for fetching space images from the API.</param>
    public SpaceImageService(ILogger<SpaceImageService> logger, ISpaceImageRepository repository,
        IHttpSpaceImageRepository httpRepository)
    {
        _logger = logger;
        _repository = repository;
        _httpRepository = httpRepository;
    }

    /// <inheritdoc/>
    public async Task<SpaceImage?> GetSpaceImage(int id)
    {
        return await _repository.GetSpaceImage(id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SpaceImage>> GetSpaceImages(int page, int pageSize)
    {
        return await _repository.GetSpaceImages(page, pageSize);
    }

    /// <inheritdoc/>
    public async Task<SpaceImage?> GetSpaceImage(DateTime? date)
    {
        if (date is null) return await GetSpaceImage();

        if (date.Value.Date == DateTime.Now.Date)
            return await GetSpaceImage();

        return await _repository.GetSpaceImageByDate(date.Value);
    }

    /// <summary>
    /// Retrieves the space image of the day from the API or the database if already fetched.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation and returns the space image of the day.</returns>
    /// <exception cref="SpaceImageFetchingException">Thrown when the space image could not be fetched from the API.</exception>
    private async Task<SpaceImage?> GetSpaceImage()
    {
        var spaceImage = await _repository.GetSpaceImageOfTheDay();
        if (spaceImage is not null) return spaceImage;
        _logger.LogInformation("Fetching space image of the day from the API.");
        spaceImage = await _httpRepository.FetchSpaceImageOfTheDay();

        if (string.IsNullOrWhiteSpace(spaceImage.Title))
        {
            _logger.LogError("The title returned from the API was null or empty.");
            throw new SpaceImageFetchingException("The title returned from the API was null or empty.");
        }

        if (string.IsNullOrWhiteSpace(spaceImage.Description))
        {
            _logger.LogError("The description returned from the API was null or empty.");
            throw new SpaceImageFetchingException("The description returned from the API was null or empty.");
        }

        if (string.IsNullOrWhiteSpace(spaceImage.Image))
        {
            _logger.LogError("The image URL returned from the API was null or empty.");
            throw new SpaceImageFetchingException("The image URL returned from the API was null or empty.");
        }

        await _repository.AddSpaceImage(spaceImage);

        return spaceImage;
    }
}