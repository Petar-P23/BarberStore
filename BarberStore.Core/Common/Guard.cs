using static BarberStore.Core.Constants.ExceptionMessageConstants;

namespace BarberStore.Core.Common;

public static class Guard
{
    public static void AgainstNull(object? value, string? name = null)
    {
        name ??= nameof(value);
        if (value == null)
        {
            throw new ArgumentNullException(string.Format(GuardNullException, name));
        }
    }

    public static void AgainstNullOrWhiteSpaceString(string? value, string? name = null)
    {
        name ??= nameof(value);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(string.Format(GuardStringException, name));
        }
    }
}