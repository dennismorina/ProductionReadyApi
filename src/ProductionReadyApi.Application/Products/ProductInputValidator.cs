using ProductionReadyApi.Application.Common.Exceptions;

namespace ProductionReadyApi.Application.Products;

internal static class ProductInputValidator
{
    public static void Validate(string? sku, string? name, decimal price, int stockQuantity)
    {
        var errors = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        AddRequiredAndLengthError(errors, "sku", sku, "SKU", 64);
        AddRequiredAndLengthError(errors, "name", name, "Name", 200);

        if (price < 0)
        {
            Add(errors, "price", "Price must be greater than or equal to 0.");
        }

        if (stockQuantity < 0)
        {
            Add(errors, "stockQuantity", "Stock quantity must be greater than or equal to 0.");
        }

        if (errors.Count == 0)
        {
            return;
        }

        throw new RequestValidationException(
            errors.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.ToArray(),
                StringComparer.OrdinalIgnoreCase));
    }

    public static void ValidatePaging(int page, int pageSize)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        if (page < 1)
        {
            errors["page"] = ["Page must be greater than or equal to 1."];
        }

        if (pageSize is < 1 or > 100)
        {
            errors["pageSize"] = ["Page size must be between 1 and 100."];
        }

        if (errors.Count > 0)
        {
            throw new RequestValidationException(errors);
        }
    }

    private static void AddRequiredAndLengthError(
        Dictionary<string, List<string>> errors,
        string key,
        string? value,
        string displayName,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Add(errors, key, $"{displayName} is required.");
            return;
        }

        if (value.Trim().Length > maxLength)
        {
            Add(errors, key, $"{displayName} must not exceed {maxLength} characters.");
        }
    }

    private static void Add(Dictionary<string, List<string>> errors, string key, string message)
    {
        if (!errors.TryGetValue(key, out var messages))
        {
            messages = [];
            errors[key] = messages;
        }

        messages.Add(message);
    }
}
