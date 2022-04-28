using Semver;

namespace MaaDownloadServer.Extensions;

public static class SemanticVersionExtension
{
    public static SemVersion ParseToSemVer(this string semverString)
    {
        return SemVersion.Parse(semverString, SemVersionStyles.Strict);
    }

    public static bool TryParseToSemVer(this string semverString, out SemVersion semVersion)
    {
        return SemVersion.TryParse(semverString, SemVersionStyles.Strict, out semVersion);
    }
}
