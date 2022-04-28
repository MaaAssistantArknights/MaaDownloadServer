namespace MaaDownloadServer.Utils;

// Tag Example
// FullPackage%%Core%%windows%%x64%%1.0.0
// UpdatePackage%%Core&&windows%%x64%%1.0.0%%1.1.0

public static class PublicContentTagUtil
{
    public static PublicContentTag ParseFromTagString(this string tagString)
    {
        var tagParts = tagString.Split("%%");
        if (tagParts.Length < 5)
        {
            throw new ArgumentException("Invalid tag string");
        }

        var typeObj = Enum.Parse<PublicContentTagType>(tagParts[0]);
        var component = tagParts[1];
        var platformObj = Enum.Parse<Platform>(tagParts[2]);
        var archObj = Enum.Parse<Architecture>(tagParts[3]);
        var versionObj = tagParts[4].ParseToSemVer();

        if (typeObj is PublicContentTagType.FullPackage)
        {
            return new PublicContentTag(typeObj, platformObj, archObj, component, versionObj);
        }

        if (tagParts.Length != 6)
        {
            throw new ArgumentException("Invalid tag string");
        }

        var targetObj = tagParts[5].ParseToSemVer();
        return new PublicContentTag(typeObj, platformObj, archObj, component, versionObj, targetObj);
    }

    public static string ParseToTagString(this PublicContentTag tag)
    {
        var (publicContentTagType, p, a, component, semVersion, target) = tag;
        if (publicContentTagType is PublicContentTagType.UpdatePackage && target is null ||
            semVersion is null || p is Platform.UnSupported || a is Architecture.UnSupported)
        {
            throw new ArgumentException("Invalid tag");
        }
        return publicContentTagType is PublicContentTagType.FullPackage ?
            $"{publicContentTagType}%%{component}%%{p}%%{a}%%{semVersion}" :
            $"{publicContentTagType}%%{component}%%{p}%%{a}%%{semVersion}%%{target}";
    }
}
