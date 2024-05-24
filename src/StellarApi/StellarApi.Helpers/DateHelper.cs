namespace StellarApi.Helpers;

/// <summary>
/// Represents a class that can be helpful during the process of date parsing.
/// </summary>
public static class DateHelper
{
    /// <summary>
    /// Checks the dates and returns the final creation and modification dates.
    /// </summary>
    /// <param name="creationDate">The creation date of the object.</param>
    /// <param name="modificationDate">The modification date of the object.</param>
    /// <returns>A tuple containing the final creation and modification dates.</returns>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="modificationDate"/> is in the future or before the <paramref name="creationDate"/>.</exception>
    public static (DateTime CreationDate, DateTime ModificationDate) CheckDates(DateTime? creationDate,
        DateTime? modificationDate)
    {
        DateTime finalCreationDate;
        DateTime finalModificationDate;

        if (modificationDate.HasValue)
        {
            if (modificationDate.Value > DateTime.Now)
                throw new ArgumentException("The modification date cannot be in the future.", nameof(modificationDate));
            else if (creationDate.HasValue && modificationDate.Value < creationDate.Value)
                throw new ArgumentException("The modification date cannot be before the creation date.",
                    nameof(modificationDate));
            finalModificationDate = modificationDate.Value;
        }
        else
            finalModificationDate = DateTime.Now;

        if (creationDate.HasValue)
        {
            if (creationDate.Value > DateTime.Now)
                throw new ArgumentException("The creation date cannot be in the future.", nameof(creationDate));
            finalCreationDate = creationDate.Value;
        }
        else
        {
            finalCreationDate = DateTime.Now;
            finalModificationDate = DateTime.Now;
        }

        return (finalCreationDate, finalModificationDate);
    }
}