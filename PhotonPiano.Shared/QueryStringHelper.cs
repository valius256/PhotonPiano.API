using System.Collections;
using System.Globalization;
using System.Text;

namespace PhotonPiano.Shared;

public class QueryStringHelper
{
    public string ToQueryString(object obj)
    {
        if (obj == null)
            return string.Empty;

        var properties = obj.GetType().GetProperties()
            .Where(p => p.GetValue(obj) != null) // Exclude null values
            .SelectMany(p =>
            {
                var value = p.GetValue(obj);

                // Handle collections (e.g., List<Guid>, List<Shift>)
                if (value is IEnumerable enumerable && value is not string)
                    return enumerable
                        .Cast<object>()
                        .Select(item => $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(item.ToString())}");

                // Handle other data types
                return new[]
                    { $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(value?.ToString() ?? string.Empty)}" };
            });

        return "?" + string.Join("&", properties);
    }

    public string NormalizeString(string input)
    {
        return string.Concat(input.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
            .ToLowerInvariant();
    }
}