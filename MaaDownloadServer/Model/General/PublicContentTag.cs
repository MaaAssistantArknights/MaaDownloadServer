using Semver;

namespace MaaDownloadServer.Model.General;

public record PublicContentTag(
    PublicContentTagType Type,
    Platform Platform,
    Architecture Architecture,
    string Component,
    SemVersion Version,
    SemVersion Target = null);
