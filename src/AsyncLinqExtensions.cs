namespace AlphabetUpdateServer;

public static class AsyncLinqExtensions
{
    // helper method for converting IEnumerable into IAsyncEnumerable without compiler warning
    #pragma warning disable CS1998
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> enumerable)
    {
        foreach (var item in enumerable)
        {
            yield return item;
        }
    }
}