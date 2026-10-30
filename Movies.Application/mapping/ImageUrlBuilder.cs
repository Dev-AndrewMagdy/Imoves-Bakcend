namespace Movies.Application.mapping;

internal static class ImageUrlBuilder
{
    public static string? Build(string? path, string? baseUrl, string size)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(baseUrl)
            || Uri.TryCreate(path, UriKind.Absolute, out _))
        {
            return path;
        }

        return $"{baseUrl.TrimEnd('/')}/{size}/{path.TrimStart('/')}";
    }
}
