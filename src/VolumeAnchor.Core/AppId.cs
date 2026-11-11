
namespace VolumeAnchor.Core;

public enum AppIdKind
{
    ExecutablePath,
    AppUserModelId
}

public sealed class AppId : IEquatable<AppId>
{
    public AppIdKind Kind { get; }
    public string Value { get; }

    private AppId(AppIdKind kind, string value)
    {
        Kind = kind;
        Value = value;
    }

    public static AppId FromExecutablePath(string? path)
    {
        if (path is null || !IsValidWindowsPath(path))
            throw new ArgumentException("A fully qualified Windows executable path is required.", nameof(path));

        return new AppId(AppIdKind.ExecutablePath, path);
    }

    public static AppId FromAppUserModelId(string? appUserModelId)
    {
        if (appUserModelId is null || !IsValidAppUserModelId(appUserModelId))
            throw new ArgumentException("A valid packaged application ID is required.", nameof(appUserModelId));

        return new AppId(AppIdKind.AppUserModelId, appUserModelId);
    }

    public bool Equals(AppId? other)
    {
        if (other is null || Kind != other.Kind) return false;

        var comparer = Kind == AppIdKind.AppUserModelId
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal;

        return comparer.Equals(Value, other.Value);
    }

    public override bool Equals(object? obj) => obj is AppId other && Equals(other);

    public override int GetHashCode()
    {
        var comparer = Kind == AppIdKind.AppUserModelId
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal;

        return HashCode.Combine(Kind, comparer.GetHashCode(Value));
    }

    private static bool IsValidWindowsPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || path != path.Trim()) return false;

        foreach (var character in path)
        {
            if (char.IsControl(character) || character == '/') return false;
        }

        var isDrivePath = path.Length >= 4 &&
            char.IsAsciiLetter(path[0]) &&
            path[1] == ':' &&
            path[2] == '\\';

        var isUncPath = path.StartsWith(@"\\", StringComparison.Ordinal) &&
            path.Length >= 5 &&
            path[2] != '?' &&
            path[2] != '.';

        if (!isDrivePath && !isUncPath) return false;

        var segments = (isDrivePath ? path[3..] : path[2..]).Split('\\');
        if (segments.Length < (isDrivePath ? 1 : 3)) return false;

        foreach (var segment in segments)
        {
            if (string.IsNullOrWhiteSpace(segment) || segment is "." or "..") return false;
        }

        return true;
    }

    private static bool IsValidAppUserModelId(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value != value.Trim()) return false;

        var separator = value.IndexOf('!');
        if (separator <= 0 || separator == value.Length - 1 || value.LastIndexOf('!') != separator)
            return false;

        foreach (var character in value)
        {
            if (char.IsWhiteSpace(character) || char.IsControl(character) || character is '\\' or '/')
                return false;
        }

        return true;
    }
}
