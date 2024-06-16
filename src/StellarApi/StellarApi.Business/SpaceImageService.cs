using Microsoft.Extensions.Logging;
using StellarApi.Infrastructure.Business;
using StellarApi.Infrastructure.Repository;
using StellarApi.Model.Space;

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
    public async Task<SpaceImage> GetSpaceImageOfTheDay()
    {
        var spaceImage = await _repository.GetSpaceImageOfTheDay();
        if (spaceImage is null)
        {
            _logger.LogInformation("Fetching space image of the day from the API.");
            spaceImage = await _httpRepository.FetchSpaceImageOfTheDay();

            _repository.AddSpaceImage(spaceImage);
        }

        return spaceImage;
    }
}