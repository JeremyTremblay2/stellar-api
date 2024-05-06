namespace StellarApi.Helpers;

public static class DateHelper
{
    public static (DateTime CreationDate, DateTime ModificationDate) CheckDates(DateTime? modificationDate,
        DateTime? creationDate)
    {
        DateTime finalCreationDate;
        DateTime finalModificationDate;

        if (modificationDate.HasValue)
        {
            if (modificationDate.Value > DateTime.UtcNow)
                throw new ArgumentException("The modification date cannot be in the future.", nameof(modificationDate));
            else if (creationDate.HasValue && modificationDate.Value < creationDate.Value)
                throw new ArgumentException("The modification date cannot be before the creation date.",
                    nameof(modificationDate));
            finalModificationDate = modificationDate.Value;
        }
        else
            finalModificationDate = DateTime.UtcNow;

        if (creationDate.HasValue)
        {
            if (creationDate.Value > DateTime.UtcNow)
                throw new ArgumentException("The creation date cannot be in the future.", nameof(creationDate));
            finalCreationDate = creationDate.Value;
        }
        else
        {
            finalCreationDate = DateTime.UtcNow;
            finalModificationDate = DateTime.UtcNow;
        }

        return (finalCreationDate, finalModificationDate);
    }
}