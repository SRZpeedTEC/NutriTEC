namespace NutriTEC.Domain.Enums;

public enum ProductStatus
{
    Active,
    Inactive,
    PendingReview,
    Rejected
}

public static class ProductStatusExtensions
{
    public static string ToDatabaseValue(this ProductStatus status)
    {
        // The database stores product statuses with uppercase values defined by the CHECK constraint.
        return status switch
        {
            ProductStatus.Active => "ACTIVE",
            ProductStatus.Inactive => "INACTIVE",
            ProductStatus.PendingReview => "PENDING_REVIEW",
            ProductStatus.Rejected => "REJECTED",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported product status.")
        };
    }

    public static ProductStatus FromDatabaseValue(string value)
    {
        // EF uses this conversion to translate persisted strings back into the domain enum.
        return value switch
        {
            "ACTIVE" => ProductStatus.Active,
            "INACTIVE" => ProductStatus.Inactive,
            "PENDING_REVIEW" => ProductStatus.PendingReview,
            "REJECTED" => ProductStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported product status value.")
        };
    }
}
