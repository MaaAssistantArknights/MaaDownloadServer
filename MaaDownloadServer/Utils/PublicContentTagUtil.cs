using Semver;

namespace MaaDownloadServer.Utils;

public static class PublicContentTagUtil
{
    public static PublicContentTag ParseFromTagString(this string tagString)
    {
        var tagParts = tagString.Split("%%");
        if (tagParts.Length < 4)
        {
            throw new ArgumentException("Invalid tag string");
        }

        var typeObj = Enum.Parse<PublicContentTagType>(tagParts[0]);
        var platformObj = Enum.Parse<Platform>(tagParts[1]);
        var archObj = Enum.Parse<Architecture>(tagParts[2]);
        var versionObj = SemVersion.Parse(tagParts[3]);

        if (typeObj is PublicContentTagType.FullPackage)
        {
            return new PublicContentTag(typeObj, platformObj, archObj, versionObj);
        }

        if (tagParts.Length != 5)
        {
            throw new ArgumentException("Invalid tag string");
        }

        var targetObj = SemVersion.Parse(tagParts[4]);
        return new PublicContentTag(typeObj, platformObj, archObj, versionObj, targetObj);
    }

    public static string ParseToTagString(this PublicContentTag tag)
    {
        var (publicContentTagType, p, a, semVersion, target) = tag;
        if (publicContentTagType is PublicContentTagType.UpdatePackage && target is null ||
            semVersion is null || p is Platform.UnSupported || a is Architecture.UnSupported)
        {
            throw new ArgumentException("Invalid tag");
        }
        return publicContentTagType is PublicContentTagType.FullPackage ?
            $"{publicContentTagType}%%{p}%%{a}%%{semVersion}" :
            $"{publicContentTagType}%%{p}%%{a}%%{semVersion}%%{target}";
    }
}
