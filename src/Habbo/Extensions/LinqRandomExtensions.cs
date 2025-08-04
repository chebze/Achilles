namespace Achilles.Habbo.Extensions;

public static class LinqRandomExtensions
{
    private static readonly Random _random = new Random();

    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => _random.Next());
    }
}
